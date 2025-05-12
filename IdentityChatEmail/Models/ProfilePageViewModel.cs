namespace IdentityChatEmail.Models
{
    public class ProfilePageViewModel
    {
        public List<InboxMessageViewModel> InboxMessages { get; set; }
        public UserUpdateViewModel User { get; set; }
    }
}
