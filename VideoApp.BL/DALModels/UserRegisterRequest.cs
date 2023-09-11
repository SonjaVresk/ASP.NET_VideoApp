
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApp.BL.DALModels
{
    public class UserRegisterRequest 
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public int CountryOfResidenceId { get; set; }

        public string Phone { get; set; }
    }
}
