using System.ComponentModel.DataAnnotations;

namespace BookStoreApp.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; }

        [Required, StringLength(200)]
        public string Author { get; set; }

        [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100000.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
    }
}


