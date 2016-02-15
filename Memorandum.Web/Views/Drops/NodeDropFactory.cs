using Memorandum.Core.Domain;

namespace Memorandum.Web.Views.Drops
{
    public static class NodeDropFactory
    {
        public static NodeDrop Create(Node node)
        {
            if (node is TextNode)
                return new TextNodeDrop((TextNode) node);

            if (node is URLNode)
                return new UrlNodeDrop((URLNode) node);

            if (node is FileNode)
                return new FileNodeDrop((FileNode) node);

            if (node is DirectoryNode)
                return new DirectoryNodeDrop((DirectoryNode) node);

            return null;
        }
    }
}