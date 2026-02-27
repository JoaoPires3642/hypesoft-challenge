using Hypesoft.Infrastructure.Data;
using Hypesoft.Infrastructure.Repositories;
using Hypesoft.Infrastructure.Queries;
using Hypesoft.Application.Infrastructure.Cache;
using Hypesoft.Domain.Repositories;
using Hypesoft.Domain.Queries;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Hypesoft.Application.Behaviors;
using FluentValidation;
using MediatR;
using System.Security.Claims;
using System.Text.Json;

try
{
    Log.Information("Iniciando API");
    
    var builder = WebApplication.CreateBuilder(args);

   
    var keycloakInternalUrl = builder.Configuration["KEYCLOAK_INTERNAL_URL"] ?? "http://keycloak:8080/realms/hypesoft-realm";
    var keycloakExternalUrl = builder.Configuration["KEYCLOAK_EXTERNAL_URL"] ?? "http://localhost:8080/realms/hypesoft-realm";
    var keycloakAudience = builder.Configuration["KEYCLOAK_AUDIENCE"] ?? "account";
    var disableAuth = builder.Configuration.GetValue("DISABLE_AUTH", false);
    var corsOrigins = builder.Configuration["CORS_ORIGINS"]
        ?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        ?? new[]
        {
            "http://localhost:3000",
            "https://crispy-space-train-4jq5vp4q95xvfj4rw-3000.app.github.dev"
        };

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
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Hypesoft Gestão de Produtos API",
            Version = "v1.0",
            Description = "Sistema de Gestão de Produtos - Desafio Hypesoft.",
        });
        
        options.EnableAnnotations();

        if (!disableAuth)
        {
            options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    Implicit = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{keycloakExternalUrl}/protocol/openid-connect/auth"),
                        Scopes = new Dictionary<string, string>
                        {
                            { "openid", "OpenID" }
                        }
                    }
                }
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "oauth2" }
                    },
                    new List<string> { "openid" }
                }
            });
        }
        
        // Incluir XML comments para documentação
        var xmlFile = Path.Combine(AppContext.BaseDirectory, "Hypesoft.API.xml");
        if (File.Exists(xmlFile))
        {
            options.IncludeXmlComments(xmlFile);
        }
    });

    // CORS Configuration
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowFrontend", policy =>
        {
            policy.WithOrigins(corsOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });

    // Health Checks
    builder.Services.AddHealthChecks();

    //Validation
    builder.Services.AddValidatorsFromAssembly(typeof(Hypesoft.Application.Validators.CreateProductValidator).Assembly);

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(Hypesoft.Application.Mappings.ProductMappingProfile).Assembly);

    // MediatR
    builder.Services.AddMediatR(cfg => 
    {
        cfg.RegisterServicesFromAssembly(typeof(Hypesoft.Application.Commands.CreateProductCommand).Assembly);
        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    });

    // Repositories
    builder.Services.AddScoped<IProductRepository, ProductRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

    // Query Services
    builder.Services.AddScoped<IProductQueryService, ProductQueryService>();

    // Cache Services
    builder.Services.AddDistributedMemoryCache();
    builder.Services.AddScoped<ICacheInvalidator, CacheInvalidator>();

    if (!disableAuth)
    {
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Use internal URL for fetching OIDC configuration (container to container)
                options.MetadataAddress = $"{keycloakInternalUrl}/.well-known/openid-configuration";
                options.RequireHttpsMetadata = false; // Apenas para dev/docker
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    // Accept both internal and external issuer URLs
                    ValidIssuers = new[] { keycloakInternalUrl, keycloakExternalUrl },
                    ValidateAudience = true,
                    ValidAudience = keycloakAudience,
                    ValidateLifetime = true
                };
            });

        builder.Services.AddAuthorization();
    }
    else
    {
        builder.Services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new Microsoft.AspNetCore.Authorization.AuthorizationPolicyBuilder()
                .RequireAssertion(_ => true)
                .Build();
            options.FallbackPolicy = options.DefaultPolicy;
        });
    }

    // EF Core com MongoDB (ou InMemory para testes)
    var useInMemoryDb = builder.Configuration.GetValue("USE_IN_MEMORY_DB", false);
    if (useInMemoryDb)
    {
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseInMemoryDatabase("HypesoftInMemory"));
    }
    else
    {
        var connectionString = builder.Configuration.GetConnectionString("MongoDb") ?? throw new InvalidOperationException("MongoDb connection string not found");
        var databaseName = builder.Configuration.GetValue<string>("ConnectionStrings:DatabaseName") ?? throw new InvalidOperationException("DatabaseName not found");

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMongoDB(connectionString, databaseName));
    }
        
    

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
            options.OAuthClientId("hypesoft-client");
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

    app.UseCors("AllowFrontend");
    app.UseHttpsRedirection();
    if (!disableAuth)
    {
        app.UseAuthentication();
    }
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

public partial class Program
{
}
