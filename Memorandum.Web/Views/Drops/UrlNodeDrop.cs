using System;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    class UrlNodeDrop : NodeDrop
    {
        private readonly URLNode _node;

        public UrlNodeDrop(URLNode node)
        {
            _node = node;
        }

        public override string Provider { get { return _node.Provider; }}
        public override string Id { get { return _node.Hash; } }
        public string Name { get { return _node.Name; } }
        public string Hash { get { return _node.Hash; } }
        public string Url { get { return _node.URL; } }
        public string Image { get { return _node.Image; } }
        public DateTime DateAdded { get { return _node.DateAdded; } }
    }
}