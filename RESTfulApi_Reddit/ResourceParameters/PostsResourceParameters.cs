using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTfulApi_Reddit.ResourceParameters {
    public class PostsResourceParameters {
        public int PageNumber { get; set; } = 1;

        const int maxPageSize = 20;

        public int _pageSize = 10;

        public int PageSize {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        public string SearchQuery { get; set; }

        public string Fields { get; set; }
    }
}