using Memorandum.Core.Domain.Files;

namespace Memorandum.Core.Domain.Users
{
    public class Sharing
    {
        public IItem Item { get; }
        public User Target { get; }
        public SharingType Type { get; }

        public Sharing(IItem item, User target, SharingType type)
        {
            Item = item;
            Target = target;
            Type = type;
        }

        public Sharing(User owner, SharingInfo info)
            : this(FileManager.Get(owner, info.ItemRelativePath), UserManager.Get(info.Target), info.Type)
        {
        }

        public Sharing(User owner, string relPath, string target, SharingType type)
            : this(FileManager.Get(owner, relPath), UserManager.Get(target), type)
        {
        }

        public static implicit operator SharingInfo(Sharing sharing)
        {
            return new SharingInfo()
            {
                ItemRelativePath = sharing.Item.RelativePath,
                Target = sharing.Target.Name,
                Type = sharing.Type
            };
        }
    }
}