using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Video.API.APIModels;
using Video.API.Mapping;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;

namespace Video.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
         private readonly RwaMoviesContext _dbContext;
         private readonly IVideoRepository _videoRepository;
         private readonly ILogger<VideoController> _logger;
         private readonly IMapper _mapper;         


        public VideoController(RwaMoviesContext dbContext, ILogger<VideoController> logger, IVideoRepository videoRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _videoRepository = videoRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<APIVideo>> GetAll()
        {
            try
            {
                var blVideos = _videoRepository.GetVideos();

                if (blVideos == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
                {
                    return NotFound("Video not found!");
                }                                          

                var apiVideos = Video.API.Mapping.VideoMapper.MapToAPI(blVideos);

                return Ok(apiVideos);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<APIVideo> Get(int id)
        {
            try
            {
                _logger.LogInformation($"Dohvaćen video ID={id}"); //Ispis u konzoli informacije i dohvaćenom videu

                var blVideo = _videoRepository.GetVideo(id);

                //Validacija:
                if (blVideo == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
                {
                    return NotFound("Video not found!");
                }

                var apiVideo = Video.API.Mapping.VideoMapper.MapToAPI(blVideo);

                return Ok(apiVideo);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }


        [HttpGet("[action]")]
        public ActionResult<IEnumerable<APIVideo>> Search(string filter, string orderBy, string direction, int page, int size)
        {            
            try
            {
                // Cookie handling()
                var searchFilter = HttpContext.Request.Cookies["search.filter"];
                if (!string.IsNullOrEmpty(filter))
                {
                    // Add/update cookie value
                    HttpContext.Response.Cookies.Append("search.filter", filter);
                }
                else if (!string.IsNullOrEmpty(searchFilter))
                {
                    // Read value if exists
                    filter = searchFilter;
                }


                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //filtriranje
                var dalVideos = _dbContext.Videos.Include("Genre").Include("VideoTags").Include("VideoTags.Tag").AsEnumerable().Where(x =>
                           x.Name.Contains(filter, StringComparison.InvariantCultureIgnoreCase));

                var blVideos = VideoApp.BL.Mapping.VideoMapper.MapToBl(dalVideos);

                //sortiranje
                if (string.Compare(orderBy, "id", ignoreCase: true) == 0)
                {
                    blVideos = blVideos.OrderBy(x => x.Id);
                }
                else if (string.Compare(orderBy, "name", ignoreCase: true) == 0)
                {
                    blVideos = blVideos.OrderBy(x => x.Name);
                }
                else if (string.Compare(orderBy, "totaltime", ignoreCase: true) == 0)
                {
                    blVideos = blVideos.OrderBy(x => x.TotalTime);
                }
                else // default: order by Id
                {
                    blVideos = blVideos.OrderBy(x => x.Id);
                }

                //desc
                if (string.Compare(direction, "desc", true) == 0)
                {
                    blVideos = blVideos.Reverse();
                }

                //page
                blVideos = blVideos.Skip(page * size).Take(size);

                var apiVidoes = Video.API.Mapping.VideoMapper.MapToAPI(blVideos);

                return Ok(apiVidoes);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        public ActionResult<APIVideo> Post([FromBody] APIVideo apiVideo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var blVideo = Video.API.Mapping.VideoMapper.MapToBl(apiVideo);

                var dalVideo = VideoApp.BL.Mapping.VideoMapper.MapToDal(blVideo);

                dalVideo.VideoTags = blVideo.Tags.Select(tagName => new VideoTag
                {
                    VideoId = dalVideo.Id,
                    Tag = new Tag { Name = tagName }
                }).ToList();

                dalVideo.TotalSeconds = GetSeconds(blVideo.TotalTime);

                int GetSeconds(TimeSpan timeSpan)
                {
                    int seconds = (int)timeSpan.TotalSeconds;
                    return seconds;
                }

                var dbTags = _dbContext.Tags.Where(x => blVideo.Tags.Contains(x.Name));
                dalVideo.VideoTags = dbTags.Select(x => new VideoTag { Tag = x }).ToList();

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

                if (!string.IsNullOrEmpty(apiVideo.Image))
                {
                    var dalImage = new VideoApp.BL.DALModels.Image { Id = nextImageId, Content = apiVideo.Image };
                    dalVideo.Image = dalImage;
                }

                //Genre:
                var dalGenres = _dbContext.Genres;
                bool genreFound = false;

                foreach (var genre in dalGenres)
                {
                    if (genre.Name == apiVideo.Genre)
                    {
                        dalVideo.GenreId = genre.Id;
                        genreFound = true;
                        break;
                    }
                }

                if (!genreFound)
                {
                    var newGenre = new Genre
                    {
                        Name = apiVideo.Genre,
                        Description = apiVideo.Genre
                    };

                    _dbContext.Genres.Add(newGenre);
                    _dbContext.SaveChanges();

                    dalVideo.GenreId = newGenre.Id;
                }

                //TotalTime
                dalVideo.TotalSeconds = GetSeconds(blVideo.TotalTime);

                _dbContext.Videos.Add(dalVideo);

                _dbContext.SaveChanges();

                blVideo = VideoApp.BL.Mapping.VideoMapper.MapToBl(dalVideo);

                apiVideo = Video.API.Mapping.VideoMapper.MapToAPI(blVideo);

                apiVideo.Genre = dalVideo.Genre.Name;
                //apiVideo.Image = dalVideo.Image.Content;

                return (apiVideo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]

        public ActionResult<APIVideo> Modify(int id, APIVideo apiVideo)
        {
            try
            {
                var blVideo = Video.API.Mapping.VideoMapper.MapToBl(apiVideo);

                //Validacija:
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  // 400 Bad Request
                }

                var dalVideo = _dbContext.Videos.FirstOrDefault(x => x.Id == id);

                if (dalVideo == null) //Provjera ima li videa u bazi, vraća 404 Not Found
                {
                    return NotFound();
                }

                dalVideo.Name = apiVideo.Name;
                dalVideo.Description = apiVideo.Description;


                //Genre:
                var dalGenres = _dbContext.Genres;
                bool genreFound = false;

                foreach (var genre in dalGenres)
                {
                    if (genre.Name == apiVideo.Genre)
                    {
                        dalVideo.GenreId = genre.Id;
                        genreFound = true;
                        break;
                    }
                }

                if (!genreFound)
                {
                    var newGenre = new Genre
                    {
                        Name = apiVideo.Genre,
                        Description = apiVideo.Genre
                    };

                    _dbContext.Genres.Add(newGenre);
                    _dbContext.SaveChanges();

                    dalVideo.GenreId = newGenre.Id;
                }

                //dalVideo.Image.Content = apiVideo.Image;

                dalVideo.TotalSeconds = GetSeconds(blVideo.TotalTime);
                dalVideo.StreamingUrl = apiVideo.StreamingUrl;

                int GetSeconds(TimeSpan timeSpan)
                {
                    int seconds = (int)timeSpan.TotalSeconds;
                    return seconds;
                }

                // (1) Remove unused tags
                var toRemove = dalVideo.VideoTags.Where(x => !apiVideo.Tags.Contains(x.Tag.Name));
                foreach (var srTag in toRemove)
                {
                    _dbContext.VideoTags.Remove(srTag);
                }

                // (2) Add new tags
                var existingDbTagNames = dalVideo.VideoTags.Select(x => x.Tag.Name);
                var newTagNames = apiVideo.Tags.Except(existingDbTagNames);
                foreach (var newTagName in newTagNames)
                {
                    var dbTag = _dbContext.Tags.FirstOrDefault(x => newTagName == x.Name);
                    // What if the tag doesn't exist at all?
                    if (dbTag == null)
                        continue;

                    dalVideo.VideoTags.Add(new VideoTag
                    {
                        Video = dalVideo,
                        Tag = dbTag
                    });
                }

                _dbContext.SaveChanges();

                blVideo = VideoApp.BL.Mapping.VideoMapper.MapToBl(dalVideo);
                blVideo.Genre = apiVideo.Genre;                

                return Ok(blVideo); // Status Ok
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, "Problem u izmjeni podataka");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<APIVideo> Delete(int id)
        {
            try
            {
                var video = _dbContext.Videos.Include("VideoTags").FirstOrDefault(x => x.Id == id);

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

                return Ok(video);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem u brisanju podataka");
            }
        }
    }
}
