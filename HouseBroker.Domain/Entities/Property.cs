using HouseBroker.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HouseBroker.Domain.Entities
{
    public class Property : BaseEntity
    {
        [Column(TypeName = "nvarchar(100)")]
        public string PropertyName { get; set; }
        [Column(TypeName ="nvarchar(100)")]
        public string PropertyType { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string Location { get; set; }
        public double Price { get; set; }
        [Column(TypeName = "nvarchar(250)")]
        public string ImagePath { get; set; }
        [Column(TypeName = "nvarchar(1500)")]
        public string Features { get; set; }
        [Column(TypeName = "nvarchar(500)")]
        public string Description { get; set; }
        [Column(TypeName = "nvarchar(20)")]
        public string Contact { get; set; }
        public Guid UserId { get; set; }
    }
}
