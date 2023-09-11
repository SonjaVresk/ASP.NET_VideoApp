using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VideoApp.WEB.User.ViewModels
{
    public class VMRegister
    {
        [DisplayName("User name")]
        public string Username { get; set; }

        [DisplayName("E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("Confirm e-mail")]
        [Compare("Email")]
        public string Email2 { get; set; }

        [DisplayName("First name")]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        public string LastName { get; set; }
        
        public string Phone { get; set; }

        [DisplayName("Country")]
        public int CountryId { get; set; }

        public SelectListItem[]? Countries { get; set; }

        [DisplayName("Password")]
        public string Password { get; set; }

        [DisplayName("Repeat password")]
        [Compare("Password")]
        public string Password2 { get; set; }
    }
}
