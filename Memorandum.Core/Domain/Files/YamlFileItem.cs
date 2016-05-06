using System.IO;
using Memorandum.Core.Domain.Users;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Memorandum.Core.Domain.Files
{
    class YamlFileItem<T> : FileItem
    {
        private T _data;

        public T Data
        {
            get
            {
                if (_data == null)
                    Load();
                return _data;
            }
        }

        public YamlFileItem(User user, string relativePath) : base(user, relativePath)
        {
        }

        public void Load()
        {
            if(!File.Exists(FileSystemPath))
                return;

            using (var reader = new StreamReader(GetStream(FileMode.Open)))
            {
                var deserializer = new Deserializer(namingConvention: new PascalCaseNamingConvention());
                _data = deserializer.Deserialize<T>(reader);
            }
        }

        public void Save()
        {
            using (var writer = new StreamWriter(GetStream(FileMode.OpenOrCreate)))
            {
                var serializer = new Serializer(namingConvention: new PascalCaseNamingConvention());
                serializer.Serialize(writer, Data);
            }
        }
    }
}
