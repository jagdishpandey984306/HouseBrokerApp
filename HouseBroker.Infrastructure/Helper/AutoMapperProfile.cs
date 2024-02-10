using AutoMapper;
using HouseBroker.Application.Dtos.Property;
using HouseBroker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Infrastructure.Helper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Property, PropertyDto>().ReverseMap();
            CreateMap<Property, PropertyViewModel>().ReverseMap();
        }
    }
}
