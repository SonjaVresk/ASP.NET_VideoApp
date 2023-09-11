using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;

namespace VideoApp.BL.Repositories
{
    public interface IVideoRepository
    {
        IEnumerable<BLVideo> GetVideos();
        BLVideo GetVideo(int id);

        (IEnumerable<BLVideo>, int) GetPagedVideos(string filter, int page, int size, string orderBy, string direction);

        (IEnumerable<BLVideo>, int) GetPagedVideosAdmin(string filterName, string filterGenre, int page, int size, string orderBy, string direction);

        int GetTotal();
        //string GetTime(int seconds);
    }
}
