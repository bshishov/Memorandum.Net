using System;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public class TextNodeDrop : NodeDrop
    {
        private readonly TextNode _node;

        public int Id { get { return _node.Id; } }
        public string Text { get { return _node.Text; } }
        public DateTime DateAdded { get { return _node.DateAdded; } }

        public TextNodeDrop(TextNode node)
            : base(node.NodeId)
        {
            _node = node;
        }
    }
}