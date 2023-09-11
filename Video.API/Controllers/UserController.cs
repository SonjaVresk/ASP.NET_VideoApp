
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using VideoApp.BL.BLModels;
using VideoApp.BL.DALModels;
using VideoApp.BL.Repositories;

namespace Video.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailSendRepository _emailSendRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly RwaMoviesContext _dbContext;
        //private readonly UserManager<IdentityUser> _userManager;


        public UserController(IUserRepository userRepository, ILogger<UserController> logger, IEmailSendRepository emailSendRepository, INotificationRepository notificationRepository,RwaMoviesContext dbContext)
        {
            _userRepository = userRepository;
            _logger = logger;
            _emailSendRepository = emailSendRepository;
            _notificationRepository = notificationRepository;
            _dbContext = dbContext;
            //_userManager = userManager;
        }


        [HttpGet]
        public ActionResult<IEnumerable<BLUser>> Get()
        {
            var blUsers = _userRepository.GetAll();

            return Ok(blUsers);
        }


        [HttpPost("[action]")]
        public ActionResult<User> Register([FromBody] UserRegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var newUser = _userRepository.Add(request);        

                var token = newUser.SecurityToken;
                var confirmationLink = Url.Action(nameof(ValidateEmailAPI), "User", new { email = newUser.Email, SecurityToken = token }, Request.Scheme);
                var notification = new BLNotification(new string[] { newUser.Email }, "VideoApp confirmation email link", confirmationLink);

                notification.ReceiverEmail = newUser.Email;

                _notificationRepository.CreateNotification(notification);

                _dbContext.SaveChanges();                                

                return Ok(new UserRegisterResponse
                {
                    Id = newUser.Id,
                    SecurityToken = newUser.SecurityToken
                   
                });                              
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<string> ValidateEmailAPI(string email, string token)
        {
            try
            {
                _userRepository.ValidateEmailAPI(email, token);

                return Ok("Verification success");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("[action]")]
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

        [HttpPost("[action]")]
        public ActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                _userRepository.ChangePassword(request);
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
