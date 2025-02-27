using Card.API.Mappings;
using Card.Application.Interfaces;
using Card.Application.Validators;
using Card.Infrastructure.Services;
using FluentValidation;

namespace Card.API.DependencyInjection
{
    /// <summary>
    /// Provides methods for registering dependencies and application services
    /// within the dependency injection container. Includes configuration for
    /// components such as MediatR, AutoMapper, and FluentValidation.
    /// </summary>
    public static class DiContainer
    {
        /// <summary>
        /// Registers application services for the dependency injection container,
        /// including MediatR, AutoMapper, and FluentValidation configurations.
        /// </summary>
        /// <param name="services">
        /// An instance of <see cref="IServiceCollection"/> that is used to register services.
        /// </param>
        /// <returns>
        /// The modified <see cref="IServiceCollection"/> after adding application services.
        /// </returns>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ICardService, CardService>();
            services.AddScoped<ICardActionService, CardActionService>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(ICardService).Assembly));
            services.AddAutoMapper(typeof(CardMappingProfile));
            services.AddValidatorsFromAssemblyContaining<GetCardDetailsQueryValidator>();
            return services;
        }
    }
}