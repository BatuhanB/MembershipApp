using System.ComponentModel.DataAnnotations;

namespace MemberShip.Web.Areas.Admin.Models
{
    public class UpdateRoleViewModel
    {
        [Required(ErrorMessage ="This field can not be empty!")]
        public required string Name { get; set; }
        
        public required string Id { get; set; }
    }
}
