using System.Collections.Generic;
using System.Linq;
using Memorandum.Core.Domain.Files;

namespace Memorandum.Core.Domain.Users
{
    public class User
    {
        public UserInfo Info { get; }
        public string Name => Info.Name;
        public IDirectoryItem Base { get; }
        public string BaseDirectory => Info.BaseDirectory;
        public string Password => Info.Password;
        public string IpAddress => Info.IpAddress;
        public bool UseIpAuth => Info.UseIpAuth;
        public bool UsePasswordAuth => Info.UsePasswordAuth;

        public IEnumerable<Sharing> Sharings
        {
            get
            {
                if (_sharings == null)
                {
                    _sharings = Info.Sharings?.Select(s => new Sharing(this, s)).ToList();

                    if(_sharings == null)
                        _sharings = new List<Sharing>();
                }

                return _sharings;
            }
        }

        private List<Sharing> _sharings;

        public User(UserInfo info)
        {
            Info = info;
            Base = FileManager.GetDirectory(this, string.Empty);
        }

        public void Share(IItem item, User to, SharingType type)
        {
            var existingShare = Sharings.FirstOrDefault(s => s.Item.Equals(item) && s.Target.Equals(to));
            if (existingShare != null)
            {
                _sharings.Remove(existingShare);
            }

            var sharing = new Sharing(item, to, type);
            _sharings.Add(new Sharing(item, to, type));
            Info.Sharings.Add(sharing);
            UserManager.Save();
        }

        public bool Equals(User user)
        {
            return user.Name.Equals(this.Name);
        }

        public static implicit operator UserInfo(User user)
        {
            return user.Info;
        }

        public override string ToString()
        {
            return $"User {Name}";
        }
    }
}