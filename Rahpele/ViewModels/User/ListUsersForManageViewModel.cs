namespace Rahpele.ViewModels.User
{
    public class ListUsersForManageViewModel
    {
        public Guid Id { get; set; }
        public int RowId { get; set; }
        public string UserName { get; set; }
        public string AvatarThumbnail { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsPhoneNumberConfirmed { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public bool IsDeleted { get; set; }
        public string RegistrationDate { get; set; }
    }
}
