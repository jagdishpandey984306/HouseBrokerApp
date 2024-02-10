using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Application.Dtos.Property
{
    public class PropertySearch
    {
        public string? propertyType { get; set; }
        public string? location { get; set; }
        public double minPrice { get; set; } = 0;
        public double maxPrice { get; set; } = 0;
    }
}
