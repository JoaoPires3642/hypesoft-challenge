using Hypesoft.Infrastructure.Data;
using Hypesoft.Infrastructure.Repositories;
using Hypesoft.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;
using Serilog;

try
{
    Log.Information("Iniciando API");
    
    var builder = WebApplication.CreateBuilder(args);

    // Configuração do Serilog 
    builder.Host.UseSerilog((context, services, configuration) => configuration
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId());

    //Controllers
    builder.Services.AddControllers();

    // Swagger/OpenAPI configuration
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "Hypesoft Gestão de Produtos API",
            Version = "v1.0",
            Description = "Sistema de Gestão de Produtos - Desafio Hypesoft.",
        });
        
        options.EnableAnnotations();
        
        // Incluir XML comments para documentação
        var xmlFile = Path.Combine(AppContext.BaseDirectory, "Hypesoft.API.xml");
        if (File.Exists(xmlFile))
        {
            options.IncludeXmlComments(xmlFile);
        }
    });

    // Health Checks
    builder.Services.AddHealthChecks();

    // MediatR
    builder.Services.AddMediatR(cfg => 
        cfg.RegisterServicesFromAssembly(typeof(Hypesoft.Application.Commands.CreateProductCommand).Assembly));

    // Repositories
    builder.Services.AddScoped<IProductRepository, ProductRepository>();

    //EF Core com MONGO
    var connectionString = builder.Configuration.GetConnectionString("MongoDb");
    var databaseName = builder.Configuration.GetValue<string>("ConnectionStrings:DatabaseName");

    builder.Services.AddDbContext<AppDbContext>();

    var app = builder.Build();

    // Serilog request logging
    app.UseSerilogRequestLogging();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hypesoft API v1");
            options.RoutePrefix = "swagger";
            options.DefaultModelsExpandDepth(2);
        });
    }

    //Endpoint de health 
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";
            var response = new
            {
                status = report.Status.ToString(),
                checks = report.Entries.Select(x => new
                {
                    component = x.Key,
                    status = x.Value.Status.ToString(),
                    description = x.Value.Description
                }),
                duration = report.TotalDuration
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    });

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "A aplicação falhou ao iniciar!");
}
finally
{
    Log.CloseAndFlush();
}
