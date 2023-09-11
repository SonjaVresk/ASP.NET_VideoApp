using System.ComponentModel;

namespace VideoApp.WEB.Admin.ViewModels
{
    public class VMCountry
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [DisplayName("Country")]
        public string Name { get; set; }
    }
}
