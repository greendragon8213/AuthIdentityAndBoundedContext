using System.ComponentModel.DataAnnotations;

namespace BLL.Models
{
    public class UserForReadDTO
    {
        [StringLength(100)]
        public string PhoneNumber { get; set; }

        [StringLength(255)]
        public string Email { get; set; }
        
        [StringLength(20)]
        public string FirstName { get; set; }

        [StringLength(20)]
        public string LastName { get; set; }

        [StringLength(1024)]
        public string UserImagePath { get; set; }
    }
}
