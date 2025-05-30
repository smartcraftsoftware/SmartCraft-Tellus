using Asp.Versioning;
using SmartCraft.Core.Tellus.Domain.Repositories;
using SmartCraft.Core.Tellus.Application.Services;
using SmartCraft.Core.Tellus.Domain.Services;
using SmartCraft.Core.Tellus.Infrastructure.Repositories;
using SmartCraft.Core.Tellus.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using SmartCraft.Core.Tellus.Infrastructure.Context;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using Microsoft.AspNetCore.Authentication.Cookies;
using SmartCraft.Core.Tellus.Application.Client;
using System.Reflection;
using Microsoft.OpenApi.Models;
using SmartCraft.Core.Tellus.Domain.Validators;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var Configuration = builder.Configuration;

Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();


//Create logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();
builder.Services.AddSingleton(Log.Logger);
builder.Logging.ClearProviders();
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(logger, dispose: true));

builder.Services.AddHealthChecks();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swaggerGenOptions =>
{
    swaggerGenOptions.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "SmartCraft Tellus API", 
        Version = "v1",
        Description = "API to access real-world data from trucks. More info: [here](https://github.com/smartcraftsoftware/SmartCraft-Tellus/blob/main/GETTING-STARTED.md)"
    });
    swaggerGenOptions.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    swaggerGenOptions.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
    
    
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    swaggerGenOptions.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
builder.Services.AddApiVersioning(apiVersioningOptions =>
{
    apiVersioningOptions.AssumeDefaultVersionWhenUnspecified = true;
    apiVersioningOptions.DefaultApiVersion = new ApiVersion(new DateOnly(2016, 7, 1));
});

//DB
builder.Services.AddDbContext<VehicleContext>(options => options.UseNpgsql(Configuration.GetConnectionString("VehicleConnectionString")));
builder.Services.AddDbContext<CompanyContext>(options => options.UseNpgsql(Configuration.GetConnectionString("TenantConnectionString")));


builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<HttpClient>();

//Clients
builder.Services.AddScoped<IVehicleClient, ScaniaClient>();
builder.Services.AddScoped<IVehicleClient, VolvoClient>();
builder.Services.AddScoped<IVehicleClient, ManClient>();

//Services
builder.Services.AddScoped<IVehiclesService, VehiclesService>();
builder.Services.AddScoped<IEsgService, EsgService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();

//Validators
builder.Services.AddScoped<EsgReportValidator>();


//Context
builder.Services.AddScoped<CompanyContext>();
builder.Services.AddScoped<VehicleContext>();

//Repositories
builder.Services.AddScoped<IRepository<EsgVehicleReport, VehicleContext>, Repository<EsgVehicleReport, VehicleContext>>();
builder.Services.AddScoped<IRepository<Vehicle, VehicleContext>, Repository<Vehicle, VehicleContext>>();
builder.Services.AddScoped<IRepository<StatusReport, VehicleContext>, Repository<StatusReport, VehicleContext>>();
builder.Services.AddScoped<IRepository<IntervalStatusReport, VehicleContext>, Repository<IntervalStatusReport, VehicleContext>>();
builder.Services.AddScoped<ICompanyRepository<Company, CompanyContext>, CompanyRepository>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var vehicleContext = scope.ServiceProvider
        .GetRequiredService<VehicleContext>();
    var tenantContext = scope.ServiceProvider
        .GetRequiredService<CompanyContext>();
    
    vehicleContext.Database.Migrate();
    tenantContext.Database.Migrate();
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(swaggerOptions =>
    {
        swaggerOptions.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
        {
            swaggerDoc.Servers = new List<OpenApiServer> { new () { Url = $"{Configuration.GetValue(typeof(string),"SwaggerProtocol") ?? httpReq.Scheme}://{Configuration.GetValue(typeof(string),"SwaggerBasePath") ?? httpReq.Host.Value}" } };
        });
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthentication();

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
