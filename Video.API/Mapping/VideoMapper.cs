using Video.API.APIModels;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Repositories;

namespace Video.API.Mapping
{
    public static class VideoMapper
    {
        
        public static IEnumerable<APIVideo> MapToAPI(IEnumerable<BLVideo> blVideos) =>
          blVideos.Select(x => MapToAPI(x));

        public static APIVideo MapToAPI(BLVideo blVideo)
        {
            var apiVideo = new APIVideo
            {
                Name = blVideo.Name,
                Description = blVideo.Description,
                Genre = blVideo.Genre,
                StreamingUrl = blVideo.StreamingUrl,
                Image = blVideo.Image?.Content,
                Tags = blVideo.Tags,
                TotalTime = blVideo.TotalTime.ToString(@"hh\:mm\:ss")
            };
            
            return apiVideo;
        }
        public static IEnumerable<BLVideo> MapToBl(IEnumerable<APIVideo> apiVideo) =>
           apiVideo.Select(x => MapToBl(x));

        public static BLVideo MapToBl(APIVideo apiVideo)
        {
            var blVideo = new BLVideo
            {
                Name = apiVideo.Name,
                Description = apiVideo.Description,
                Genre = apiVideo.Genre,
                StreamingUrl = apiVideo.StreamingUrl,
                Tags = apiVideo.Tags
                
            };

            if (TimeSpan.TryParse(apiVideo.TotalTime, out var totalTime))
            {
                blVideo.TotalTime = totalTime;
            }
            else
            {
                throw new Exception();
            }

            // Create an instance of the Image class and assign the Content property
            blVideo.Image = new Image
            {
                Content = apiVideo.Image
            };

            return blVideo;
        }      
    }
}
