using Memorandum.Core.Domain.Users;

namespace Memorandum.Web.ViewModels
{
    class UserViewModel
    {
        public string Name { get; private set; }
        public DirectoryViewModel Base { get; }

        public UserViewModel(User user)
        {
            Name = user.Name;
            Base = new DirectoryViewModel(user.Base);
        }
    }
}
