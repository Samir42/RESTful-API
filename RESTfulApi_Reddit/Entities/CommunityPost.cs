using System.Collections;
using System.Collections.Generic;

namespace RESTfulApi_Reddit.Entities {
    public class CommunityPost : Post {
        public int CommunityId { get; set; }
        public virtual Community Community { get; set; }

        public virtual IEnumerable<CommunitiesPosts> CommunitiesPosts { get; set; }
    }
}