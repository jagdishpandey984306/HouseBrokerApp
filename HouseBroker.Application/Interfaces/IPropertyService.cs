using HouseBroker.Application.Dtos.Auth;
using HouseBroker.Application.Dtos.General;
using HouseBroker.Application.Dtos.Property;
using HouseBroker.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<GeneralServiceResponseDto> CreatePropertyAsync(PropertyDto propertyDto);
        Task<GeneralServiceResponseDto> UpdatePropertyAsync(PropertyDto propertyDto);
        Task<IList<PropertyViewModel>> ListPropertyAsync();
        Task<Property> PropertyGetByIdAsync(int id);
        Task<GeneralServiceResponseDto> PropertyDeleteAsync(int id);

        Task<IList<PropertyViewModel>> PropertySearchAsync(PropertySearch search);
    }
}
