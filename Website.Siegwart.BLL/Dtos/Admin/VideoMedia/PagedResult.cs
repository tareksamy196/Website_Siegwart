using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Website.Siegwart.BLL.Dtos.Admin.VideoGallery
{
    internal class PagedResult<T>
    {
        // Generic paged result helper
       
            public int TotalItems { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public T[] Items { get; set; } = Array.Empty<T>();
        
    }
}
