using HouseBroker.Application.Dtos.Auth;
using HouseBroker.Application.Dtos.Property;
using HouseBroker.Application.Interfaces;
using HouseBroker.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace HouseBroker.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        #region Properties
        private readonly IPropertyService _propertyService;
        #endregion

        #region Ctor
        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }
        #endregion

        #region Methods
        //Route -> create property
        [HttpPost]
        [Route("create")]
        [Authorize(Roles = StaticUserRoles.BROKER)]
        public async Task<IActionResult> Create([FromBody] PropertyDto registerDto)
        {
            var result = await _propertyService.CreatePropertyAsync(registerDto);
            return Ok(result);
        }

        //Route -> update property
        [HttpPost]
        [Route("update")]
        [Authorize(Roles = StaticUserRoles.BROKER)]
        public async Task<IActionResult> Update([FromBody] PropertyDto registerDto)
        {
            var result = await _propertyService.UpdatePropertyAsync(registerDto);
            return Ok(result);
        }

        //Route -> return the property by id
        [HttpGet]
        [Route("getbyid")]
        [Authorize(Roles = StaticUserRoles.BROKER)]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var result = await _propertyService.PropertyGetByIdAsync(id);
            return Ok(result);
        }

        //Route -> delete property
        [HttpDelete]
        [Route("delete")]
        [Authorize(Roles = StaticUserRoles.BROKER)]
        public async Task<IActionResult> delete([FromQuery] int id)
        {
            var result = await _propertyService.PropertyDeleteAsync(id);
            return Ok(result);
        }

        //Route -> return property list by location,property-type and price-range
        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> Search([FromQuery] PropertySearch search)
        {
            var result = await _propertyService.PropertySearchAsync(search);
            return Ok(result);
        }

        //Route -> return property list 
        [HttpGet]
        [Route("list")]
        public async Task<IActionResult> List([FromQuery] int page)
        {
            var result = await _propertyService.PropertyListAsync(page, 10);
            return Ok(result);
        }
        #endregion
    }
}
