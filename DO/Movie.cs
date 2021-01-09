using FluentNHibernate.Mapping;
using System;

namespace DO
{
    public class Movie
    {
        public virtual Guid MoviePK { get; set; }
        public virtual string MovieName { get; set; }
        public virtual string Country { get; set; }
        public virtual int syncId { get; set; }
        public virtual string OverView { get; set; }
        public virtual DateTime ReleaseDate { get; set; }
    }

    public class MovieMappings : ClassMap<Movie>
    {
        #region Constructor
        public MovieMappings()
        {
            Table("Movies");
            Id(u => u.MoviePK).GeneratedBy.Assigned();
            Map(u => u.MovieName).Length(500);
            Map(u => u.Country).Length(10);
            Map(u => u.syncId);
            Map(u => u.OverView).Length(500);
            Map(u => u.ReleaseDate);
        }
        #endregion
    }
}
