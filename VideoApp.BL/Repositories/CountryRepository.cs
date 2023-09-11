using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;

namespace VideoApp.BL.Repositories
{
    public class CountryRepository :ICountryRepository
    {
        private readonly RwaMoviesContext _dbContext;
        public CountryRepository(RwaMoviesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<BLCountry> GetCountries()
        {
            var dalCountries = _dbContext.Countries;

            var bLCountries = CountryMapper.MapToBl(dalCountries);

            return bLCountries;
        }

        public (IEnumerable<BLCountry>, int) GetPagedCountries(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Country> dalCountry = _dbContext.Countries.AsEnumerable();

            //sortiranje
            if (string.Compare(orderBy, "id", ignoreCase: true) == 0)
            {
                dalCountry = dalCountry.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", ignoreCase: true) == 0)
            {
                dalCountry = dalCountry.OrderBy(x => x.Name);
            }
            else // default: order by Id
            {
                dalCountry = dalCountry.OrderBy(x => x.Id);
            }

            //desc
            if (string.Compare(direction, "asc", true) == 0)
            {
                dalCountry = dalCountry.Reverse();
            }

            var unpagedCount = dalCountry.Count();

            //page
            dalCountry = dalCountry.Skip(page * size).Take(size);

            var blCountries = CountryMapper.MapToBl(dalCountry);

            return (blCountries, unpagedCount);
        }

        public int GetTotal()
        {
            var countriesCount =  _dbContext.Countries.Count();

            return countriesCount;
        }
    }
}
