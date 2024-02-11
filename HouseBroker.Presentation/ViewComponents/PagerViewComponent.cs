using HouseBroker.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace HouseBroker.Presentation.ViewComponents
{
    public class PagerViewComponent : ViewComponent
    {
        public Task<IViewComponentResult> InvokeAsync(PagedResultBase result)
        {
            return Task.FromResult((IViewComponentResult)View("Default", result));
        }
    }
}
