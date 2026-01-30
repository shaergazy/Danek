using System.ComponentModel.DataAnnotations;
using System;

namespace Danek.DAL.Models
{
    public class Book
    {
        [Key]
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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}