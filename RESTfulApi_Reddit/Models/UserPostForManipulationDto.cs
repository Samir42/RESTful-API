using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.Models
{
    public abstract class UserPostForManipulationDto
    {
        [Required(ErrorMessage = "You should fill out a title")]
        [MaxLength(50, ErrorMessage = "The title shouldn't have more than 50 characters")]
        public string Title { get; set; }

        [MaxLength(200, ErrorMessage = "The text shouldn't have more than 200 characters")]
        public string Text { get; set; }
    }
}
