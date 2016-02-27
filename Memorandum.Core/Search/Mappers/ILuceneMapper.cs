using Lucene.Net.Documents;

namespace Memorandum.Core.Search.Mappers
{
    public interface ILuceneMapper<T>
    {
        string IdField { get; }
        string[] Fields { get; }
        string IdOf(T obj);
        Document ToDocument(T obj);
        T FromDocument(Document doc);
    }
}