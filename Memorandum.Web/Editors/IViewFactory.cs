using Memorandum.Core.Domain.Files;
using Memorandum.Web.ViewModels;

namespace Memorandum.Web.Editors
{
    interface IViewFactory<in T>
        where T : IItem
    {
        ItemViewModel Create(T item);
    }
}