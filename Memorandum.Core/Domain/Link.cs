using System;

namespace Memorandum.Core.Domain
{
    public class Link
    {
        public virtual int Id { get; set; }
        public virtual string Comment { get; set; }
        public virtual string StartNode { get; set; }
        public virtual string EndNode { get; set; }
        public virtual string StartNodeProvider { get; set; }
        public virtual string EndNodeProvider { get; set; }
        public virtual DateTime DateAdded { get; set; }
        public virtual User User { get; set; }

        public virtual NodeIdentifier GetStartIdentifier()
        {
            return new NodeIdentifier(StartNodeProvider, StartNode);
        }

        public virtual NodeIdentifier GetEndIdentifier()
        {
            return new NodeIdentifier(EndNodeProvider, EndNode);
        }

        public Link()
        {
            
        }

        public Link(Node start, Node end)
        {
            if (start != null && start.NodeId != null)
            {
                StartNode = start.NodeId.Id;
                StartNodeProvider = start.NodeId.Provider;
            }
            EndNode = end.NodeId.Id;
            EndNodeProvider = end.NodeId.Provider;
        }

        public override string ToString()
        {
            return $"{StartNodeProvider}:{StartNode} -> {EndNodeProvider}:{EndNode}";
        }
    }
}
