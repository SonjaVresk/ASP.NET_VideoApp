using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using VideoApp.BL.DALModels;

namespace VideoApp.WEB.Admin.ViewModels
{
    public class VMVideo
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int GenreId { get; set; }
        public int GenreName { get; set; }

        [DisplayName("Duration")]
        public TimeSpan TotalTime { get; set; }

        [DisplayName("Link")]
        public string? StreamingUrl { get; set; }

        public int? ImageId { get; set; }

        public string? ImageURl { get; set; }

        public List<string> Tags { get; set; }

        public  SelectListItem[]? GenreList { get; set; }
        public  SelectListItem[]? ImageList { get; set; }
    }
}
