using VideoApp.BL.BLModels;

namespace VideoApp.WEB.Admin.ViewModels
{
    public class VMVideoSearch
    {
        public List<BLVideo> Videos { get; set; }
        public string SearchString { get; set; }
    }
}
