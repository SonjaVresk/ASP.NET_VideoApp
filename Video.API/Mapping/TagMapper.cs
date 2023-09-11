using Video.API.APIModels;
using VideoApp.BL.BLModels;

namespace Video.API.Mapping
{
    public static class TagMapper
    {
        public static IEnumerable<APITag> MapToAPI(IEnumerable<BLTag> blTag) =>
          blTag.Select(x => MapToAPI(x));

        public static APITag MapToAPI(BLTag blTag)
        {
            return new APITag
            {
                Name = blTag.Name                
            };
        }

        public static IEnumerable<BLTag> MapToBl(IEnumerable<APITag> apiTag) =>
          apiTag.Select(x => MapToBl(x));

        public static BLTag MapToBl(APITag apiTag)
        {
            return new BLTag
            {
                Name = apiTag.Name
            };
        }
    }
}
