using Memorandum.Web.Creators;

namespace Memorandum.Web.ViewModels
{
    class CreatorViewModel
    {
        private readonly ICreator _creator;
        public string Id => _creator.Id;
        public string Name => _creator.Name;
        public string Template => _creator.Template;

        public CreatorViewModel(ICreator creator)
        {
            _creator = creator;
        }
    }
}
