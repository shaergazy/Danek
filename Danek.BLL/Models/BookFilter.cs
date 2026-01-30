using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danek.BLL.Models
{
    public class BookFilter
    {
        public string? TitlePart { get; set; }
        public string? Author { get; set; }
        // null = both, true = in stock (>0), false = out of stock (==0)
        public bool? InStock { get; set; }
    }
}
