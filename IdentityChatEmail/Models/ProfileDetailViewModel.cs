namespace IdentityChatEmail.Models
{
    public class ProfileDetailViewModel
    {
        public List<InboxMessageViewModel> InboxMessages { get; set; }
        public UserUpdateViewModel User { get; set; }
    }

    public class UserUpdateViewModel2
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
