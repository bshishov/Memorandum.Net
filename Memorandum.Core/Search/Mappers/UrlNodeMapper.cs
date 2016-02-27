using System;
using Lucene.Net.Documents;
using Memorandum.Core.Domain;

namespace Memorandum.Core.Search.Mappers
{
    class UrlNodeMapper : ILuceneMapper<URLNode>
    {
        public static readonly string[] F = { "Id", "Url", "Name", "Image", "DateAdded", "UserId" };

        public string[] Fields => F;

        public string IdField => F[0];

        public string IdOf(URLNode obj) => obj.NodeId.Id;

        public URLNode FromDocument(Document doc)
        {
            return new URLNode
            {
                Id = Convert.ToInt32(doc.Get(F[0])),
                URL = doc.Get(F[1]),
                Name= doc.Get(F[2]),
                Image = doc.Get(F[3]),
                DateAdded = Convert.ToDateTime(doc.Get(F[4])),
                User = new User() { Id = Convert.ToInt32(doc.Get(F[5])) },
            };
        }

        public Document ToDocument(URLNode obj)
        {
            var doc = new Document();

            doc.Add(new Field(F[0], obj.NodeId.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[1], obj.URL, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(F[2], obj.Name ?? string.Empty, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(F[3], obj.Image ?? string.Empty, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[4], obj.DateAdded.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[5], obj.User.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            return doc;
        }
    }
}