using AutoMapper;
using HouseBroker.Application.Dtos.General;
using HouseBroker.Application.Dtos.Property;
using HouseBroker.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HouseBroker.Presentation.Controllers
{
    public class PropertyController : Controller
    {
        #region Properties
        private readonly IPropertyService _propertyService;
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public PropertyController(IPropertyService propertyService, IMapper mapper)
        {
            _propertyService = propertyService;
            _mapper = mapper;
        }
        #endregion

        #region Methods
        /// <summary>
        /// list View
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Broker")]
        public async Task<IActionResult> Index()
        {
            var result = await _propertyService.ListPropertyAsync();
            return View(result);
        }


        /// <summary>
        /// create View
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Broker")]
        public IActionResult Create()
        {
            return View(new PropertyDto());
        }

        /// <summary>
        /// create or update property
        /// </summary>
        /// <param name="propertyDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate([FromForm] PropertyDto propertyDto)
        {
            if (ModelState.IsValid)
            {
                var result = propertyDto.Id == 0 ? await _propertyService.CreatePropertyAsync(propertyDto) : await _propertyService.UpdatePropertyAsync(propertyDto);
                if (result.IsSucceed)
                {
                    return RedirectToAction("Index", "Property");
                }
                return View("Create", propertyDto);
            }
            return View("Create", propertyDto);
        }

        /// <summary>
        /// get the data by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _propertyService.PropertyGetByIdAsync(id);
            var result = _mapper.Map<PropertyDto>(data);
            return View("Create", result);
        }

        /// <summary>
        /// delete the property
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _propertyService.PropertyDeleteAsync(id);
            if (result.IsSucceed)
            {
                return RedirectToAction("Index", "Property");
            }
            return RedirectToAction("Index", "Property");
        }

        /// <summary>
        /// search the property by parameter
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchProperty(PropertySearch search)
        {
            var result = await _propertyService.PropertySearchAsync(search);
            return PartialView("_SearchResultViewPartial", result);
        }
        #endregion
    }
}
