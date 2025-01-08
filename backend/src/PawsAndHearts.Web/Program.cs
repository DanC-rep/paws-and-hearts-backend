using PawsAndHearts.Accounts.Infrastructure.Seeding;
using PawsAndHearts.Framework.Middlewares;
using PawsAndHearts.Web.Extensions;
using Serilog;

namespace PawsAndHearts.Web;

public class Program
{
    public static async Task Main(string[] args)
    {
        DotNetEnv.Env.Load();
        
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddLogging(builder.Configuration);
        
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddCustomSwaggerGen();

        builder.Services
            .AddAccountsModule(builder.Configuration)
            .AddBreedManagementModule(builder.Configuration)
            .AddPetManagementModule(builder.Configuration)
            .AddDiscussionsModule(builder.Configuration)
            .AddVolunteerRequestsModule(builder.Configuration);

        builder.Services.AddApplicationLayers();

        builder.Services.AddAuthServices(builder.Configuration);

        var app = builder.Build();
        
        var accountsSeeder = app.Services.GetService<AccountSeeder>();

        if (accountsSeeder != null) await accountsSeeder.SeedAsync();

        app.UseExceptionMiddleware();

        app.UseSerilogRequestLogging();
            
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors(config =>
        {
            config.WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });

        app.UseAuthentication();
        app.UseScopedDataMiddleware();
        app.UseAuthorization();
            
        app.MapControllers();

        await app.RunAsync();
    }
}