using AutoMapper;
using Ypdf.Web.AccoutAPI.Models;
using Ypdf.Web.Domain.Models.Api.Dto;

namespace Ypdf.Web.AccoutAPI.Mappings;

public class EntityMappingProfile : Profile
{
    public EntityMappingProfile()
    {
        CreateMap<Subscription, SubscriptionDto>();
        CreateMap<UserSubscription, UserSubscriptionDto>();
        CreateMap<User, UserDto>();
    }
}
