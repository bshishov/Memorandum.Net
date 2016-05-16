using System;
using System.Collections.Generic;
using Memorandum.Core.Domain.Files;

namespace Memorandum.Web.ViewModels
{
    public abstract class ItemViewModel
    {
        protected readonly IItem Item;

        protected ItemViewModel(IItem item)
        {
            Item = item;
            FullPath = $"{item.Owner.Name}/{item.RelativePath}";
            URL = $"/tree/{FullPath}";
        }

        public bool IsLink => Item.IsLink;
        public string URL { get;  }
        public string Path => Item.RelativePath;
        public string FullPath { get; }
        public string Name => Item.Name;
        public DateTime Modified => Item.Modified;
        public bool IsDirectory => Item.IsDirectory;


        private IEnumerable<DirectoryViewModel> _breadcrumbs;
        public IEnumerable<DirectoryViewModel> Breadcrumbs => _breadcrumbs ?? (_breadcrumbs = GetBreadcrumbs());

        public IEnumerable<DirectoryViewModel> GetBreadcrumbs()
        {
            var bc = new List<DirectoryViewModel>();
            var d = Item.GetParent();
            while (d != null)
            {
                bc.Insert(0, new DirectoryViewModel(d));

                if(d.IsRoot)
                    break;

                d = d.GetParent();
            }

            return bc;
        }
    }
}