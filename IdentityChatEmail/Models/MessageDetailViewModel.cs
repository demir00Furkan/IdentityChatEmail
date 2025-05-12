using IdentityChatEmail.Entities;

namespace IdentityChatEmail.Models
{
    public class MessageDetailViewModel
    {
        
        public Message Message { get; set; }
        public string SenderName { get; set; }
        public string SenderSurname { get; set; }
    }

}
