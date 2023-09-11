using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using VideoApp.BL.DALModels;

namespace VideoApp.WEB.User.ViewModels
{
    public class VMUser
    {
        public int Id { get; set; }

        [DisplayName("Username")]
        public string Username { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        public string? Phone { get; set; }

        [DisplayName("CountryId")]
        public int CountryOfResidenceId { get; set; }


    }
}
