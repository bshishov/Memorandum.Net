using System;
using Lucene.Net.Documents;
using Memorandum.Core.Domain;

namespace Memorandum.Core.Search.Mappers
{
    class FileNodeMapper : ILuceneMapper<BaseFileNode>
    {
        public static readonly string[] F = {"Path", "IsDirectory" };

        public string[] Fields => F;

        public string IdField => F[0];

        public string IdOf(BaseFileNode obj) => obj.Path;

        public BaseFileNode FromDocument(Document doc)
        {
            var isDir = Convert.ToBoolean(doc.Get(F[1]));

            if(isDir)
                return new DirectoryNode(doc.Get(F[0]));

            return new FileNode(doc.Get(F[0]));
        }

        public Document ToDocument(BaseFileNode obj)
        {
            var doc = new Document();
            doc.Add(new Field(F[0], obj.Path, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field(F[1], obj.IsDirectory.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            return doc;
        }
    }
}