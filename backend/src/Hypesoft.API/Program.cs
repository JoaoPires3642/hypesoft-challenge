using Hypesoft.Infrastructure.Data;
using Hypesoft.Infrastructure.Repositories;
using Hypesoft.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;


var builder = WebApplication.CreateBuilder(args);

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

// MediatR
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Hypesoft.Application.Commands.CreateProductCommand).Assembly));

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();

//EF Core com MONGO
var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var databaseName = builder.Configuration.GetValue<string>("ConnectionStrings:DatabaseName");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMongoDB(connectionString ?? "", databaseName ?? ""));


    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Hypesoft API v1");
            options.RoutePrefix = "swagger";
            options.DefaultModelsExpandDepth(2);
            options.DefaultModelExpandDepth(2);
        });
    }

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
