using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoApp.BL.BLModels;

namespace VideoApp.BL.Repositories
{
    public interface ITagRepository
    {
        IEnumerable<BLTag> GetTags();
        BLTag GetTag(int id);
    }
}
