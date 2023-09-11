using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class VideoTagMapper
    {
        public static IEnumerable<VideoTag> MapToDal(IEnumerable<BLVideoTag> blVideoTag) =>
            blVideoTag.Select(x => MapToDal(x));

        public static VideoTag MapToDal(BLVideoTag blVideoTag)
        {
            return new VideoTag
            {
                Id = blVideoTag.Id,
                VideoId = blVideoTag.VideoId,
                TagId = blVideoTag.TagId
            };
        }

        public static IEnumerable<BLVideoTag> MapToBl(IEnumerable<VideoTag> dalVideoTag) =>
          dalVideoTag.Select(x => MapToBl(x));

        public static BLVideoTag MapToBl(VideoTag dalVideoTag)
        {
            return new BLVideoTag
            {
                Id = dalVideoTag.Id,
                VideoId = dalVideoTag.VideoId,
                TagId = dalVideoTag.TagId
            };
        }
    }
}
