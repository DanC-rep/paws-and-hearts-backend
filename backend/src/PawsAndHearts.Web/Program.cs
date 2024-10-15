using PawsAndHearts.Accounts.Application;
using PawsAndHearts.Accounts.Infrastructure;
using PawsAndHearts.BreedManagement.Application;
using PawsAndHearts.BreedManagement.Infrastructure;
using PawsAndHearts.BreedManagement.Presentation;
using PawsAndHearts.PetManagement.Application;
using PawsAndHearts.PetManagement.Infrastructure;
using PawsAndHearts.PetManagement.Presentation;
using PawsAndHearts.Web.Extensions;
using Serilog;
using Serilog.Events;

namespace PawsAndHearts.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Seq(builder.Configuration.GetConnectionString("Seq") 
                         ?? throw new ArgumentNullException("Seq"))
            .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
            .CreateLogger();
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCustomSwaggerGen();

        builder.Services.AddSerilog();

        builder.Services
            .AddPetManagementInfrastructure(builder.Configuration)
            .AddBreedManagementInfrastructure(builder.Configuration)
            .AddAccountsInfrastructure(builder.Configuration)
            .AddPetManagementApplication()
            .AddBreedManagementApplication()
            .AddAccountsApplication()
            .AddPetManagementPresentation()
            .AddBreedManagementPresentation();

        builder.Services.AddAuthServices(builder.Configuration);

        var app = builder.Build();

        app.UseExceptionMiddleware();

        app.UseSerilogRequestLogging();
            
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();
            
        app.MapControllers();

        await app.RunAsync();
    }
}