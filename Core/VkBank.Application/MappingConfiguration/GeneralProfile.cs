using AutoMapper;
using VkBank.Application.Features.Menu.Commands;
using VkBank.Domain.Entities;

namespace VkBank.Application.MappingConfiguration
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            // Queries

            // Commands
            CreateMap<EntityMenu, CreateMenuCommandRequest>().ReverseMap();
            CreateMap<EntityMenu, UpdateMenuCommandRequest>().ReverseMap();
        }
    }
}
