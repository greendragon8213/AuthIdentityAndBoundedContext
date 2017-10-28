using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class UserPermissionDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string UserName { get; set; }
        
        public bool IsDeleted { get; set; }

        public List<RoleDTO> Roles { get; set; }
    }
}
