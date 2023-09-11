using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class CountryMapper
    {
        public static IEnumerable<Country> MapToDal(IEnumerable<BLCountry> blCountry) =>
          blCountry.Select(x => MapToDal(x));

        public static Country MapToDal(BLCountry blCountry)
        {
            return new Country
            {
                Id = blCountry.Id,
                Code = blCountry.Code,
                Name = blCountry.Name
            };
        }

        public static IEnumerable<BLCountry> MapToBl(IEnumerable<Country> country) =>
         country.Select(x => MapToBl(x));


        public static BLCountry MapToBl(Country dalCountry)
        {
            return new BLCountry
            {
                Id = dalCountry.Id,
                Code = dalCountry.Code,
                Name = dalCountry.Name
            };
        }
    }
}
