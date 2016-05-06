using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Core.Domain.Permissions
{
    public static class PermissionManager
    {
        public static User GetOwner(IItem item)
        {
            foreach (var user in UserManager.Users)
            {
                if (item.FileSystemPath.StartsWith(user.Base.FileSystemPath))
                    return user;
            }
            return null;
        }

        public static bool CanRead(IItem item, User user)
        {
            return true;
        }

        public static bool CanWrite(IItem item, User user)
        {
            return true;
        }
    }
}
