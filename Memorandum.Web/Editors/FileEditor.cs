using System.Linq;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Views.Drops;

namespace Memorandum.Web.Editors
{
    public abstract class FileEditor : IFileEditor
    {
        public abstract string Name { get; }
        public abstract string Template { get; }

        protected string[] Extensions;

        protected FileEditor(params string[] extensions)
        {
            Extensions = extensions;
        }

        public virtual bool CanHandle(IFileItem item)
        {
            return Extensions.Contains(item.Extension);
        }

        public virtual FileItemDrop GetView(IFileItem item)
        {
            return new FileItemDrop(item);
        }
    }
}