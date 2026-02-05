using System;

namespace Danek.Web.Models.Catalog
{
    public class BookCardViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string Intro { get; set; } = null!;

        public bool InStock { get; set; }
    }
}
