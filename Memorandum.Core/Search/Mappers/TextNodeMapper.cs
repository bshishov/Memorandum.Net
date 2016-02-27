using System;
using Lucene.Net.Documents;
using Memorandum.Core.Domain;

namespace Memorandum.Core.Search.Mappers
{
    class TextNodeMapper : ILuceneMapper<TextNode>
    {
        public static readonly string[] F = { "Id", "Text", "DateAdded", "UserId" };

        public string[] Fields => F;

        public string IdField => F[0];

        public string IdOf(TextNode obj) => obj.NodeId.Id;

        public TextNode FromDocument(Document doc)
        {
            return new TextNode
            {
                Id = Convert.ToInt32(doc.Get(F[0])),
                Text = doc.Get(F[1]),
                DateAdded = Convert.ToDateTime(doc.Get(F[2])),
                User = new User() { Id = Convert.ToInt32(doc.Get(F[3]))},
            };
        }

        public Document ToDocument(TextNode obj)
        {
            var doc = new Document();

            doc.Add(new Field(F[0], obj.NodeId.Id, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[1], obj.Text, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(F[2], obj.DateAdded.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field(F[3], obj.User.Id.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

            return doc;
        }
    }
}
