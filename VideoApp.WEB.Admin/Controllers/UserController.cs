using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;
using VideoApp.WEB.Admin.ViewModels;

namespace VideoApp.WEB.Admin.Controllers
{
    public class UserController : Controller
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly ILogger<VideoController> _logger;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;


        public UserController(RwaMoviesContext dbContext, ILogger<VideoController> logger, IMapper mapper, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _logger = logger;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        public ActionResult Index(string filterFirstName, string filterLastName, string filterUserName, string filterCountry, bool searchFirstName, bool searchLastName, bool searchUsername, bool searchCountry)
        {
            // Cookie: Filters
            var cookieFilters = Request.Cookies["filters"];
            var filters = new List<string>();

            if (!string.IsNullOrEmpty(cookieFilters))
            {
                filters = JsonConvert.DeserializeObject<List<string>>(cookieFilters);
            }

            if (!string.IsNullOrEmpty(filterFirstName))
            {
                filters.Add("filterFirstName=" + filterFirstName);
            }

            if (!string.IsNullOrEmpty(filterLastName))
            {
                filters.Add("filterLastName=" + filterLastName);
            }

            if (!string.IsNullOrEmpty(filterUserName))
            {
                filters.Add("filterUserName=" + filterUserName);
            }

            if (!string.IsNullOrEmpty(filterCountry))
            {
                filters.Add("filterCountry=" + filterCountry);
            }

            Response.Cookies.Append("filters", JsonConvert.SerializeObject(filters));



            ViewData["filterFirstName"] = filterFirstName;
            ViewData["filterLastName"] = filterLastName;
            ViewData["filterUserName"] = filterUserName;
            ViewData["filterCountry"] = filterCountry;

            var blUsers = _userRepository.GetUsers(filterFirstName, filterLastName, filterUserName, filterCountry);
            //var vmUsers = _mapper.Map<IEnumerable<VMUser>>(blUsers);

            return View(blUsers);
        }


        public ActionResult Register()
        {
            var vMRegister = new VMRegister();

            vMRegister.Countries = _dbContext.Countries.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToArray();

            return View(vMRegister);
        }

        [HttpPost]
        public ActionResult Register(VMRegister vMRegister)
        {
            _userRepository.CreateUser(
                vMRegister.Username,
                vMRegister.FirstName,
                vMRegister.LastName,
                vMRegister.Email,
                vMRegister.Phone,
                vMRegister.Password,
                vMRegister.CountryId                
                );

            return RedirectToAction("Index");
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var blUser = _userRepository.GetUser(id);

                return View(blUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }            
        }

        [HttpPost]
        public ActionResult Delete(BLUser user)
        {
            _userRepository.SoftDeleteUser(user.Id);
            return RedirectToAction("Index");
        }


        public ActionResult Details(int id)
        {

            var blUser = _userRepository.GetUser(id);

            //Validacija:
            if (blUser == null)
            {
                return NotFound("User not found!");
            }

            return View(blUser);
        }

        public ActionResult Edit(int id)
        {
            var blUser = _userRepository.GetUser(id);

            var mvUser = _mapper.Map<VMUser>(blUser);

            //Select lista za državu:
            mvUser.CountryList = _dbContext.Countries.Select(x => new SelectListItem
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToArray();

            //Validacija:
            if (mvUser == null) 
            {
                return NotFound("User not found!");
            }

            return View(mvUser);
        }


        [HttpPost]
        public ActionResult Edit(int id, VMUser vmUser)
        {
            // Validation
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dalUser = _userRepository.GetUser(id);

            var token = dalUser.SecurityToken;

            if (dalUser == null)
            {
                return NotFound();
            }

            var blUser = _mapper.Map<BLUser>(vmUser);

            dalUser.CreatedAt = blUser.CreatedAt;
            dalUser.Username = blUser.Username;
            dalUser.FirstName = blUser.FirstName;
            dalUser.LastName = blUser.LastName;
            dalUser.Email = blUser.Email;
            dalUser.Phone = blUser.Phone;
            dalUser.IsConfirmed = blUser.IsConfirmed;            
            dalUser.CountryOfResidenceId = blUser.CountryOfResidenceId;           

            _dbContext.SaveChanges();

            return RedirectToAction("Index");
        }

    }
}
