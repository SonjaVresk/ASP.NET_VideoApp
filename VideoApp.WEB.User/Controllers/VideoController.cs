using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;
using VideoApp.WEB.User.ViewModels;

namespace VideoApp.WEB.User.Controllers
{
    [Authorize]
    public class VideoController : Controller
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IVideoRepository _videoRepository;
        private readonly IMapper _mapper;                

        public VideoController(RwaMoviesContext dbContext, IVideoRepository videoRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _videoRepository = videoRepository;
            _mapper = mapper;
        }

        public ActionResult Index(string filter, int page, int size, string orderBy, string? direction)
        {

            if (size == 0)
                size = 6;

            (var pagedVideos, var unpagedCount) = _videoRepository.GetPagedVideos(filter, page, size, orderBy, direction);

            var vmVideos = _mapper.Map<IEnumerable<VMVideo>>(pagedVideos);

            foreach (var vmVideo in vmVideos)
            {
                var dalImage = _dbContext.Images.FirstOrDefault(x => x.Id == vmVideo.ImageId);
                if (dalImage != null)
                {
                    vmVideo.ImageURl = dalImage.Content;
                }
            }

            ViewData["page"] = page;
            ViewData["size"] = size;
            ViewData["orderBy"] = orderBy;
            ViewData["direction"] = direction;
            ViewData["pages"] = (int)Math.Ceiling((double)unpagedCount / size);

            ViewData["videos"] = vmVideos;

            return View();
        }

        //public ActionResult GetFilteredVideos(string filter)
        //{
        //    var filteredVideos = _videoRepository.GetFilteredVideos(filter);

        //    var vmVideos = _mapper.Map<IEnumerable<BLVideo>>(filteredVideos);

        //    ViewData["page"] = filter;

        //    return PartialView("_VideoPartial", vmVideos);
        //}

        public ActionResult VideoPartial(string filter, int page, int size, string? orderBy, string? direction)
        {
            if (size == 0)
                size = 6;

            (var pagedVideos, var unpagedCount) = _videoRepository.GetPagedVideos(filter, page, size, orderBy, direction);

            var vmVideos = _mapper.Map<IEnumerable<VMVideo>>(pagedVideos);



            foreach (var vmVideo in vmVideos)
            {
                var dalImage = _dbContext.Images.FirstOrDefault(x => x.Id == vmVideo.ImageId);
                if (dalImage != null)
                {
                    vmVideo.ImageURl = dalImage.Content;
                }
            }
                        

            ViewData["filter"] = filter;
            ViewData["page"] = page;
            ViewData["size"] = size;
            ViewData["orderBy"] = orderBy;
            ViewData["direction"] = direction;
            ViewData["pages"] = (int)Math.Ceiling((double)unpagedCount / size);

            ViewData["videos"] = vmVideos;

            return PartialView("_VideoPartial");
        }



        // GET: VideoController/Details/5
        public ActionResult Details(int id)
        {
            
            var dalVideos = _dbContext.Videos.Include("Genre")
                                            .Include("VideoTags")
                                            .Include("VideoTags.Tag")
                                            .Include("Image")
                                            .FirstOrDefault(x => x.Id == id);

            //Validacija:
            if (dalVideos == null)
            {
                return NotFound("Video not found!");
            }

            var blVideo = VideoMapper.MapToBl(dalVideos);

            var vmVideos = _mapper.Map<VMVideo>(blVideo);


            // Naziv žanra:
            vmVideos.GenreName = dalVideos.Genre.Name;

            // Url slike:
            if (dalVideos.Image != null && dalVideos.Image.Content != null)
            {
                vmVideos.ImageURl = dalVideos.Image.Content;
            }

            return View("Details", vmVideos);
        }
    }
}
