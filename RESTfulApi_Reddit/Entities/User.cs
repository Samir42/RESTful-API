using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Entities {
    public class User : IdentityUser<int> {
        [Required]
        [MaxLength(15)]
        public string Name { get; set; }
        [Required]
        [MaxLength(15)]
        public string Surname { get; set; }
        public string About { get; set; }
        public string BannerUrl { get; set; }
        public string AvatarUrl { get; set; }
        public DateTimeOffset RegisteredAt { get; set; }
        public virtual  IEnumerable<CommunitiesUsers> CommunitiesUsers { get; set; }
    }
}
