using System.ComponentModel.DataAnnotations;

namespace BookStore.Models
{
    public class BookViewModel
    {
        public int Id { get; set; }
        [Required( ErrorMessage ="Please enter the title of your book")]
        [StringLength(100,MinimumLength = 5)]
        public string Title { get; set; }

        [Required(ErrorMessage ="Please enter author name")]
        public string Author { get; set; }
        [StringLength(500, MinimumLength = 5)]
        [Required]
        public string Description { get; set; }
        [Required(ErrorMessage = "Please enter the category of your book")]
        [StringLength(100, MinimumLength = 5)]
        public string Category { get; set; }
        [Required]
        public int LanguageId { get; set; }
        public string Language { get; set; }
        [Required(ErrorMessage = "Please enter the Page Count of your book")]
        [Display(Name ="Page Count")]
        public int TotalPages { get; set; }

        //[Required]
        [Display(Name ="Cover Photo")]
        public IFormFile BookCover { get; set; }
        public string CoverImageUrl { get; set; }

        [Display(Name = "Book PDF")]
        public IFormFile BookPdf { get; set; }
        public string BookPdfUrl { get; set; }

        [Display(Name ="Gallery Images")]
        IFormFileCollection GalleryFiles { get; set; }
        public List<GalleryImageViewModel> Gallery { get; set; }

    }
}
