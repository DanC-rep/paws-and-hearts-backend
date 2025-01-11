using System.Text.Json;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PawsAndHearts.VolunteerRequests.Contracts;
using PawsAndHearts.VolunteerRequests.Infrastructure.DbContexts;
using Polly;
using Polly.Retry;

namespace PawsAndHearts.VolunteerRequests.Infrastructure.Outbox;

public class ProcessOutboxMessageService
{
    private readonly WriteDbContext _dbContext;
    private readonly IPublishEndpoint _publihser;
    private readonly ILogger<ProcessOutboxMessageService> _logger;

    public ProcessOutboxMessageService(
        WriteDbContext dbContext,
        IPublishEndpoint publisher,
        ILogger<ProcessOutboxMessageService> logger)
    {
        _dbContext = dbContext;
        _publihser = publisher;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext
            .Set<OutboxMessage>()
            .OrderBy(m => m.OccuredOnUtc)
            .Where(m => m.ProcessedOnUtc == null)
            .Take(100)
            .ToListAsync(cancellationToken);

        if (messages.Count == 0)
            return;

        var pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(2),
                ShouldHandle = new PredicateBuilder().Handle<Exception>(),
                OnRetry = retryArgs =>
                {
                    _logger.LogCritical(
                        retryArgs.Outcome.Exception,
                        "Current attempt: {attemptNumber}",
                        retryArgs.AttemptNumber);

                    return ValueTask.CompletedTask;
                }
            })
            .Build();

        var processingTasks = messages.Select(m => ProcessMessageAsync(m, pipeline, cancellationToken));
        
        await Task.WhenAll(processingTasks);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save changes to the database");
        }
    }

    private async Task ProcessMessageAsync(
        OutboxMessage message, 
        ResiliencePipeline pipeline,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var messageType = AssemblyReference.Assembly.GetType(message.Type)
                              ?? throw new NullReferenceException("Message type not found");

            var deserializedMessage = JsonSerializer.Deserialize(message.Payload, messageType)
                                      ?? throw new NullReferenceException("Message payload not found");

            await pipeline.ExecuteAsync(async token =>
            {
                await _publihser.Publish(deserializedMessage, messageType, token);

                message.ProcessedOnUtc = DateTime.UtcNow;
            }, cancellationToken);
        }
        catch (Exception ex)
        {
            message.Error = ex.Message;
            message.ProcessedOnUtc = DateTime.UtcNow;
            _logger.LogError(ex, "Failed to process message id: {messageId}", message.Id);
        }
    }
}