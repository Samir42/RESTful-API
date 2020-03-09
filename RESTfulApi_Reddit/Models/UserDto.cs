using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string About { get; set; }
    }
}
