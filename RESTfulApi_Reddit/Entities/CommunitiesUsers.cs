using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Entities {
    [Table("CommunitiesUsers")]
    public class CommunitiesUsers {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CommunityId { get; set; }
        public virtual User User { get; set; }
        public virtual Community Community { get; set; }
    }
}
