using System;

namespace Memorandum.Core.Domain
{
    public class User
    {
        public virtual int Id { get; set; }
        public virtual string Password { get; set; }
        public virtual DateTime LastLogin { get; set; }
        public virtual bool IsSuperUser { get; set; }
        public virtual string Username { get; set; }
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }
        public virtual string Email { get; set; }
        public virtual bool IsStaff { get; set; }
        public virtual bool IsActive { get; set; }
        public virtual DateTime DateJoined { get; set; }
    }
}
