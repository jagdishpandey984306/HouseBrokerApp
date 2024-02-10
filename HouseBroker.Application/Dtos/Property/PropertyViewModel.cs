using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Application.Dtos.Property
{
    public class PropertyViewModel
    {
        public int Id { get; set; }
        public string PropertyName { get; set; }
        public string PropertyType { get; set; }
        public string Location { get; set; }
        public double Price { get; set; }
        public string ImagePath { get; set; }
        public string Features { get; set; }
        public string Description { get; set; }
        public string Contact { get; set; }
    }
}
