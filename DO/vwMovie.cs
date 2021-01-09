using System.Collections.Generic;

namespace DO
{
    public class vwMovie : Movie
    {
        public List<vwUserMovieNote> MovieNotes { get; set; } = new List<vwUserMovieNote>();

        public double? MovieRating { get; set; }

        public double? UserMovieRating { get; set; }
    }
}
