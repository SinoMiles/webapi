using System.ComponentModel.DataAnnotations;
namespace UsingWebAPIWithEF6.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="标题不能为空")]
        public string Title { get; set; }
        [Required]
        public int Year { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public string Genre { get; set; }
        //Foreign Key
        public int AuthorId { get; set; }
        public Author Author { get; set; }
    }
}