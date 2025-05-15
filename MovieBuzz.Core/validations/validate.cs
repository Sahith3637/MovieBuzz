using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Core.validations
{
    public class GenreValidationAttribute : ValidationAttribute
    {
        public static class ValidGenres
        {
            public static readonly HashSet<string> Genres = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Action",
            "Adventure",
            "Animation",
            "Comedy",
            "Crime",
            "Documentary",
            "Drama",
            "Fantasy",
            "Horror",
            "Mystery",
            "Romance",
            "Sci-Fi",
            "Thriller",
            "Western",
            "Family",
            "Musical",
            "Biography",
            "History",
            "War",
            "Sport"
        };

            public static bool IsValidGenre(string genre)
            {
                if (string.IsNullOrWhiteSpace(genre)) return false;
                return Genres.Contains(genre.Trim());
            }
        }
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string genreString)
                return new ValidationResult("Genre must be a string.");

            var genres = genreString
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(g => g.Trim())
                .ToArray();

            if (genres.Length > 3)
                return new ValidationResult("Too many genres. Maximum allowed is 3.");

            var invalidGenres = genres
                .Where(g => !ValidGenres.IsValidGenre(g))
                .ToArray();

            if (invalidGenres.Any())
            {
                var invalidList = string.Join(", ", invalidGenres);
                var allowedList = string.Join(", ", ValidGenres.Genres);
                return new ValidationResult($"Invalid genre(s): {invalidList}. Allowed genres are: {allowedList}.");
            }

            return ValidationResult.Success;
        }
    }
}
