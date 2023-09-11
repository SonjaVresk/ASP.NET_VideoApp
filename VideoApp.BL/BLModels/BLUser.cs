using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.BLModels
{
    public class BLUser
    {
        public int Id { get; set; }

        [DisplayName("Created")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Deleted")]
        public DateTime? DeletedAt { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        public bool IsConfirmed { get; set; }

        public string? SecurityToken { get; set; }

        public int CountryOfResidenceId { get; set; }

        [DisplayName("Country")]
        public string CountryOfResidence { get; set; }
        
    }
}
