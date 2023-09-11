using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;

namespace VideoApp.BL.Mapping
{
    public static class GenreMapper
    {
        public static IEnumerable<Genre> MapToDal(IEnumerable<BLGenre> blGenre) =>
          blGenre.Select(x => MapToDal(x));

        public static Genre MapToDal(BLGenre blGenre)
        {
            return new Genre
            {
                Id = blGenre.Id,
                Name = blGenre.Name,
                Description = blGenre.Description
            };
        }

        public static IEnumerable<BLGenre> MapToBl(IEnumerable<Genre> genre) =>
          genre.Select(x => MapToBl(x));

        public static BLGenre MapToBl(Genre dalGenre)
        {
            return new BLGenre
            {
                Id = dalGenre.Id,
                Name = dalGenre.Name,
                Description = dalGenre.Description,
                Videos = dalGenre.Videos.Select(x => x.Name).ToList()
            };
        }
    }
}
