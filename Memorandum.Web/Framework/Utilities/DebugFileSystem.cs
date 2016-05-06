using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DotLiquid;
using DotLiquid.Exceptions;
using DotLiquid.FileSystems;

namespace Memorandum.Web.Framework.Utilities
{
    class DebugFileSystem : IFileSystem
    {
        public string Root { get; }

        public DebugFileSystem(string root)
        {
            Root = Path.GetFullPath(root);
        }

        public string ReadTemplateFile(DotLiquid.Context context, string templateName)
        {
            string templatePath = (string)context[templateName];
            string fullPath = FullPath(templatePath);
            if (!File.Exists(fullPath))
                throw new FileSystemException("Template does not exist", templatePath);
            return File.ReadAllText(fullPath);
        }

        public string FullPath(string templatePath)
        {
            if (templatePath == null || !Regex.IsMatch(templatePath, @"^[^.\/][a-zA-Z0-9_\/]+$"))
                throw new FileSystemException("Illegal Template Name", templatePath);

            string fullPath = templatePath.Contains("/")
                ? Path.Combine(Root, Path.GetDirectoryName(templatePath), $"_{Path.GetFileName(templatePath)}.liquid")
                : Path.Combine(Root, $"_{templatePath}.liquid");

            return fullPath;
        }
    }
}
