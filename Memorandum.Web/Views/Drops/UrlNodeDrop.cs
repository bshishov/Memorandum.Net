using System;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    class UrlNodeDrop : NodeDrop
    {
        private readonly URLNode _node;

        public UrlNodeDrop(URLNode node) : base(node)
        {
            _node = node;
        }

        public int Id { get { return _node.Id; } }
        public string Name { get { return _node.Name; } }
        public string Url { get { return _node.URL; } }
        public string Image { get { return _node.Image; } }
        public DateTime DateAdded { get { return _node.DateAdded; } }
    }
}