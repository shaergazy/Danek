using System;

namespace Danek.Web.Models.Catalog
{
    public class BookDetailsViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = null!;

        public string Author { get; set; } = null!;

        public string? Description { get; set; }

        public string Intro { get; set; } = null!;

        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
