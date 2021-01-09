using FluentNHibernate.Mapping;
using System;

namespace DO
{
    public class UserProfile
    {
        public virtual Guid UserProfilePK { get; set; }
        public virtual string UserName { get; set; }
        public virtual string Password { get; set; }
        public virtual string Email { get; set; }
    }

    public class UserProfileMappings : ClassMap<UserProfile>
    {
        #region Constructor
        public UserProfileMappings()
        {
            Table("UserProfiles");
            Id(u => u.UserProfilePK).GeneratedBy.Assigned();
            Map(u => u.Email).Length(100);
            Map(u => u.UserName).Length(30);
            Map(u => u.Password).Length(10);
        }
        #endregion
    }
}
