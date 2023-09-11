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
    public class TagRepository : ITagRepository
    {
        private readonly RwaMoviesContext _dbContext;

        public TagRepository(RwaMoviesContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<BLTag> GetTags()
        {
            try
            {
                var dalTags = _dbContext.Tags;

                var blTags = TagMapper.MapToBl(dalTags);

                return blTags;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public BLTag GetTag(int id)
        {

            try
            {
                var dalTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);

                var blTag = TagMapper.MapToBl(dalTag);

                return blTag;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

    }
}
