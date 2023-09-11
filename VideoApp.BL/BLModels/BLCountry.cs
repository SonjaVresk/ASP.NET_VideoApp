using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApp.BL.BLModels
{
    public class BLCountry
    {
        public int Id { get; set; }

        public string Code { get; set; }

        [DisplayName("Country")]
        public string Name { get; set; }
    }
}
