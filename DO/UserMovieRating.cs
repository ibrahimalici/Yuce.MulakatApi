using FluentNHibernate.Mapping;
using System;

namespace DO
{
    public class UserMovieRating
    {
        public virtual Guid UserMovieRatingPK { get; set; }
        public virtual Guid UserProfileFK { get; set; }
        public virtual Guid MovieFK { get; set; }
        public virtual double Rating { get; set; }
    }

    public class UserMovieRatingMappings : ClassMap<UserMovieRating>
    {
        #region Constructor
        public UserMovieRatingMappings()
        {
            Table("UserMovieRatings");
            Id(u => u.UserMovieRatingPK).GeneratedBy.Assigned();
            Map(u => u.UserProfileFK);
            Map(u => u.MovieFK);
            Map(u => u.Rating);
        }
        #endregion
    }
}
