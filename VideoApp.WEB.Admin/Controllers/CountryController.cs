using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;
using VideoApp.WEB.Admin.ViewModels;

namespace VideoApp.WEB.Admin.Controllers
{
    public class CountryController : Controller
    {        
        private readonly IMapper _mapper;
        private readonly ICountryRepository _countryRepository;

        public CountryController(IMapper mapper, ICountryRepository countryRepository)
        {            
            _mapper = mapper;
            _countryRepository = countryRepository;
        }

        public ActionResult Index(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                    size = 6;

                (var blCountries, var unpagedCount) = _countryRepository.GetPagedCountries(page, size, orderBy, direction);
                

                if (blCountries == null)
                {
                    return NotFound("Countries not found!");
                }

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = unpagedCount / size;

                var vmCountries = _mapper.Map<IEnumerable<VMCountry>>(blCountries);

                return View(vmCountries);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        public IActionResult CountryPartial(int page, int size, string orderBy, string direction)
        {
            try
            {
                if (size == 0)
                    size = 6;

                (var blCountries, var unpagedCount) = _countryRepository.GetPagedCountries(page, size, orderBy, direction);

                if (blCountries == null)
                {
                    return NotFound("Countries not found!");
                }

                ViewData["page"] = page;
                ViewData["size"] = size;
                ViewData["orderBy"] = orderBy;
                ViewData["direction"] = direction;
                ViewData["pages"] = unpagedCount / size;

                var vmCountries = _mapper.Map<IEnumerable<VMCountry>>(blCountries);

                return PartialView("_CountryPart", vmCountries);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
