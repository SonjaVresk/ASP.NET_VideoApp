using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class ImageMapper
    {
        public static Image MapToDal(BLImage blImage)
        {
            return new Image
            {
                Id = blImage.Id,
                Content = blImage.Content
            };
        }

        public static BLImage MapToBl(Image dalImage)
        {
            return new BLImage
            {
                Id = dalImage.Id,
                Content = dalImage.Content
            };
        }
    }
}
