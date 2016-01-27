using System.Collections.Generic;
using System.IO;
using System.Linq;
using Memorandum.Core.Domain;

namespace Memorandum.Core.Repositories
{
    public class FileNodeRepository : IRepository<BaseFileNode, string>
    {
        public IEnumerable<BaseFileNode> GetAll()
        {
            throw new System.NotImplementedException();
        }

        public void Delete(BaseFileNode entity)
        {
            throw new System.NotImplementedException();
        }

        public void Save(BaseFileNode entity)
        {
            throw new System.NotImplementedException();
        }

        public BaseFileNode FindById(string id)
        {
            var path = Path.GetFullPath(id);
            var attr = File.GetAttributes(path);
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                return new DirectoryNode(path);
            
            return new FileNode(path);
        }

        public IEnumerable<BaseFileNode> ByIds(string[] ids)
        {
            return ids.Select(FindById);
        }
    }
}