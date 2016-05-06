using System;
using Lucene.Net.Documents;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Files;

namespace Memorandum.Core.Search.Mappers
{
    class FileNodeMapper : ILuceneMapper<IItem>
    {
        public static readonly string[] F = {"Id", "IsDirectory" };

        public string[] Fields => F;

        public string IdField => F[0];

        public string IdOf(IItem obj) => obj.FileSystemPath;

        public IItem FromDocument(Document doc)
        {
            throw new NotImplementedException();
            //return FileManager.GetAbsolute(doc.Get(F[0]));
        }

        public Document ToDocument(IItem obj)
        {
            var doc = new Document();
            doc.Add(new Field(F[0], obj.FileSystemPath, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(F[1], obj.IsDirectory.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }
    }
}