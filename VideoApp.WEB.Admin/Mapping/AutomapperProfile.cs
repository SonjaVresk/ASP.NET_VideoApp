using AutoMapper;

namespace VideoApp.WEB.Admin.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile() 
        {
            CreateMap<BL.BLModels.BLUser, ViewModels.VMUser>();
            CreateMap<ViewModels.VMUser, BL.BLModels.BLUser>();

            CreateMap<BL.BLModels.BLVideo, ViewModels.VMVideo>();
            CreateMap<ViewModels.VMVideo, BL.BLModels.BLVideo>();

            CreateMap<BL.BLModels.BLCountry, ViewModels.VMCountry>();
            CreateMap<ViewModels.VMCountry, BL.BLModels.BLCountry>();

            CreateMap<BL.BLModels.BLTag, ViewModels.VMTag>();
            CreateMap<ViewModels.VMTag, BL.BLModels.BLTag>();

            CreateMap<BL.BLModels.BLGenre, ViewModels.VMGenre>();
            CreateMap<ViewModels.VMGenre, BL.BLModels.BLGenre>();

            CreateMap<BL.BLModels.BLImage, ViewModels.VMImage>();
            CreateMap<ViewModels.VMImage, BL.BLModels.BLImage>();
        }
    }
}
