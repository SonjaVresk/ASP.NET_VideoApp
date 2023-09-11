using AutoMapper;
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
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;    
        private readonly RwaMoviesContext _dbContext;

        public TagController(ITagRepository tagRepository, IMapper mapper, RwaMoviesContext dbContext)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        // GET: TagController
        public ActionResult Index()
        {
            try
            {
                var blTags = _tagRepository.GetTags();

                if (blTags == null)
                {
                    return NotFound("Tag not found!");
                }

                return View(blTags);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        // GET: VideoController/Details/5
        public ActionResult Details(int id)
        {
            var blTag = _tagRepository.GetTag(id);

            //Validacija:
            if (blTag == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
            {
                return NotFound("Tag not found!");
            }

            return View(blTag);
        }

        // GET: TagController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TagController/Create
        [HttpPost]
        public ActionResult Create(VMTag vmTag)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var blTag = _mapper.Map<BLTag>(vmTag);
                var dalTag = TagMapper.MapToDal(blTag);
                            

                _dbContext.Tags.Add(dalTag);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // GET: TagController/Edit/5
        public ActionResult Edit(int id)
        {
            var blTag = _tagRepository.GetTag(id);
            var vmTag = _mapper.Map<VMTag>(blTag);

            return View(vmTag);
        }

        // POST: TagController/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, VMTag vmTag)
        {
            // Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  // 400 Bad Request
            }

            var dalTag = _dbContext.Tags.FirstOrDefault(x => x.Id == id);

            if (dalTag == null)
            {
                return NotFound(); // Video not found, return 404 Not Found
            }

            dalTag.Name = vmTag.Name;

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: TagController/Delete/5
        public ActionResult Delete(int id)
        {
            try
            {
                var blTag = _tagRepository.GetTag(id);

                return View(blTag);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // POST: TagController/Delete/5
        [HttpPost]
        public ActionResult Delete(BLTag blTag)
        {
            try
            {
                var dalTag = _dbContext.Tags.Include(tag => tag.VideoTags)
                                            .FirstOrDefault(x => x.Id == blTag.Id);

                if (dalTag == null)
                {
                    return NotFound(); // Tag not found, return 404 Not Found
                }

                // Remove associated VideoTag records
                _dbContext.VideoTags.RemoveRange(dalTag.VideoTags);

                // Remove the tag
                _dbContext.Tags.Remove(dalTag);

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
