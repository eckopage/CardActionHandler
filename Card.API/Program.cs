using Common.Logging;
using Serilog;
using System.Reflection;
using Card.API.Localization;
using Card.API.Mappings;
using Card.API.Middleware;
using Card.Application.Handlers;
using Card.Application.Interfaces;
using Card.Application.Validators;
using Card.Infrastructure.Services;
using FluentValidation;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog for logging
builder.Host.UseSerilog(Logging.ConfigureLogger);

// Add services to the DI container
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure middleware and the HTTP request pipeline
ConfigureMiddleware(app);

app.Run();
return;

void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    // Configure CORS policy
    services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
    });

    // Add controllers
    services.AddControllers();

    // Enable antiforgery protection and API documentation
    services.AddAntiforgery();
    services.AddEndpointsApiExplorer();

    // Configure Swagger
    ConfigureSwagger(services);

    // Register AutoMapper profiles
    services.AddAutoMapper(typeof(CardMappingProfile));

    // Configure Redis caching
    services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = configuration.GetValue<string>("CacheSettings:ConnectionString");
    });

    // Register localization services
    services.AddSingleton<IStringLocalizerFactory, JsonStringLocalizerFactory>();
    services.AddTransient(typeof(IStringLocalizer<>), typeof(StringLocalizer<>));

    // Register FluentValidation validators
    services.AddValidatorsFromAssemblyContaining<GetCardDetailsQueryValidator>();

    // Register MediatR for CQRS pattern
    services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
        Assembly.GetExecutingAssembly(),
        typeof(GetCardDetailsQueryHandler).Assembly,
        typeof(ICardService).Assembly,
        typeof(ICardActionService).Assembly
    ));

    // Register application services
    services.AddScoped<ICardService, CardService>();
    services.AddScoped<ICardActionService, CardActionService>();
}

void ConfigureSwagger(IServiceCollection services)
{
    services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Card.API", Version = "v1" });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
    });
}

void ConfigureMiddleware(WebApplication application)
{
    // Configure localization
    var supportedCultures = new[] { "en", "pl", "de" };
    var localizationOptions = new RequestLocalizationOptions()
        .SetDefaultCulture("en")
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);

    application.UseRequestLocalization(localizationOptions);

    // Configure error handling and developer tools
    if (application.Environment.IsDevelopment())
    {
        application.UseDeveloperExceptionPage();
        application.UseSwagger();
        application.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Card.API v1");
        });
    }

    // Register global exception handling middleware
    application.UseMiddleware<ExceptionHandlingMiddleware>();

    // Enable CORS
    application.UseCors("CorsPolicy");

    // Configure authentication and authorization
    application.UseAuthentication();
    application.UseAuthorization();

    // Map controllers for handling requests
    application.MapControllers();
}
