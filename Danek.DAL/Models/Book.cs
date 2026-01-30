using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Danek.Web.Models
{
    public class Book
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(200)]
        [Display(Name = "Book Name")]
        public string Title { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        [Display(Name = "Author")]
        public string Author { get; set; } = null!;

        [Required]
        [MaxLength(500)]
        [Display(Name = "Short Intro")]
        [DataType(DataType.MultilineText)]
        public string Intro { get; set; } = null!;

        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be 0 or greater")]
        public int Quantity { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }



    //public class Book
    //{
    //    [Key]
    //    public Guid Id { get; set; }
    //    [Required]
    //    [MaxLength(200)]
    //    [DisplayName("Book Name")]
    //    public string Title { get; set; }

    //    [Required]
    //    [MaxLength(200)]
    //    public string Author { get; set; }

    //    [Required]
    //    [MaxLength(500)]
    //    public string Intro { get; set; }

    //    public string? Description { get; set; }

    //    [Range(0, int.MaxValue)]
    //    public int Quantity { get; set; }

    //    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    //}
}
