using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.BLModels
{
    public class BLVideo
    {
        public int Id { get; set; }

        [DisplayName("Created At")]
        public DateTime CreatedAt { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int GenreId { get; set; }

        public string Genre { get; set; }

        [DisplayName("Total time")]
        public TimeSpan TotalTime { get; set; }

        [DisplayName("Link")]
        public string? StreamingUrl { get; set; }

        public int? ImageId { get; set; }

        [DisplayName("Image")]
        public Image Image { get; set; }

        public List<string> Tags { get; set; }
    }
}
