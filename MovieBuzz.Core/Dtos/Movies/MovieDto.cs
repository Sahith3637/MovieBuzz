using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.Dtos.Movies
{
    public class MovieDto
    {
        [Required(ErrorMessage = "Movie name is required")]
        [StringLength(100, ErrorMessage = "Movie name cannot be longer than 100 characters")]
        public required string MovieName { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [StringLength(50, ErrorMessage = "Genre cannot be longer than 50 characters")]
        public required string Genre { get; set; }

        [Required(ErrorMessage = "Age restriction is required")]
        [Range(0, 22, ErrorMessage = "Age restriction must be between 0 and 18")]
        public required int AgeRestriction { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(60, 200, ErrorMessage = "Duration must be between 60 and 200 minutes")]
        public required int Duration { get; set; }

        public required string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0, 1000, ErrorMessage = "Price must be between 0.01 and 1000")]
        public decimal Price { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public required string PosterImageUrl { get; set; }

        [Url(ErrorMessage = "Invalid URL format")]
        public required string TrailerUrl { get; set; }
    }

}
