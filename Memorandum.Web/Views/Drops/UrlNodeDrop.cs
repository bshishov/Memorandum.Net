using System;
using Memorandum.Core.Domain;
using Memorandum.Web.Views.Providers;

namespace Memorandum.Web.Views.Drops
{
    internal class UrlNodeDrop : NodeDrop
    {
        private readonly URLNode _node;

        public UrlNodeDrop(URLNode node) : base(node)
        {
            _node = node;
        }

        public int Id => _node.Id;

        public string Name => _node.Name;

        public string Url => _node.URL;

        public string Image => _node.Image;

        public DateTime DateAdded => _node.DateAdded;

        public string Extension => UrlProvider.GetKnownExtension(_node);
    }
}