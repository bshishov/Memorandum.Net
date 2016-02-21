using System.Globalization;

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

        public NodeIdentifier(string provider, int id) : this(provider, id.ToString(CultureInfo.InvariantCulture))
        {
        }

        public override bool Equals(object obj)
        {
            var id2 = obj as NodeIdentifier;
            if (id2 == null)
                return false;
            return this.Id.Equals(id2.Id) && this.Provider.Equals(id2.Provider);
        }

        public override int GetHashCode()
        {
            return Provider.GetHashCode() ^ Id.GetHashCode();
        }
    }
}