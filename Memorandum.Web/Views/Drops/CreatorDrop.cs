using DotLiquid;
using Memorandum.Web.Creators;

namespace Memorandum.Web.Views.Drops
{
    class CreatorDrop : Drop
    {
        private readonly ICreator _creator;
        public string Id => _creator.Id;
        public string Name => _creator.Name;
        public string Template => _creator.Template;

        public CreatorDrop(ICreator creator)
        {
            _creator = creator;
        }
    }
}
