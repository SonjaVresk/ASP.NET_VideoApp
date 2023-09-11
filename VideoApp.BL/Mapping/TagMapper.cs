using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class TagMapper
    {

        public static IEnumerable<Tag> MapToDal(IEnumerable<BLTag> blTag) =>
            blTag.Select(x => MapToDal(x));

        public static Tag MapToDal(BLTag blTag)
        {
            return new Tag
            {
                Id = blTag.Id,
                Name = blTag.Name
            };
        }

        public static IEnumerable<BLTag> MapToBl(IEnumerable<Tag> dalTag) =>
          dalTag.Select(x => MapToBl(x));

        public static BLTag MapToBl(Tag dalTag)
        {
            return new BLTag
            {
                Id = dalTag.Id,
                Name = dalTag.Name
            };
     
        }   
    }
}
