using Memorandum.Core.Repositories;

namespace Memorandum.Core.Domain
{
    public abstract class Node
    {
        public abstract NodeIdentifier NodeId { get; }

        public abstract User User { get; set; }
     
    }
}