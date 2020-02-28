using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Entities {
    [Table("CommunitiesPosts")]
    public class CommunitiesPosts {
        public int Id { get; set; }
        [Required]
        public int CommunityId { get; set; }
        [Required]
        public int CommunityPostId { get; set; }
        public virtual Community Community { get; set; }
        public virtual CommunityPost   CommunityPost { get; set; }
    }
}
