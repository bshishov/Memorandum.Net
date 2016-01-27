using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public abstract class NodeDrop : Drop
    {
        public string NodeId { get; private set; }
        public string Provider { get; private set; }

        protected NodeDrop(NodeIdentifier identifier)
        {
            NodeId = identifier.Id;
            Provider = identifier.Provider;
        }
    }
}