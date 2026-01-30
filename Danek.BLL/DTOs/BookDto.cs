using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;

namespace Danek.BLL.DTOs
{
    public class BookDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Author { get; set; } = null!;

        [Required]
        [StringLength(500)]
        public string Intro { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
    }
}