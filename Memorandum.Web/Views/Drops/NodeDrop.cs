using System.Net;
using DotLiquid;
using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public abstract class NodeDrop : Drop
    {
        public string Link { get; private set; }
        public string NodeId { get; private set; }
        public string Provider { get; private set; }
        public int UserId { get; private set; }

        protected NodeDrop(Node node)
        {
            NodeId = node.NodeId.Id;
            Provider = node.NodeId.Provider;
            Link = string.Format("/{0}/{1}", Provider, WebUtility.UrlEncode(NodeId));
            if(node.User != null)
                UserId = node.User.Id;
        }
    }
}