using System;
using System.Collections.Generic;
using Memorandum.Core.Domain;

namespace Memorandum.Core.Repositories
{
    public class NodeRepository : IRepository<Node, NodeIdentifier>
    {
        private readonly FileNodeRepository _fileNodeRepository;
        private readonly TextNodeRepository _textNodeRepository;
        private readonly UrlNodeRepository _urlNodeRepository;

        public NodeRepository(
            TextNodeRepository textNodeRepository, 
            UrlNodeRepository urlNodeRepository, 
            FileNodeRepository fileNodeRepository)
        {
            _urlNodeRepository = urlNodeRepository;
            _textNodeRepository = textNodeRepository;
            _fileNodeRepository = fileNodeRepository;
        }

        public IEnumerable<Node> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Delete(Node entity)
        {
            var provider = entity.NodeId.Provider;
            switch (provider)
            {
                case "text":
                    _textNodeRepository.Delete(entity as TextNode);
                    break;
                case "url":
                    _urlNodeRepository.Delete(entity as URLNode);
                    break;
                case "file":
                    _fileNodeRepository.Delete(entity as FileNode);
                    break;
            }
        }

        public void Save(Node entity)
        {
            var provider = entity.NodeId.Provider;
            switch (provider)
            {
                case "text":
                    _textNodeRepository.Save(entity as TextNode);
                    break;
                case "url":
                    _urlNodeRepository.Save(entity as URLNode);
                    break;
                case "file":
                    _fileNodeRepository.Save(entity as FileNode);
                    break;
            }
        }

        public Node FindById(NodeIdentifier id)
        {
            if (id.Provider == "text")
                return _textNodeRepository.FindById(Convert.ToInt32(id.Id));
            if (id.Provider == "url")
                return _urlNodeRepository.FindById(Convert.ToInt32(id.Id));
            if (id.Provider == "file")
                return _fileNodeRepository.FindById(id.Id);

            return null;
        }

        public IEnumerable<Node> ByIds(NodeIdentifier[] ids)
        {
            throw new NotImplementedException();
        }
    }
}
