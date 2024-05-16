using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkBank.Application.Features.Commands.CreateEvent;
using VkBank.Application.Features.Commands.DeleteEvent;
using VkBank.Application.Features.Commands.UpdateEvent;
using VkBank.Application.Features.Queries.GetAllEvent;
using VkBank.Application.Features.Queries.GetEvent;
using VkBank.Domain.Entities;

namespace VkBank.Application.MappingConfiguration
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            //Commands
            CreateMap<Menu, CreateMenuCommandRequest>().ReverseMap();
            CreateMap<Menu, UpdateMenuCommandRequest>().ReverseMap();
            CreateMap<Menu, DeleteMenuCommandRequest>().ReverseMap();

            //Queries
            CreateMap<Menu, GetAllMenuQueryRequest>().ReverseMap();
            CreateMap<Menu, GetMenuQueryRequest>().ReverseMap();
        }
    }
}
