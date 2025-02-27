using Card.API.Mappings;
using Card.Application.Interfaces;
using Card.Application.Validators;
using FluentValidation;
using Card.API.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configure services
ConfigureServices(builder);

var app = builder.Build();

// Configure the HTTP request pipeline
ConfigureMiddleware(app);

await app.UseOcelot();
await app.RunAsync();

static void ConfigureServices(WebApplicationBuilder builder)
{
    // Register controllers
    builder.Services.AddControllers();

    // Configure CORS
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy", policy =>
        {
            policy.AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowAnyOrigin();
        });
    });
 
    // Configure AutoMapper
    builder.Services.AddAutoMapper(typeof(CardMappingProfile));

    // Register MediatR
    builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ICardService).Assembly));

    // Register FluentValidation
    builder.Services.AddValidatorsFromAssemblyContaining<GetCardDetailsQueryValidator>();

    // Register application-specific services
    builder.Services.AddApplicationServices();

    // Configure Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

static void ConfigureMiddleware(WebApplication app)
{
    // Configure middleware for development environment
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseRouting();
    app.UseCors("CorsPolicy");
    app.UseAuthorization();

    // Map controllers
    app.MapControllers();

    // Map custom endpoints
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapGet("/", async context =>
        {
            await context.Response.WriteAsync("Ocelot is running here ...");
        });
    });
}