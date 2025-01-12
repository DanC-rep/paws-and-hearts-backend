using MassTransit;
using PawsAndHearts.Accounts.Application.UseCases.CreateVolunteerAccount;
using PawsAndHearts.SharedKernel;
using PawsAndHearts.VolunteerRequests.Contracts.Messaging;

namespace PawsAndHearts.Accounts.Application.Consumers;

public class ApproveVolunteerRequestEventConsumer : IConsumer<ApproveVolunteerRequestEvent>
{
    private readonly CreateVolunteerAccountHandler _handler;

    public ApproveVolunteerRequestEventConsumer(CreateVolunteerAccountHandler handler)
    {
        _handler = handler;
    }

    public async Task Consume(ConsumeContext<ApproveVolunteerRequestEvent> context)
    {
        var command = new CreateVolunteerAccountCommand(
            context.Message.UserId, 
            context.Message.Experience,
            context.Message.Requisites);

        var result = await _handler.Handle(command);

        if (result.IsFailure)
            throw new CanNotCreateRecordException(result.Error.First());
    }
}