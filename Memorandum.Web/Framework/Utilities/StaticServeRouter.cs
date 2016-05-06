using System;
using System.IO;
using Memorandum.Web.Framework.Responses;
using Memorandum.Web.Framework.Routing;
using MimeTypes;

namespace Memorandum.Web.Framework.Utilities
{
    class StaticServeRouter : Router
    {
        private readonly string _basePath;

        public StaticServeRouter(string basePath)
        { 
            _basePath = basePath;
            this.Bind("/(.+)$", Handler);
        }

        private Response Handler(IRequest request, string[] args)
        {
            var filePath = args[0];
            var fullPath = Path.Combine(_basePath, filePath);
            var mime = MimeTypeMap.GetMimeType(Path.GetExtension(fullPath));
            var response = new StreamedHttpResponse(File.OpenRead(fullPath), contenttype: mime);
            response.Headers.Add("Expires", DateTime.Now.AddDays(1).ToString("R"));
            response.Headers.Add("Cache-Control", "public");
            return response;
        }
    }
}
