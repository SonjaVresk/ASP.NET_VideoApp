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
    public class GenreRepository : IGenreRepository
    {
        private readonly RwaMoviesContext _dbContext;

        public GenreRepository(RwaMoviesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<BLGenre> GetGenres()
        {
            var dalGenres = _dbContext.Genres.Include("Videos");

            var blGenres = GenreMapper.MapToBl(dalGenres);

            return blGenres;
        }

        public BLGenre GetGenre(int id)
        {
            var dalGenre = _dbContext.Genres.Include("Videos").FirstOrDefault(x => x.Id == id);

            var blGenre = GenreMapper.MapToBl(dalGenre);

            return blGenre;
        }

        public (IEnumerable<BLGenre>, int) GetPagedGenres(int page, int size, string orderBy, string direction)
        {
            IEnumerable<Genre> dalGenre = _dbContext.Genres.Include("Videos").AsEnumerable();

            //sortiranje
            if (string.Compare(orderBy, "id", ignoreCase: true) == 0)
            {
                dalGenre = dalGenre.OrderBy(x => x.Id);
            }
            else if (string.Compare(orderBy, "name", ignoreCase: true) == 0)
            {
                dalGenre = dalGenre.OrderBy(x => x.Name);
            }
            else // default: order by Id
            {
                dalGenre = dalGenre.OrderBy(x => x.Id);
            }

            //desc
            if (string.Compare(direction, "asc", true) == 0)
            {
                dalGenre = dalGenre.Reverse();
            }

            var unpagedCount = dalGenre.Count();

            //page
            dalGenre = dalGenre.Skip(page * size).Take(size);

            var blGenres = GenreMapper.MapToBl(dalGenre);

            return (blGenres, unpagedCount);
        }

        public int GetTotal()
        {
            var genresCount = _dbContext.Genres.Count();

            return genresCount;
        }
    }
}
