using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;

namespace VideoApp.BL.Repositories
{
    public interface IGenreRepository
    {
        IEnumerable<BLGenre> GetGenres();
        (IEnumerable<BLGenre>, int) GetPagedGenres(int page, int size, string orderBy, string direction);
        BLGenre GetGenre(int id);
        int GetTotal();
    }
}
