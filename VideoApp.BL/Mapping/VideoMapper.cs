using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class VideoMapper
    {
        public static IEnumerable<Video> MapToDal(IEnumerable<BLVideo> blVideos) =>
           blVideos.Select(x => MapToDal(x));

        public static Video MapToDal(BLVideo blVideo)
        {
            if(blVideo != null)
            {
                return new Video
                {
                    Id = blVideo.Id,
                    CreatedAt = blVideo.CreatedAt,
                    Name = blVideo.Name,
                    Description = blVideo.Description,
                    GenreId = blVideo.GenreId,
                    StreamingUrl = blVideo.StreamingUrl,
                    ImageId = blVideo.ImageId, 
                    Image = blVideo.Image,
                    TotalSeconds = GetSeconds(blVideo.TotalTime),                    
                };

                int GetSeconds(TimeSpan timeSpan)
                {
                    int seconds = (int)timeSpan.TotalSeconds;
                    return seconds;
                }
            }
            else
            {
                throw new ArgumentNullException("Video not found!");
            }
        }
        public static IEnumerable<BLVideo> MapToBl(IEnumerable<Video> videos) =>
           videos.Select(x => MapToBl(x));

        public static BLVideo MapToBl(Video dalVideo)
        {
           if(dalVideo != null)
            {
                try
                {
                    return new BLVideo
                    {
                        Id = dalVideo.Id,
                        CreatedAt = dalVideo.CreatedAt,
                        Name = dalVideo.Name,
                        Description = dalVideo.Description,
                        GenreId = dalVideo.GenreId,                                      
                        StreamingUrl = dalVideo.StreamingUrl,
                        ImageId = dalVideo?.ImageId,
                        Image = dalVideo?.Image,
                        Tags = dalVideo.VideoTags.Select(x => x.Tag.Name).ToList(),
                        TotalTime = GetTime(dalVideo.TotalSeconds)
                    };

                    TimeSpan GetTime(int seconds)
                    {
                        TimeSpan time = TimeSpan.FromSeconds(seconds);
                        return time;
                    }
                }
                catch
                {
                    throw new Exception("Video not found!");
                }

            }
            else
            {
                throw new ArgumentNullException("Video not found!");
            }
        }
    }
}
