using AutoMapper;
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
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IGenreRepository _genreRepository;
        private readonly IMapper _mapper;

        public GenreController(RwaMoviesContext dbContext, IGenreRepository genreRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _genreRepository = genreRepository;
            _mapper = mapper;
        }


        [HttpGet]
        public ActionResult<IEnumerable<APIGenre>> GetAll()
        {
            try
            {
                var blGenre = _genreRepository.GetGenres();

                if (blGenre == null)
                {
                    return NotFound("Genre not found!");
                }

                var apiGenres = API.Mapping.GenreMapper.MapToAPI(blGenre);

                return Ok(apiGenres);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<APIGenre> Get(int id)
        {
            try
            {                
                var blGenre = _genreRepository.GetGenre(id);

                //Validacija:
                if (blGenre == null)
                {
                    return NotFound("Genre not found!");
                }

                var apiGenre = Video.API.Mapping.GenreMapper.MapToAPI(blGenre);

                return Ok(apiGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<APIGenre> Post([FromBody] APIGenre apiGenre)
        {
            try
            {
                var blGenre = API.Mapping.GenreMapper.MapToBl(apiGenre);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var dalGenre = VideoApp.BL.Mapping.GenreMapper.MapToDal(blGenre);

                _dbContext.Genres.Add(dalGenre);

                _dbContext.SaveChanges();

                blGenre = VideoApp.BL.Mapping.GenreMapper.MapToBl(dalGenre);

                apiGenre = API.Mapping.GenreMapper.MapToAPI(blGenre);

                return (apiGenre);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]

        public ActionResult<APIGenre> Modify(int id, APIGenre apiGenre)
        {
            try
            {
                //Validacija:
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  // 400 Bad Request
                }

                var dalGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);

                if (dalGenre == null) //Provjera ima li videa u bazi, vraća 404 Not Found
                {
                    return NotFound();
                }

                var blGenre = Video.API.Mapping.GenreMapper.MapToBl(apiGenre);

                dalGenre.Name = blGenre.Name;
                dalGenre.Description = blGenre.Description;

                _dbContext.SaveChanges();

                blGenre = VideoApp.BL.Mapping.GenreMapper.MapToBl(dalGenre);

                apiGenre = Video.API.Mapping.GenreMapper.MapToAPI(blGenre);

                return Ok(apiGenre); // Status Ok
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem u izmjeni podataka");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<BLGenre> Delete(int id)
        {
            try
            {
                var dalGenre = _dbContext.Genres.FirstOrDefault(x => x.Id == id);

                if (dalGenre == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
                {
                    return NotFound();
                }

                var videos = _dbContext.Videos.Where(x => x.GenreId == id);

                //brisanje svih videa tog žanra:
                _dbContext.Videos.RemoveRange(videos);

                _dbContext.SaveChanges();

                //brisanje žanra:
                _dbContext.Genres.Remove(dalGenre);

                var genre = VideoApp.BL.Mapping.GenreMapper.MapToBl(dalGenre);

                _dbContext.SaveChanges();


                return Ok(genre);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem u brisanju podataka");
            }
        }
    }
}
