using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DB.IdentityEntities;

namespace DB.ApplicationEntities
{
    public class ApplicationUser
    {
        //One to one relation
        [Key, ForeignKey("CustomIdentityUser")]
        public int Id { get; set; }

        //[Required]
        [StringLength(20)]
        public string FirstName { get; set; }

        //[Required]
        [StringLength(20)]
        public string LastName { get; set; }
        
        [StringLength(1024)]
        public string UserImagePath { get; set; }

        public virtual CustomIdentityUser CustomIdentityUser { get; set; }

        public virtual ICollection<ApplicationUserToSomething> ApplicationUserToSomethings { get; set; }
    }
}
