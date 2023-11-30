namespace MemberShip.Web.Areas.Admin.Models
{
    public class AssignRoleViewModel
    {
        public required string RoleId { get; set; }
        public required string Name { get; set; }
        public bool Exists { get; set; }
    }
}
