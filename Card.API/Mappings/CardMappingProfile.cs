using AutoMapper;
using Card.Application.Models;
using Card.Domain.Entities;

namespace Card.API.Mappings
{
    /// <summary>
    /// Defines the mapping configuration between domain entities and data transfer objects (DTOs) for card-related objects.
    /// </summary>
    /// <remarks>
    /// This mapping profile is used by AutoMapper to handle the transformation of data between the <see cref="Card.Domain.Entities.CardDetails"/> domain entity
    /// and the <see cref="Card.Application.Models.CardDetailsDto"/> data transfer object. This ensures that the application layers can communicate effectively while
    /// maintaining separation of concerns.
    /// </remarks>
    /// <example>
    /// The mapping defined in this profile includes:
    /// - Mapping of <c>CardType</c> from an enumeration to its string representation.
    /// - Mapping of <c>CardStatus</c> from an enumeration to its string representation.
    /// - Direct mapping of the <c>IsPinSet</c> boolean property.
    /// </example>
    public class CardMappingProfile: Profile
    {
        public CardMappingProfile()
        {
            CreateMap<CardDetails, CardDetailsDto>()
                .ForMember(dest => dest.CardType, opt => opt.MapFrom(src => src.CardType.ToString()))
                .ForMember(dest => dest.CardStatus, opt => opt.MapFrom(src => src.CardStatus.ToString()))
                .ForMember(dest => dest.IsPinSet, opt => opt.MapFrom(src => src.IsPinSet));
            
            CreateMap<string[], CardAllowedActionsDto>()
                .ForMember(dest => dest.AllowedActions, opt => opt.MapFrom(src => src));
        }
    }
}