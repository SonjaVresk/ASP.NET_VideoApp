using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Mapping;
using VideoApp.BL.Repositories;
using VideoApp.WEB.User.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace VideoApp.WEB.User.Controllers
{
    public class UserController : Controller
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailSendRepository _emailSendRepository;

        public UserController(IUserRepository userRepository, IMapper mapper, RwaMoviesContext dbContext, IEmailSendRepository emailSendRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _dbContext = dbContext;
            _emailSendRepository = emailSendRepository;
        }

        //public ActionResult Index()
        //{
        //    var blUsers = _userRepository.GetAll();
        //    var vmUsers = _mapper.Map<IEnumerable<VMUser>>(blUsers);

        //    return View(vmUsers);
        //}

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
            if (!ModelState.IsValid)
                return View(vMRegister);

            if (_userRepository.CheckUsernameExists(vMRegister.Username))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View(vMRegister);
            }

            //OTKOMENTIRATI:

            //if (_userRepository.CheckEmailExists(vMRegister.Email))
            //{
            //    ModelState.AddModelError("Email", "E-mail already exists");
            //    return View(vMRegister);
            //}

            var blUser = _userRepository.CreateUser(
                vMRegister.Username,
                vMRegister.FirstName,
                vMRegister.LastName,
                vMRegister.Email,
                vMRegister.Phone,
                vMRegister.Password,
                vMRegister.CountryId);


            //Email

            var token = blUser.SecurityToken;
            var confirmationLink = Url.Action(nameof(ValidateEmail), "User", new { email = blUser.Email, SecurityToken = token}, Request.Scheme);
            var message = new BLNotification(new string[] { blUser.Email }, "Confirmation email link", confirmationLink);
            _emailSendRepository.SendEmail(message);


            return RedirectToAction("UserRegistered");
        }

        public IActionResult UserRegistered()
        {
            return View();
        }

        public ActionResult ValidateEmail (VMValidateEmail validate)
        {
            if (!ModelState.IsValid)
                return View(validate);

            _userRepository.ConfirmEmail(validate.Email, validate.SecurityToken);

            return RedirectToAction("Login");
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(VMLogin login)
        {
            try
            {
                var blUser = _userRepository.GetConfirmedUser(login.Username, login.Password);

                if (blUser == null)
                {
                    ModelState.AddModelError("Username", "Invalid username or password");
                    return View(login);
                }

                var claims = new List<Claim> { new Claim(ClaimTypes.Name, blUser.Username) };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var properties = new AuthenticationProperties
                {
                    IsPersistent = login.StaySignedIn,
                };
                HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    properties).Wait();

                if (login.RedirectUrl != null)
                {
                    return Redirect(login.RedirectUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Video");
                }
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme).Wait();
                        

            return RedirectToAction("Login");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ChangePassword(VMChangePassword changePassword)
        {
            // Change user password, skip BL for simplicity
            _userRepository.ChangePassword(
                changePassword.Username,
                changePassword.NewPassword);

  

            return RedirectToAction("Index", "Video");
        }

        public ActionResult Profile(string username)
        {

            var dalUser = _dbContext.Users.Include("CountryOfResidence").FirstOrDefault(u => u.Username == username);

            //Validacija:
            if (dalUser == null) //Provjera ima li videa u bazi, vraća 404 Not Found, Delete može vratiti samo 404
            {
                return NotFound("User not found!");
            }

            var blUser = UserMapper.MapToBl(dalUser);

            blUser.CountryOfResidence = dalUser.CountryOfResidence.Name;
                       
            return View("Profile", blUser);
        }

        [HttpPost]
        public ActionResult<Tokens> JwtTokens([FromBody] JwtTokensRequest request)
        {
            try
            {
                return Ok(_userRepository.JwtTokens(request));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
