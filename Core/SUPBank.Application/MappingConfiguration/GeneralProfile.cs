using AutoMapper;
using SUPBank.Application.Features.Menu.Commands;
using SUPBank.Domain.Entities;

namespace SUPBank.Application.MappingConfiguration
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
