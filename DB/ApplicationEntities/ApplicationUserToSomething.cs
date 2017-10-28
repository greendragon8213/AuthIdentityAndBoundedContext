using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DB.ApplicationEntities
{
    public class ApplicationUserToSomething
    {
        [Key, Column(Order = 0)]
        [ForeignKey("ApplicationUser")]
        public int UserId { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Something")]
        public Guid SomethingId { get; set; }

        public virtual Something Something { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
