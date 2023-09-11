using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApp.BL.Mapping
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<DALModels.User, BLModels.BLUser>();
            CreateMap<DALModels.Video, BLModels.BLVideo>();
            CreateMap<DALModels.Genre, BLModels.BLGenre>();
            CreateMap<DALModels.Video, BLModels.BLVideo>();
            CreateMap<DALModels.Country, BLModels.BLCountry>();
            CreateMap<DALModels.Notification, BLModels.BLNotification>();
        }
    }
}
