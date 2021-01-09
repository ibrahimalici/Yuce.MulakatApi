using FluentNHibernate.Mapping;
using System;

namespace DO
{
    public class UserMovieNote
    {
        public virtual Guid MovieNotePK { get; set; }
        public virtual Guid UserProfileFK { get; set; }
        public virtual Guid MovieFK { get; set; }
        public virtual string Note { get; set; }
    }

    public class UserMovieNoteMappings : ClassMap<UserMovieNote>
    {
        #region Constructor
        public UserMovieNoteMappings()
        {
            Table("UserMovieNotes");
            Id(u => u.MovieNotePK).GeneratedBy.Assigned();
            Map(u => u.UserProfileFK);
            Map(u => u.MovieFK);
            Map(u => u.Note).Length(250);
        }
        #endregion
    }
}
