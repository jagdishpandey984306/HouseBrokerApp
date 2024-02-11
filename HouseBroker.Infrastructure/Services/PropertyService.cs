using AutoMapper;
using Azure;
using HouseBroker.Application.Dtos.General;
using HouseBroker.Application.Dtos.Property;
using HouseBroker.Application.Interfaces;
using HouseBroker.Application.Models;
using HouseBroker.Domain.Entities;
using HouseBroker.Infrastructure.Data;
using HouseBroker.Infrastructure.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        /// <summary>
        /// check property exists or not 
        /// if not exists save the property othewise return message data already exists 
        /// </summary>
        /// <param name="propertyDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// check property exists or not 
        /// if not exists update the property othewise return message data already exists 
        /// </summary>
        /// <param name="propertyDto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// it return the list of property
        /// </summary>
        /// <param name="page">pass page as parameter</param>
        /// <param name="pageSize">pass pagesize as parameter</param>
        /// <returns></returns>
        public async Task<PagedResult<PropertyViewModel>> PropertyListAsync(int page, int pageSize)
        {
            var data = await _dbContext.Property.GetPagedAsync(page, pageSize);
            var list = _mapper.Map<PagedResult<PropertyViewModel>>(data);
            return list;
        }

        /// <summary>
        /// return  property details by id
        /// </summary>
        /// <param name="id">id as parameter</param>
        /// <returns></returns>
        public async Task<Property> PropertyGetByIdAsync(int id)
        {
            var data = await _dbContext.Property.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            return data;
        }

        /// <summary>
        /// delete the property by id
        /// </summary>
        /// <param name="id">id as parameter</param>
        /// <returns></returns>
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

        /// <summary>
        /// search property for user by location,price and propertytype
        /// </summary>
        /// <param name="search"> search as parameter </param>
        /// <returns></returns>
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
