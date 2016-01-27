namespace Memorandum.Core.Domain
{
    public class NodeIdentifier
    {
        public readonly string Provider;
        public readonly string Id;

        public NodeIdentifier(string provider, string id)
        {
            Provider = provider;
            Id = id;
        }
    }
}