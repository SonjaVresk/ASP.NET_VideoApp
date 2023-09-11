using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;

namespace VideoApp.BL.Repositories
{
    public interface ICountryRepository
    {
        IEnumerable<BLCountry> GetCountries();
        (IEnumerable<BLCountry>, int) GetPagedCountries(int page, int size, string orderBy, string direction);
        int GetTotal();
    }
}
