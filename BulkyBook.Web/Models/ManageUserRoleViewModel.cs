namespace BulkyBook.Web.Models
{
    public class ManageUserRoleViewModel
    {
        public string UserRoleId { get; set; }

        public List<UserRoleViewModel> Roles { get; set; } 
    }
}
