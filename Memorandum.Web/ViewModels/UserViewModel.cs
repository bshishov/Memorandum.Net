using DotLiquid;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Web.ViewModels
{
    public class UserViewModel
    {
        private readonly User _user;

        public string Name { get; private set; }
        public DirectoryViewModel Base { get; }

        public UserViewModel(User user)
        {
            _user = user;
            Name = user.Name;

            if(user.Base != null)
                Base = new DirectoryViewModel(user.Base);
        }

        public User GetModel() => _user;
    }
}
