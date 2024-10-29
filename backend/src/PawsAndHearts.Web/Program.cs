using PawsAndHearts.Accounts.Infrastructure.Seeding;
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
            .AddPetManagementModule(builder.Configuration);

        builder.Services.AddAuthServices(builder.Configuration);

        var app = builder.Build();
        
        var accountsSeeder = app.Services.GetRequiredService<AccountSeeder>();
        
        await accountsSeeder.SeedAsync();

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