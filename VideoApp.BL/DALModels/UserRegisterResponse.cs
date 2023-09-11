using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApp.BL.DALModels
{
    public class UserRegisterResponse
    {
        public int Id { get; set; }
        public string SecurityToken { get; set; }
    }
}
