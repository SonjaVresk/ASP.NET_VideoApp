using System.ComponentModel;

namespace VideoApp.WEB.User.ViewModels
{
    public class VMVideo
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public int GenreId { get; set; }

        [DisplayName("Genre")]
        public string? GenreName { get; set; }

        [DisplayName("Duration")]
        public TimeSpan TotalTime { get; set; }

        [DisplayName("Link")]
        public string? StreamingUrl { get; set; }

        public int? ImageId { get; set; }

        public string? ImageURl { get; set; }

        public List<string> Tags { get; set; }

    }
}
