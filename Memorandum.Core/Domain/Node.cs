namespace Memorandum.Core.Domain
{
    public abstract class Node
    {
        public abstract User User { get; set; }

        /// <summary>
        /// A provider must be set for each node
        /// </summary>
        public abstract string Provider { get; }

        /// <summary>
        /// string Id of a node
        /// </summary>
        public abstract string NodeId { get; }
    }
}