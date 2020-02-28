using System;
using System.ComponentModel.DataAnnotations;

namespace RESTfulApi_Reddit.Entities {
    public class Post {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string Text { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
