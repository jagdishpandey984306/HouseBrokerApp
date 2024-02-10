using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseBroker.Application.Dtos.Auth
{
    public class RegisterDto
    {
        [DisplayName("Firstname")]
        [Required(ErrorMessage = "Firstname is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Lastname is required")]
        [DisplayName("Lastname")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "UserName is required")]
        [DisplayName("Username")]
        public string? UserName { get; set; }
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }
        public string? Address { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string? Role { get; set; }

        public IList<SelectListItem> roles { get; set; } = new List<SelectListItem>();

    }
}
