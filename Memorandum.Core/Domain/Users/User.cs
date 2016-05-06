using Memorandum.Core.Domain.Files;

namespace Memorandum.Core.Domain.Users
{
    public class User
    {
        public string Name { get; private set; }
        public IDirectoryItem Base { get; }
        public string BaseDirectory { get; }
        public string Password { get; }

        public User(UserInfo info)
        {
            Name = info.Name;
            BaseDirectory = info.BaseDirectory;
            Password = info.Password;
            Base = new DirectoryItem(this, string.Empty);
        }
    }
}