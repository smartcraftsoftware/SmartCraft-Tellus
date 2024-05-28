using Asp.Versioning;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Application.Services;
using SmartCraft.Core.Tellus.Domain.Services;
using SmartCraft.Core.Tellus.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using SmartCraft.Core.Tellus.Application.Client;
using System.Reflection;
using SmartCraft.Core.Tellus.Domain.Validators;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

//Create logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
    .AddJsonFile("seri-log.config.json")
    .Build())
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Logging.ClearProviders();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger, dispose: true));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Tellus - A part of Smartcraft", 
        Version = "v1",
        Description = "Api to get ESG reports for vehicles."
    });
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddApiVersioning(
    o =>
    {
    o.AssumeDefaultVersionWhenUnspecified = true;
        o.DefaultApiVersion = new ApiVersion(new DateOnly(2016, 7, 1));
    });

//DB
builder.Services.AddDbContext<VehicleContext>(options => options.UseNpgsql(Configuration.GetConnectionString("VehicleConnectionString")));
builder.Services.AddDbContext<TenantContext>(options => options.UseNpgsql(Configuration.GetConnectionString("VehicleConnectionString")));

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<HttpClient>();

//Clients
builder.Services.AddScoped<IVehicleClient, ScaniaClient>();
builder.Services.AddScoped<IVehicleClient, VolvoClient>();
builder.Services.AddScoped<IVehicleClient, ManClient>();

//Services
builder.Services.AddScoped<IVehiclesService, VehiclesService>();
builder.Services.AddScoped<IEsgService, EsgService>();
builder.Services.AddScoped<ITenantService, TenantService>();

//Validators
builder.Services.AddScoped<EsgReportValidator>();


//Context
builder.Services.AddScoped<TenantContext>();
builder.Services.AddScoped<VehicleContext>();

//Repositories
builder.Services.AddScoped<IRepository<SmartCraft.Core.Tellus.Infrastructure.Models.EsgVehicleReport, VehicleContext>, Repository<SmartCraft.Core.Tellus.Infrastructure.Models.EsgVehicleReport, VehicleContext>>();
builder.Services.AddScoped<IRepository<SmartCraft.Core.Tellus.Infrastructure.Models.Vehicle, VehicleContext>, Repository<SmartCraft.Core.Tellus.Infrastructure.Models.Vehicle, VehicleContext>>();
builder.Services.AddScoped<IRepository<SmartCraft.Core.Tellus.Infrastructure.Models.StatusReport, VehicleContext>, Repository<SmartCraft.Core.Tellus.Infrastructure.Models.StatusReport, VehicleContext>>();
builder.Services.AddScoped<IRepository<SmartCraft.Core.Tellus.Infrastructure.Models.Tenant, TenantContext>, Repository<SmartCraft.Core.Tellus.Infrastructure.Models.Tenant, TenantContext>>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.Run();
