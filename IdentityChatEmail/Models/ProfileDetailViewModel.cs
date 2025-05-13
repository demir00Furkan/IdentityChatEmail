namespace IdentityChatEmail.Models
{
    public class ProfileDetailViewModel
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfileImageUrl { get; set; }
        public List<InboxMessageViewModel> InboxMessages { get; set; }
    }
}