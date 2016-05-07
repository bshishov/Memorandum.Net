using System.Collections.Generic;
using System.Linq;

namespace Memorandum.Web.Creators
{
    static class CreatorManager
    {
        private static readonly List<ICreator> _creators;
        
        public static IEnumerable<ICreator> Creators => _creators;

        static CreatorManager()
        {
            _creators = new List<ICreator>
            {
                new UploaderCreator(),
                new URLCreator(),
                new DirectoryCreator(),
                new TextCreator(),
                new MarkdownCreator()
            };
        }

        public static ICreator GetCreator(string id)
        {
            return _creators.FirstOrDefault(e => e.Id.Equals(id));
        }
    }
}
