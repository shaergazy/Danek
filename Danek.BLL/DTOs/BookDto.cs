using System.ComponentModel.DataAnnotations;

namespace Danek.BLL.DTOs
{
    public class BaseBookDto
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
    public class ListBookDto : BaseBookDto { }

    public class AddBookDto: BaseBookDto { }

    public class EditBookDto : BaseBookDto { }

    public class GetBookDto : BaseBookDto { }
}
