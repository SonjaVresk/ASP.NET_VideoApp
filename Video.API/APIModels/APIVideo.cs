using System.ComponentModel;
using VideoApp.BL.DALModels;

namespace Video.API.APIModels
{
    public class APIVideo
    {

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [DisplayName("Image")]
        public string? Image { get; set; }

        [DisplayName("Total time")]
        public string TotalTime { get; set; }
        

        [DisplayName("Streaming URL")]
        public string? StreamingUrl { get; set; }

        public string Genre { get; set; }

        public List<string>? Tags { get; set; }

    }
}
