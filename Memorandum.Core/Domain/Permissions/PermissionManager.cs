using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Core.Domain.Permissions
{
    public static class PermissionManager
    {
        public static User GetOwner(this IItem item)
        {
            return UserManager.Users.FirstOrDefault(user => item.FileSystemPath.StartsWith(user.Base.FileSystemPath));
        }
        
        public static bool CanRead(this User user, IItem item)
        {
            // Owner can read files he own
            if (item.Owner.Equals(user))
                return true;
            
            var sharing = item.GetSharingFor(user);
            if (sharing != null &&
                (sharing.Type == SharingType.ReadOnly || sharing.Type == SharingType.ReadAndWrite))
                return true;

            // if not shared
            return false;
        }

        public static bool CanWrite(this User user, IItem item)
        {
            // Owner can write files he own
            if (item.Owner.Equals(user))
                return true;

            var sharing = item.GetSharingFor(user);

            if (sharing != null &&
                (sharing.Type == SharingType.ReadAndWrite))
                return true;
            
            return false;
        }

        public static Sharing GetSharingFor(this IItem item, User targetUser)
        {
            // Finds all sharings from item owner with target user
            var sharingsWithUser = item?.Owner?.Sharings?.Where(s => s.Target.Equals(targetUser));

            // Find from this sharings the sharing for item or its parent
            // For ex. User1 shares /files/ folder with User2, User2 wants to see /files/some.txt, some.txt must be shared as its parent
            return sharingsWithUser?.FirstOrDefault(s => item.FileSystemPath.StartsWith(s.Item.FileSystemPath));
        }
    }
}
