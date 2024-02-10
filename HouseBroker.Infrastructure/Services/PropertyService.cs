using AutoMapper;
using HouseBroker.Application.Dtos.General;
using HouseBroker.Application.Dtos.Property;
using HouseBroker.Application.Interfaces;
using HouseBroker.Domain.Entities;
using HouseBroker.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Infrastructure.Repositories
{
    public class PropertyService : IPropertyService
    {
        #region Properties
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<PropertyService> _logger;
        private readonly IFileUploadService _fileUploadService;
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<PropertyService> _log;
        #endregion

        #region Ctor
        public PropertyService(ApplicationDbContext dbContext, IMapper mapper, ILogger<PropertyService> logger, IFileUploadService fileUploadService, IHttpContextAccessor httpContext, ILogger<PropertyService> log)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _fileUploadService = fileUploadService;
            _httpContext = httpContext;
            _log = log;
        }
        #endregion

        #region Methods
        public async Task<GeneralServiceResponseDto> CreatePropertyAsync(PropertyDto propertyDto)
        {
            var exists = await _dbContext.Property.AnyAsync(p => p.PropertyType == propertyDto.PropertyType && p.PropertyName == propertyDto.PropertyName);
            if (exists)
            {
                _logger.LogInformation("property with type and name is already existed");
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "Property Already Exists"
                };
            }
            else
            {
                var data = _mapper.Map<Property>(propertyDto);
                data.UserId = Guid.Parse(_httpContext.HttpContext.User.FindFirstValue("UserId"));
                var file = _fileUploadService.UploadFile(propertyDto.Image);
                data.ImagePath = file;
                await _dbContext.Property.AddAsync(data);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("property created successfully");
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    Message = "Property created successfully"
                };
            }
        }

        public async Task<GeneralServiceResponseDto> UpdatePropertyAsync(PropertyDto propertyDto)
        {
            var exists = await _dbContext.Property.AnyAsync(p => p.PropertyType == propertyDto.PropertyType && p.PropertyName == propertyDto.PropertyName);
            if (exists)
            {
                _logger.LogInformation("property with type and name is already existed");
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = false,
                    StatusCode = 409,
                    Message = "Property Already Exists"
                };
            }
            else
            {
                var file = string.Empty;
                var data = _mapper.Map<Property>(propertyDto);
                if (propertyDto.Image == null)
                {
                    data.ImagePath = _fileUploadService.UploadFile(propertyDto.Image);
                }
                else
                {
                    data.ImagePath = _dbContext.Property.FirstOrDefault(p => p.Id == propertyDto.Id).ImagePath;
                }

                _dbContext.Property.Update(data);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("property updated successfully");
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    Message = "Property updated successfully"
                };
            }
        }

        public async Task<IList<PropertyViewModel>> ListPropertyAsync()
        {
            var data = await _dbContext.Property.ToListAsync();
            var list = _mapper.Map<IList<PropertyViewModel>>(data);
            return list;
        }

        public async Task<Property> PropertyGetByIdAsync(int id)
        {
            var data = await _dbContext.Property.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return data;
        }

        public async Task<GeneralServiceResponseDto> PropertyDeleteAsync(int id)
        {
            var data = await PropertyGetByIdAsync(id);
            if (data != null)
            {
                _dbContext.Property.Remove(data);
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("property deleted successfully");
                return new GeneralServiceResponseDto()
                {
                    IsSucceed = true,
                    StatusCode = 201,
                    Message = "Property deleted successfully"
                };
            }
            return new GeneralServiceResponseDto()
            {
                IsSucceed = false,
                StatusCode = 409,
                Message = "Property deleted failed"
            };
        }

        public async Task<IList<PropertyViewModel>> PropertySearchAsync(PropertySearch search)
        {
            var query = _dbContext.Property.AsQueryable();
            if (!string.IsNullOrEmpty(search.propertyType))
            {
                query = query.Where(p => p.PropertyType.ToLower() == search.propertyType.ToLower());
            }
            if (!string.IsNullOrEmpty(search.location))
            {
                query = query.Where(p => p.Location.ToLower().Contains(search.location.ToLower()));
            }

            if (search.minPrice != 0)
            {
                query = query.Where(p => p.Price >= search.minPrice);
            }

            if (search.maxPrice != 0)
            {
                query = query.Where(p => p.Price <= search.maxPrice);
            }

            var data = await query.ToListAsync();
            var list = _mapper.Map<IList<PropertyViewModel>>(data);
            return list;
        }
        #endregion
    }
}
