using Hypesoft.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//EF Core com MONGO
var connectionString = builder.Configuration.GetConnectionString("MongoDb");
var databaseName = builder.Configuration.GetValue<string>("ConnectionStrings:DatabaseName");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMongoDB(connectionString ?? "", databaseName ?? ""));