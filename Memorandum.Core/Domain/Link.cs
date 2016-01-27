namespace Memorandum.Core.Domain
{
    public class Link
    {
        public virtual int Id { get; set; }
        public virtual string Relation { get; set; }
        public virtual string StartNode { get; set; }
        public virtual string EndNode { get; set; }
        public virtual string StartNodeProvider { get; set; }
        public virtual string EndNodeProvider { get; set; }
        public virtual User User { get; set; }


        public Link()
        {
            
        }

        public Link(Node start, Node end)
        {
            StartNode = start.NodeId;
            StartNodeProvider = start.Provider;
            EndNode = end.NodeId;
            EndNodeProvider = end.Provider;
        }

        public override string ToString()
        {
            return string.Format("{0}:{1} -> {2}:{3}", StartNodeProvider, StartNode, EndNodeProvider, EndNode);
        }
    }
}
