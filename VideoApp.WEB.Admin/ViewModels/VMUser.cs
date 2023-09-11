using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VideoApp.BL.DALModels;

namespace VideoApp.WEB.Admin.ViewModels
{
    public class VMUser
    {
        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        [DisplayName("User name")]
        public string Username { get; set; }

        [DisplayName("First name")]
        [Required]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        [Required]
        public string LastName { get; set; }

        [DisplayName("E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public bool IsConfirmed { get; set; }

        public string? SecurityToken { get; set; }

        public int CountryOfResidenceId { get; set; }

        public SelectListItem[]? CountryList { get; set; }
    }
}
