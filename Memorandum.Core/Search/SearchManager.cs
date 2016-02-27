using System.Linq;
using System.Threading.Tasks;
using Memorandum.Core.Domain;
using Memorandum.Core.Search.Mappers;

namespace Memorandum.Core.Search
{
    public static class SearchManager
    {
        private static readonly string BaseDirectory = 
            System.IO.Directory.GetCurrentDirectory();

        public static readonly LuceneIndex<TextNode> TextNodeIndex = 
            new LuceneIndex<TextNode>(System.IO.Path.Combine(BaseDirectory, "TextNodeIndex"), new TextNodeMapper());

        public static readonly LuceneIndex<URLNode> UrlNodeIndex =
            new LuceneIndex<URLNode>(System.IO.Path.Combine(BaseDirectory, "URLNodeIndex"), new UrlNodeMapper());

        public static readonly LuceneIndex<BaseFileNode> FileNodeIndex =
            new LuceneIndex<BaseFileNode>(System.IO.Path.Combine(BaseDirectory, "FileNodeIndex"), new FileNodeMapper());

        public static readonly LuceneIndex<Link> LinkIndex =
            new LuceneIndex<Link>(System.IO.Path.Combine(BaseDirectory, "LinkIndex"), new LinkMapper());

        public static bool IndexDotFiles { get; set; }

        public static void StartIndexingTask()
        {
            Task.Run(() => BuildIndecesTask());
        }

        private static void BuildIndecesTask()
        {
            TextNodeIndex.ClearLuceneIndex();
            FileNodeIndex.ClearLuceneIndex();
            UrlNodeIndex.ClearLuceneIndex();
            LinkIndex.ClearLuceneIndex();

            using (var unit = new UnitOfWork())
            {
                TextNodeIndex.AddUpdateLuceneIndex(unit.Text.GetAll());
                UrlNodeIndex.AddUpdateLuceneIndex(unit.URL.GetAll());
                LinkIndex.AddUpdateLuceneIndex(unit.Links.GetAll());

                // TODO: index like GetAll();
                var fileLinks = unit.Links.Where(l => l.EndNodeProvider == "file").ToList();
                var fileNodes = unit.Files.ByIds(fileLinks.Select(l => l.EndNode).ToArray()).ToList();
                FileNodeIndex.AddUpdateLuceneIndex(fileNodes);

                foreach (var dir in fileNodes.OfType<DirectoryNode>())
                {
                    IndexFileDirectory(dir);
                }
            }
        }

        private static void IndexFileDirectory(DirectoryNode input)
        {
            var child = input.GetChild().ToList();

            if (!IndexDotFiles)
                child = child.Where(f => !f.Name.StartsWith(".")).ToList();

            FileNodeIndex.AddUpdateLuceneIndex(child);

            foreach (var dir in child.OfType<DirectoryNode>())
            {
                IndexFileDirectory(dir);
            }
        }
    }
}
