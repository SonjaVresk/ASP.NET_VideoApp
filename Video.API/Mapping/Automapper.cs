using AutoMapper;

namespace Video.API.Mapping
{
    public class Automapper : Profile
    {
        public Automapper() {

            CreateMap<VideoApp.BL.BLModels.BLVideo, Video.API.APIModels.APIVideo>();
            CreateMap<Video.API.APIModels.APIVideo, VideoApp.BL.BLModels.BLVideo>();

            CreateMap<VideoApp.BL.BLModels.BLGenre, Video.API.APIModels.APIGenre>();
            CreateMap<Video.API.APIModels.APIGenre, VideoApp.BL.BLModels.BLGenre>();

            CreateMap<VideoApp.BL.BLModels.BLTag, Video.API.APIModels.APITag>();
            CreateMap<Video.API.APIModels.APITag, VideoApp.BL.BLModels.BLTag>();

            CreateMap<VideoApp.BL.BLModels.BLNotification, Video.API.APIModels.APINotification>();
            CreateMap<Video.API.APIModels.APINotification, VideoApp.BL.BLModels.BLNotification>();
        }
    }
}
