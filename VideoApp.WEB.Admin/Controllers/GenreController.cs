using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;
using VideoApp.WEB.Admin.ViewModels;

namespace VideoApp.WEB.Admin.Controllers
{
    public class GenreController : Controller
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IGenreRepository _genreRepository;

        public GenreController(IGenreRepository genreRepository, RwaMoviesContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _genreRepository = genreRepository;
        }

        // GET: GenreController
        public ActionResult Index(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                    size = 6;

                (var blGenres, var unpagedCount) = _genreRepository.GetPagedGenres(page, size, orderBy, direction);

                if (blGenres == null)
                {
                    return NotFound("Genres not found!");
                }

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = unpagedCount / size;

                return View(blGenres);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public ActionResult GenrePartial(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                    size = 6;

                (var blGenres, var unpagedCount) = _genreRepository.GetPagedGenres(page, size, orderBy, direction);

                if (blGenres == null)
                {
                    return NotFound("Genres not found!");
                }

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = unpagedCount / size;
                                

                return PartialView("_GenrePart", blGenres);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: GenreController/Details/5
        public ActionResult Details(int id)
        {
            var blGenre = _genreRepository.GetGenre(id);

            //Validacija:
            if (blGenre == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
            {
                return NotFound("Genre not found!");
            }

            return View(blGenre);
        }

        // GET: GenreController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: GenreController/Create
        [HttpPost]
        public ActionResult Create(VMGenre vmGenre)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var blGenre = _mapper.Map<BLGenre>(vmGenre);
                var dalGenre = GenreMapper.MapToDal(blGenre);
                                
                _dbContext.Genres.Add(dalGenre);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: GenreController/Edit/5
        public ActionResult Edit(int id)
        {
            var blGenres = _genreRepository.GetGenre(id);

            var vmGenre = _mapper.Map<VMGenre>(blGenres);

            if (blGenres == null)
            {
                return NotFound("Genres not found!");
            }

            return View(vmGenre);
        }

        // POST: GenreController/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, VMGenre vmGenre)
        {
            // Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request
            }

            var dalGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);

            if (dalGenre == null)
            {
                return NotFound(); // Video not found, return 404 Not Found
            }

            var blGenre = _mapper.Map<BLGenre>(vmGenre);

            dalGenre.Name = blGenre.Name;
            dalGenre.Description = blGenre.Description;

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: GenreController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var blGenre = _genreRepository.GetGenre(id);

                if (blGenre == null)
                {
                    return NotFound(); // Video not found, return 404 Not Found
                }

                return View(blGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: GenreController/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, BLGenre blGenre)
        {
            try
            {
                var dalGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);

                if (dalGenre == null)
                {
                    return NotFound();
                }

                var videos = _dbContext.Videos.Where(x => x.GenreId == id);

                foreach (var video in videos)
                {
                    var videoTags = _dbContext.VideoTags.Where(vt => vt.VideoId == video.Id);
                    var tagIds = videoTags.Select(vt => vt.TagId).ToList();
                    var tags = _dbContext.Tags.Where(t => tagIds.Contains(t.Id));
                    _dbContext.Tags.RemoveRange(tags);
                    _dbContext.VideoTags.RemoveRange(videoTags);
                }

                _dbContext.Videos.RemoveRange(videos);
                _dbContext.Genres.Remove(dalGenre);

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
