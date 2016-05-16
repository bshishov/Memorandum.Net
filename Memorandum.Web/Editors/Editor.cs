using System.Collections.Generic;
using Memorandum.Core.Domain.Files;
using Memorandum.Web.Editors.Actions;

namespace Memorandum.Web.Editors
{
    abstract class Editor<T>
        where T : IItem
    {
        public IViewFactory<T> ViewFactory { get; }
        public abstract string Name { get; }

        private readonly List<IItemAction<T>> _actions;

        protected Editor(IViewFactory<T> viewFactory, params IItemAction<T>[] actions)
        {
            _actions = new List<IItemAction<T>>(actions);
            ViewFactory = viewFactory;
        }

        public IItemAction<T> GetAction(string name)
        {
            return _actions.Find(a => a.Action.Equals(name));
        }

        public abstract bool CanHandle(T item);
    }
}
