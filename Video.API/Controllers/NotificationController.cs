using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Security;
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
    public class NotificationController : ControllerBase
    {
        private readonly RwaMoviesContext _dbContext;
        private readonly INotificationRepository _notificationRepository;
        private readonly IEmailSendRepository _emailSendRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public NotificationController(RwaMoviesContext dbContext, INotificationRepository notificationRepository, IMapper mapper, IEmailSendRepository emailSendRepository, IUserRepository userRepository)
        {
            _dbContext = dbContext;
            _notificationRepository = notificationRepository;
            _emailSendRepository = emailSendRepository;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet("count")]
        public IActionResult GetUnsentNotificationCount()
        {
            int count = _notificationRepository.GetUnsentNotificationCount();

            return Ok(new { count });
        }

        [HttpGet("unsent")]
        public IActionResult GetUnsentNotifications()
        {
            var unsentNotifications = _notificationRepository.GetUnsentNotifications();

            // Return the unsent notifications as JSON
            return Ok(unsentNotifications);
        }

        [HttpGet]
        public ActionResult<IEnumerable<APINotification>> GetAll()
        {
            try
            {
                var blNotifications = _notificationRepository.GetNotifications();

                if (blNotifications == null)
                {
                    return NotFound("Notification not found!");
                }

                var apiNotifications = Video.API.Mapping.NotificationMapper.MapToAPI(blNotifications);

                return Ok(apiNotifications);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<APINotification> Get(int id)
        {
            try
            {
                var blNotification = _notificationRepository.GetNotification(id);

                //Validacija:
                if (blNotification == null)
                {
                    return NotFound("Notification not found!");
                }

                var apiNotification = Video.API.Mapping.NotificationMapper.MapToAPI(blNotification);

                return Ok(apiNotification);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<APINotification> Post([FromBody] APINotification apiNotification)
        {
            try
            {
                var blNotification = Video.API.Mapping.NotificationMapper.MapToBl(apiNotification);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var dalNotification = VideoApp.BL.Mapping.NotificationMapper.MapToDal(blNotification);

                _dbContext.Notifications.Add(dalNotification);

                _dbContext.SaveChanges();

                blNotification = _mapper.Map<BLNotification>(dalNotification);

                apiNotification = Video.API.Mapping.NotificationMapper.MapToAPI(blNotification);

                return (apiNotification);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]

        public ActionResult<APINotification> Modify(int id, APINotification apiNotification)
        {
            try
            {
                var blNotification = Video.API.Mapping.NotificationMapper.MapToBl(apiNotification);

                //Validacija:
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);  // 400 Bad Request
                }

                var dalNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);

                if (dalNotification == null)
                {
                    return NotFound();
                }


                dalNotification.ReceiverEmail = blNotification.ReceiverEmail;
                dalNotification.Subject = blNotification.Subject;
                dalNotification.Body = blNotification.Body;

                _dbContext.SaveChanges();

                blNotification = _mapper.Map<BLNotification>(dalNotification);

                apiNotification = Video.API.Mapping.NotificationMapper.MapToAPI(blNotification);

                return Ok(apiNotification); // Status Ok
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem u izmjeni podataka");
            }
        }

        [HttpDelete("{id}")]
        public ActionResult<APINotification> Delete(int id)
        {
            try
            {
                var dalNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);

                if (dalNotification == null)
                {
                    return NotFound();
                }
                _dbContext.Notifications.Remove(dalNotification);

                var blNotification = _mapper.Map<BLNotification>(dalNotification);

                _dbContext.SaveChanges();

                var apiNotification = Video.API.Mapping.NotificationMapper.MapToAPI(blNotification);

                return Ok(apiNotification);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Problem u brisanju podataka");
            }
        }

        [HttpPost("[action]")]
        public ActionResult<BLNotification> Send(int id)
        {
            var dalNotification = _dbContext.Notifications.FirstOrDefault(x => x.Id == id);

            if (dalNotification == null)
            {
                return NotFound("Notification not found!");
            }

            var body = dalNotification.Body;

            var newNotification = new BLNotification(new string[] { dalNotification.ReceiverEmail }, "VideoApp confirmation email link", body);

            _emailSendRepository.SendEmail(newNotification);

            dalNotification.SentAt = DateTime.UtcNow;

            _dbContext.SaveChanges();

            return Ok("Notification sent");
        }
    }
}
