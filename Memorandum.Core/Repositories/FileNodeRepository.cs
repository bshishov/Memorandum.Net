using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memorandum.Core.Domain;
using Memorandum.Core.Search;

namespace Memorandum.Core.Repositories
{
    public class FileNodeRepository : IRepository<BaseFileNode, string>
    {
        public IEnumerable<BaseFileNode> GetAll()
        {
            throw new InvalidOperationException("Can't get all files specify a root");
        }

        public void Delete(BaseFileNode entity)
        {
            var dir = entity as DirectoryNode;
            if(dir != null)
            { 
                // Remove each subdirecotry and file from index
                dir.PerformOnChild(SearchManager.FileNodeIndex.ClearLuceneIndexRecord, true);

                // Remove directory iteslf
                Directory.Delete(entity.Path, true);
            }
            else
            {
                File.Delete(entity.Path);
            }

            SearchManager.FileNodeIndex.ClearLuceneIndexRecord(entity);
        }

        public void Save(BaseFileNode entity)
        {
            SearchManager.FileNodeIndex.AddUpdateLuceneIndex(entity);
        }

        public BaseFileNode FindById(string id)
        {
            try
            {
                var path = Path.GetFullPath(id);
                var attr = File.GetAttributes(path);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    return new DirectoryNode(path);

                return new FileNode(path);
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public IEnumerable<BaseFileNode> ByIds(string[] ids)
        {
            return ids.Select(FindById);
        }

        public FileNode CreateFileFromStream(string filePath, Stream stream)
        {
            using (var fileStream = File.Create(filePath))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(fileStream);
            }

            var fileNode = new FileNode(filePath);
            Save(fileNode);
            return fileNode;
        }

        public IEnumerable<Node> SearchFiles(string query)
        {
            var files = Directory.GetFiles(Directory.GetCurrentDirectory(), query, SearchOption.AllDirectories);
            return files.Select(file => new FileNode(file));
        }
    }
}