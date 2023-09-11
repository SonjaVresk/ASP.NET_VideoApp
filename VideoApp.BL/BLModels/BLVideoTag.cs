using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApp.BL.BLModels
{
    public class BLVideoTag
    {
        public int Id { get; set; }

        public int VideoId { get; set; }

        public int TagId { get; set; }
    }
}
