using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace Memorandum.Web.Framework.Responses
{
    internal class HttpResponse : Response
    {
        public WebHeaderCollection Headers { get; } = new WebHeaderCollection();
        protected byte[] Content;
        private readonly string _statusReason;
        
        public HttpResponse(byte[] content, int status = 200, string statusReason = "OK",
            string contenttype = "text/html", Dictionary<string, string> headers = null)
        {
            StatusCode = status;
            Headers.Add("Content-Type", contenttype);
            if (headers != null)
            {
                foreach (var attribute in headers)
                    Headers.Add(attribute.Key, attribute.Value);
            }
            _statusReason = statusReason;
            Content = content;
        }

        public HttpResponse(string content = "", int status = 200, string statusReason = "OK",
            string contenttype = "text/html", Dictionary<string, string> headers = null)
        {
            StatusCode = status;
            Headers.Add("Content-Type", contenttype);
            if (headers != null)
            {
                foreach (var attribute in headers)
                    Headers.Add(attribute.Key, attribute.Value);
            }
            _statusReason = statusReason;
            Content = Encoding.UTF8.GetBytes(content);
        }

        public int StatusCode { get; }

        public byte[] GetHeader()
        {
            using (var ms = new MemoryStream())
            {
                var header = Encoding.UTF8.GetBytes($"HTTP/1.1 {StatusCode} {_statusReason}");
                ms.Write(header, 0, header.Length);

                var headers = Headers.ToByteArray();
                ms.Write(headers, 0, headers.Length);

                WriteString(ms, "\n\n");
                ms.Close();
                return ms.ToArray();
            }
        }

        private static void WriteString(Stream stream, string input)
        {
            var data = Encoding.UTF8.GetBytes(input);
            stream.Write(data, 0, data.Length);
        }

        public override void WriteBody(Stream stream)
        {
            try
            {
                stream.Write(Content, 0, Content.Length);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public override byte[] GetBody()
        {
            return Content;
        }

        public override void Close()
        {
        }
    }
}