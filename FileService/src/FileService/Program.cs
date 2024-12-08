using FileService;
using FileService.Endpoints;
using FileService.Middlewares;
using Hangfire;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddFileServiceServices(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEndpoints();

var app = builder.Build();

app.UseExceptionMiddleware();

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

app.MapEndpoints();

app.Run();