using System.Net;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public abstract class NodeDrop : Drop
    {
        protected NodeDrop(Node node)
        {
            NodeId = node.NodeId.Id;
            Provider = node.NodeId.Provider;
            Link = string.Format("/{0}/{1}", Provider, WebUtility.UrlEncode(NodeId));
            if (node.User != null)
                UserId = node.User.Id;
        }

        public string Link { get; private set; }
        public string NodeId { get; }
        public string Provider { get; }
        public int UserId { get; private set; }
    }
}