namespace Memorandum.Core.Domain.Users
{
    public class Guest : User
    {
        public Guest(string ip) : base(new UserInfo() { Name = "guest", UsePasswordAuth = false, UseIpAuth = false, IpAddress = ip })
        {
        }

        public bool Equals(Guest guest)
        {
            return guest != null;
        }
    }
}