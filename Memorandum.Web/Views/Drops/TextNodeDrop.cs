using System;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public class TextNodeDrop : NodeDrop
    {
        private readonly TextNode _node;

        public TextNodeDrop(TextNode node)
            : base(node)
        {
            _node = node;
        }

        public int Id => _node.Id;

        public string Text => _node.Text;

        public DateTime DateAdded => _node.DateAdded;
    }
}