using System;
using System.Collections.Generic;

namespace RESTfulApi_Reddit.Entities {
    public class Community {
        public DateTimeOffset CreatedAt { get; set; }
        public virtual IEnumerable<CommunitiesUsers> CommunitiesUsers { get; set; }
        public virtual IEnumerable<CommunitiesPosts> CommunitiesPosts { get; set; }
    }
}
