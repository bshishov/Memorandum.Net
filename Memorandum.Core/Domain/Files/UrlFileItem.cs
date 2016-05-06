using System.IO;
using Memorandum.Core.Domain.Users;

namespace Memorandum.Core.Domain.Files
{
    public class UrlFileItem : FileItem
    {
        public string Url { get; set; }

        public UrlFileItem(User user, string relativePath) : base(user, relativePath)
        {
        }

        public UrlFileItem(string url, User user, string relativePath) : base(user, relativePath)
        {
            Url = url;
        }

        public void Load()
        {
            if (!File.Exists(FileSystemPath))
                return;

            using (var reader = new StreamReader(GetStream(FileMode.Open)))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if (line != null && (line.StartsWith("URL") || line.StartsWith("Url")))
                    {
                        Url = line.Substring(4);
                    }
                }
            }
        }

        public void Save()
        {
            using (var writer = new StreamWriter(GetStream(FileMode.OpenOrCreate)))
            {
                writer.WriteLine("[InternetShortcut]");
                writer.WriteLine($"URL={Url}");
            }
        }
    }
}