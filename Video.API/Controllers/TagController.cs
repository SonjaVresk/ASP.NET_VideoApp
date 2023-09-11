using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Video.API.APIModels;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;

namespace Video.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagController(RwaMoviesContext dbContext, ITagRepository tagRepository, IMapper mapper)
        {
            _dbContext = dbContext;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<APITag>> GetAll()
        {
            try
            {
                var blTags = _tagRepository.GetTags();

                if (blTags == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
                {
                    return NotFound("Tag not found!");
                }

                var apiTags = Video.API.Mapping.TagMapper.MapToAPI(blTags);

                return Ok(apiTags);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<APITag> Get(int id)
        {
            try
                
            {
                var blTag = _tagRepository.GetTag(id);

                //Validacija:
                if (blTag == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
                {
                    return NotFound("Tag not found!");
                }

                var apiTag = Video.API.Mapping.TagMapper.MapToAPI(blTag);

                return Ok(apiTag);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<APITag> Post([FromBody] APITag apiTag)
        {
            try
            {
                var blTag = Video.API.Mapping.TagMapper.MapToBl(apiTag);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var dalTag = TagMapper.MapToDal(blTag);

                _dbContext.Tags.Add(dalTag);

                _dbContext.SaveChanges();

                blTag = TagMapper.MapToBl(dalTag);

                apiTag = Video.API.Mapping.TagMapper.MapToAPI(blTag);

                return (apiTag);
            }
            catch (Exception ex)
            {
                return StatusCode(
                    StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]

        public ActionResult<APITag> Modify(int id, APITag apiTag)
        {
            try
            {
                var blTag = Video.API.Mapping.TagMapper.MapToBl(apiTag);

                //Validacija:
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  // 400 Bad Request
                }

                var dalTags = _dbContext.Tags.FirstOrDefault(x => x.Id == id);

                if (dalTags == null) //Provjera ima li videa u bazi, vraća 404 Not Found
                {
                    return NotFound();
                }

                dalTags.Name = blTag.Name;

                _dbContext.SaveChanges();

                blTag = TagMapper.MapToBl(dalTags);

                apiTag = Video.API.Mapping.TagMapper.MapToAPI(blTag);

                return Ok(apiTag); // Status Ok
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem u izmjeni podataka");
            }
        }


        [HttpDelete("{id}")]
        public ActionResult<APITag> Delete(int id)
        {
            try
            {
                var dalTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);

                if (dalTag == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
                {
                    return NotFound();
                }
                _dbContext.Tags.Remove(dalTag);

                var blTag = TagMapper.MapToBl(dalTag);

                _dbContext.SaveChanges();

                var apiTag = Video.API.Mapping.TagMapper.MapToAPI(blTag);

                return Ok(apiTag);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem u brisanju podataka");
            }
        }
    }
}
