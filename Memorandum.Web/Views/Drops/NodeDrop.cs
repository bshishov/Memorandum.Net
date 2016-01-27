using DotLiquid;

namespace Memorandum.Web.Views.Drops
{
    public abstract class NodeDrop : Drop
    {
        public abstract string Provider { get; }
        public abstract string Id { get; }
    }
}