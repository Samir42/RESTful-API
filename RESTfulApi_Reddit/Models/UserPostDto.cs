using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Models {
    public class UserPostDto {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
