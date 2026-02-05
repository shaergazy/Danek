using System;
using System.Collections.Generic;

namespace Danek.Web.Models.Catalog
{
    public class BookListViewModel
    {
        public string? Query { get; set; }

        public string? SelectedAuthor { get; set; }

        public bool? InStock { get; set; }

        public string SortBy { get; set; } = "title";

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 12;

        public int TotalItems { get; set; }

        public int TotalPages { get; set; }

        public List<string> Authors { get; set; } = new();

        public List<BookCardViewModel> Books { get; set; } = new();
    }
}
