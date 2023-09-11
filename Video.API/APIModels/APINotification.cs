using MimeKit;

namespace Video.API.APIModels
{
    public class APINotification
    {
        public string ReceiverEmail { get; set; }        

        public string? Subject { get; set; }

        public string Body { get; set; }
    }
}
