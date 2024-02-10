using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Application.Dtos.Property
{
    public class PropertyDto
    {
        public int Id { get; set; } = 0;
        [Required(ErrorMessage = "PropertyName is required")]
        public string? PropertyName { get; set; }

        [Required(ErrorMessage = "PropertyType is required")]
        public string? PropertyType { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string? Location { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [DataType(DataType.Currency)]
        public double Price { get; set; }

        [Required(ErrorMessage = "Image is required")]
        public IFormFile? Image { get; set; }
        public string? Features { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Contact is required")]
        public string? Contact { get; set; }
    }
}
