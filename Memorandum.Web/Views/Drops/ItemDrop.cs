using System;
using System.Collections.Generic;
using DotLiquid;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Domain.Permissions;

namespace Memorandum.Web.Views.Drops
{
    public abstract class ItemDrop : Drop
    {
        protected readonly IItem Item;

        protected ItemDrop(IItem item)
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


        private IEnumerable<DirectoryItemDrop> _breadcrumbs;
        public IEnumerable<DirectoryItemDrop> Breadcrumbs => _breadcrumbs ?? (_breadcrumbs = GetBreadcrumbs());

        public IEnumerable<DirectoryItemDrop> GetBreadcrumbs()
        {
            var bc = new List<DirectoryItemDrop>();
            var d = Item.GetParent();
            while (d != null)
            {
                bc.Insert(0, new DirectoryItemDrop(d));

                if(d.IsRoot)
                    break;

                d = d.GetParent();
            }

            return bc;
        }
    }
}