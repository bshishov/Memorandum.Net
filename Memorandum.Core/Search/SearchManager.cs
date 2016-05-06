using System.Threading.Tasks;
using Memorandum.Core.Domain;
using Memorandum.Core.Domain.Files;
using Memorandum.Core.Search.Mappers;

namespace Memorandum.Core.Search
{
    public static class SearchManager
    {
        private static readonly string BaseDirectory = 
            System.IO.Directory.GetCurrentDirectory();

        public static readonly LuceneIndex<IItem> Index =
            new LuceneIndex<IItem>(System.IO.Path.Combine(BaseDirectory, "Index"), new FileNodeMapper());
  

        public static bool IndexDotFiles { get; set; }

        public static void StartIndexingTask()
        {
            Task.Run(() => BuildIndecesTask());
        }

        private static void BuildIndecesTask()
        {
            Index.ClearLuceneIndex();
            foreach (var dir in FileManager.MonitoredDirectories)
            {
                dir.PerformOnchild(IndexFileNode, true);
            }
        }

        private static void IndexFileNode(IItem node)
        {
            if (!IndexDotFiles && node.Name.StartsWith("."))
                return;

            Index.AddUpdateLuceneIndex(node);
        }
    }
}
