using System;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public class TextNodeDrop : NodeDrop
    {
        private readonly TextNode _node;

        public override string Provider { get { return _node.Provider; } }
        public override string Id { get { return _node.NodeId; } }
        public string Text { get { return _node.Text; } }
        public DateTime DateAdded { get { return _node.DateAdded; } }

        public TextNodeDrop(TextNode node)
        {
            _node = node;
        }
    }
}