using Video.API.APIModels;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace Video.API.Mapping
{
    public static class GenreMapper
    {

        public static IEnumerable<APIGenre> MapToAPI(IEnumerable<BLGenre> blGenre) =>
          blGenre.Select(x => MapToAPI(x));

        public static APIGenre MapToAPI(BLGenre blGenre)
        {
            return new APIGenre
            {                
                Name = blGenre.Name,
                Description = blGenre.Description
            };
        }

        public static IEnumerable<BLGenre> MapToBl(IEnumerable<APIGenre> apiGenre) =>
          apiGenre.Select(x => MapToBl(x));

        public static BLGenre MapToBl(APIGenre apiGenre)
        {
            return new BLGenre
            {
                Name = apiGenre.Name,
                Description = apiGenre.Description
            };
        }
    }
}
