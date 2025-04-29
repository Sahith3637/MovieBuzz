using MovieBuzz.Core.Dtos.Movies;


namespace MovieBuzz.Core.Dtos.Shows
{
    public class ShowResponseDto : ShowDto
    {
        public int ShowId { get; set; }
        //public string MovieName { get; set; }
        //public required MovieSummaryDto Movie { get; set; }
    }
}
