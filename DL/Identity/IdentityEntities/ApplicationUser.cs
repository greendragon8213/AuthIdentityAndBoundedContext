using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DL.Identity
{
    public class ApplicationUser
    {
        //One to one relation
        [Key, ForeignKey("CustomIdentityUser")]
        public int Id { get; set; }
        
        [StringLength(20)]
        public string FirstName { get; set; }
        
        [StringLength(20)]
        public string LastName { get; set; }
        
        [StringLength(1024)]
        public string UserImagePath { get; set; }

        public virtual CustomIdentityUser CustomIdentityUser { get; set; }

        //public virtual ICollection<ApplicationUserToSomething> ApplicationUserToSomethings { get; set; }
    }
}
