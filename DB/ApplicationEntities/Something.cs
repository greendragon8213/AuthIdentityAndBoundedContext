using System;
using System.Collections.Generic;

namespace DB.ApplicationEntities
{
    public class Something
    {
        public Guid Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<ApplicationUserToSomething> ApplicationUserToSomethings { get; set; }
    }
}
