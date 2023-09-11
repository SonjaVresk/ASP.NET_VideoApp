using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using System.Drawing;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;
using VideoApp.WEB.Admin.ViewModels;

namespace VideoApp.WEB.Admin.Controllers
{
    public class VideoController : Controller
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IVideoRepository _videoRepository;
        private readonly ILogger<VideoController> _logger;
        private readonly IMapper _mapper;

        public VideoController(RwaMoviesContext dbContext, IVideoRepository videoRepository, ILogger<VideoController> logger, IMapper mapper)
        {
            _dbContext = dbContext;
            _videoRepository = videoRepository;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: VideoController
        public IActionResult Index(string filterName, string filterGenre, bool searchName, bool searchGenre, int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                    size = 6;

                //Cookie:
                var cookieFilterName = Request.Cookies["filerName"];
                if (searchName == false && string.IsNullOrEmpty(filterName))
                    filterName = cookieFilterName;
                else if (searchName == true && filterName != null)
                    Response.Cookies.Append("filerName", filterName);
                else if(searchName == true && filterName == null)
                    Response.Cookies.Delete("filerName");

                var cookieFilterGenre = Request.Cookies["filterGenre"];
                if (searchGenre == false && string.IsNullOrEmpty(filterGenre))
                    filterGenre = cookieFilterGenre;
                else if (searchGenre == true && filterGenre != null)
                    Response.Cookies.Append("filterGenre", filterGenre);
                else if (searchGenre == true && filterGenre == null)
                    Response.Cookies.Delete("filterGenre");

                (var blVideos, var unpagedCount) = _videoRepository.GetPagedVideosAdmin(filterName, filterGenre, page, size, orderBy, direction);

                if (blVideos == null)
                {
                    return NotFound("Video not found!");
                }
                ViewData["filterName"] = filterName;
                ViewData["filterGenre"] = filterGenre;
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = (int)Math.Ceiling((double)unpagedCount / size);

                //ViewData["videos"] = blVideos;

                return View(blVideos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public IActionResult VideoPartial(string filterName, string filterGenre, int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                    size = 6;

                (var blVideos, var unpagedCount) = _videoRepository.GetPagedVideosAdmin(filterName, filterGenre, page, size, orderBy, direction);

                if (blVideos == null)
                {
                    return NotFound("Videos not found!");
                }

                ViewData["filterName"] = filterName;
                ViewData["filterGenre"] = filterGenre;
                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = (int)Math.Ceiling((double)unpagedCount / size);

                //ViewData["videos"] = blVideos;

                return PartialView("_VideoPart", blVideos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: VideoController/Details/5
        public ActionResult Details(int id)
        {
            _logger.LogInformation($"Dohvaćen video ID={id}"); //Ispis u konzoli informacije i dohvaćenom videu

            var dalVideo = _dbContext.Videos.Include("Genre")
                                            .Include("VideoTags")
                                            .Include("VideoTags.Tag")
                                            .Include("Image")
                                            .FirstOrDefault(x => x.Id == id);

            //Validacija:
            if (dalVideo == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
            {
                return NotFound("Video not found!");
            }

            var blVideo = VideoMapper.MapToBl(dalVideo);

            blVideo.Genre = dalVideo.Genre.Name;

            return View(blVideo);
        }

        // GET: VideoController/Create
        public IActionResult Create()
        {
            var mvVideo = new VMVideo();

            mvVideo.GenreList = _dbContext.Genres.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToArray();

            //mvVideo.ImageList = _dbContext.Images.Select(x => new SelectListItem
            //{
            //    Text = x.Content,
            //    Value = x.Id.ToString()
            //}).ToArray();

            return View(mvVideo);
        }

        // POST: VideoController/Create
        [HttpPost]
        public IActionResult Create(VMVideo vmVideo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }            

                var blVideo = _mapper.Map<BLVideo>(vmVideo);
                var dalVideo = VideoMapper.MapToDal(blVideo);

                //Spremanje tagova u tablicu Tag u bazi:
                foreach (var tagName in vmVideo.Tags)
                {
                    var tag = _dbContext.Tags.FirstOrDefault(x => x.Name == tagName);
                    if (tag == null)
                    {
                        tag = new Tag { Name = tagName };
                        _dbContext.Tags.Add(tag);
                    }
                    dalVideo.VideoTags.Add(new VideoTag { Tag = tag });
                }

                dalVideo.GenreId = blVideo.GenreId;

                //Image:
                var lastImage = _dbContext.Images.OrderByDescending(i => i.Id).FirstOrDefault();
                int nextImageId;

                if (lastImage == null)
                {
                    nextImageId = 1;
                }
                else
                {
                    var lastImageId = lastImage.Id;
                    nextImageId = lastImageId + 1;
                }

                if (!string.IsNullOrEmpty(vmVideo.ImageURl))
                {
                    var dalImage = new BL.DALModels.Image { Id = nextImageId, Content = vmVideo.ImageURl };
                    dalVideo.Image = dalImage;
                }

                _dbContext.Videos.Add(dalVideo);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: VideoController/Edit/5
        public IActionResult Edit(int id)
        {
            var blVideo = _videoRepository.GetVideo(id);

            var mvVideo = _mapper.Map<VMVideo>(blVideo);

            //Select lista za žanr:
            mvVideo.GenreList = _dbContext.Genres.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToArray();

            if(mvVideo.ImageURl == null)
            {
                mvVideo.ImageURl = blVideo.Image.Content;

            }

            //Select lista za sliku:
            //mvVideo.ImageList = _dbContext.Images.Select(x => new SelectListItem
            //{
            //    Text = x.Content,
            //    Value = x.Id.ToString()
            //}).ToArray();


            //Validacija:
            if (mvVideo == null)
            {
                return NotFound("Video not found!");
            }
            
            return View(mvVideo);
        }

        // POST: VideoController/Edit/5
        [HttpPost]
        public IActionResult Edit(int id, VMVideo vmVideo)
        {
            // Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request
            }

            var dalVideo = _dbContext.Videos.Include(v => v.VideoTags)
                                            .ThenInclude(vt => vt.Tag)
                                            .FirstOrDefault(x => x.Id == id);

            if (dalVideo == null)
            {
                return NotFound(); // Video not found, return 404 Not Found
            }

            //Image:
            var images = _dbContext.Images;

            var dbImage = _dbContext.Images.Any(x => x.Content == vmVideo.ImageURl);
            if (dbImage != null)
            {
                var lastImage = _dbContext.Images.OrderByDescending(i => i.Id).FirstOrDefault();
                int nextImageId;

                if (lastImage == null)
                {
                    nextImageId = 1;
                }
                else
                {
                    var lastImageId = lastImage.Id;
                    nextImageId = lastImageId + 1;
                }

                if (!string.IsNullOrEmpty(vmVideo.ImageURl))
                {
                    var dalImage = new BL.DALModels.Image { Id = nextImageId, Content = vmVideo.ImageURl };
                    dalVideo.Image = dalImage;
                    _dbContext.Images.Add(dalImage);
                    _dbContext.SaveChanges();
                }
            }
            else
            {
                foreach (var image in images)
                {
                    if(image.Content == vmVideo.ImageURl){
                        dalVideo.ImageId = image.Id;
                    }
                }
            }


            var blVideo = _mapper.Map<BLVideo>(vmVideo);

            dalVideo.Name = blVideo.Name;
            dalVideo.Description = blVideo.Description;
            dalVideo.GenreId = blVideo.GenreId;
            dalVideo.TotalSeconds = GetSeconds(blVideo.TotalTime);
            dalVideo.StreamingUrl = blVideo.StreamingUrl;
            //dalVideo.ImageId = blVideo.ImageId;
            //dalVideo.Image = blVideo.Image;


            int GetSeconds(TimeSpan timeSpan)
            {
                int seconds = (int)timeSpan.TotalSeconds;
                return seconds;
            }

            // Remove old tags
            var tagsToRemove = dalVideo.VideoTags.Where(vt => !vmVideo.Tags.Contains(vt.Tag.Name)).ToList();
            foreach (var tagToRemove in tagsToRemove)
            {
                _dbContext.VideoTags.Remove(tagToRemove);
            }

            // Add new tags
            var existingTagNames = dalVideo.VideoTags.Select(vt => vt.Tag.Name);
            var newTagNames = vmVideo.Tags.Except(existingTagNames);
            foreach (var newTagName in newTagNames)
            {
                var dbTag = _dbContext.Tags.FirstOrDefault(t => t.Name == newTagName);
                if (dbTag == null)
                {
                    dbTag = new Tag { Name = newTagName };
                    _dbContext.Tags.Add(dbTag);
                }

                dalVideo.VideoTags.Add(new VideoTag
                {
                    Video = dalVideo,
                    Tag = dbTag
                });
            }

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            try
            {
                var blVideo = _videoRepository.GetVideo(id);

                if (blVideo == null)
                {
                    return NotFound(); // Video not found, return 404 Not Found
                }

                return View(blVideo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: Video/Delete/5
        [HttpPost]
        public IActionResult Delete(BLVideo bLVideo)
        {
            try
            {
                var video = _dbContext.Videos.Include("VideoTags").FirstOrDefault(x => x.Id == bLVideo.Id);

                if (video == null)
                {
                    return NotFound(); // Video not found, return 404 Not Found
                }

                // Remove VideoTags
                foreach (var videoTag in video.VideoTags.ToList())
                {
                    _dbContext.VideoTags.Remove(videoTag);
                }

                // Remove video
                _dbContext.Videos.Remove(video);

                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
