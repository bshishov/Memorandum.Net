using System;
using Lucene.Net.Documents;
using Memorandum.Core.Domain;

namespace Memorandum.Core.Search.Mappers
{
    class LinkMapper : ILuceneMapper<Link>
    {
        public static readonly string[] F = { "Id", "Comment", "StartNodeProvider", "StartNode", "EndNodeProvider", "EndNode", "DateAdded", "UserId" };

        public string IdField => F[0];
        public string[] Fields => F;
        public string IdOf(Link obj) => obj.Id.ToString();

        public Document ToDocument(Link obj)
        {
            var doc = new Document();

            doc.Add(new Field(F[0], obj.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[1], obj.Comment, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(F[2], obj.StartNodeProvider, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[3], obj.StartNode, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[4], obj.EndNodeProvider, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[5], obj.EndNode, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[6], obj.DateAdded.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[7], obj.User.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            return doc;
        }

        public Link FromDocument(Document doc)
        {
            return new Link
            {
                Id = Convert.ToInt32(doc.Get(F[0])),
                Comment = doc.Get(F[1]),
                StartNodeProvider = doc.Get(F[2]),
                StartNode = doc.Get(F[3]),
                EndNodeProvider = doc.Get(F[4]),
                EndNode = doc.Get(F[5]),
                DateAdded = Convert.ToDateTime(doc.Get(F[6])),
                User = new User() {Id = Convert.ToInt32(doc.Get(F[7]))},
            };
        }
    }
}
