using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoApp.BL.BLModels
{
    public class BLNotification
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        public string ReceiverEmail { get; set; }
        public List<MailboxAddress> To { get; set; }

        public string? Subject { get; set; }

        public string Body { get; set; }

        public DateTime? SentAt { get; set; }

        public BLNotification()
        {
            
        }

        public BLNotification(IEnumerable<string> to, string subject, string content)
        {
            To = new List<MailboxAddress>();
            To.AddRange(to.Select(x => new MailboxAddress("email", x)));
            Subject = subject;
            Body = content;
        }
    }
}
