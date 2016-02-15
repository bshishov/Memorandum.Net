using System;
using System.Collections.Generic;
using System.Text;

namespace Memorandum.Web.Framework.Responses
{
    internal class HttpResponse : Response
    {
        public Dictionary<string, string> Attributes;
        protected byte[] Content;
        private readonly string _contentType;
        private readonly string _statusReason;

        public HttpResponse(byte[] content, int status = 200, string statusReason = "OK",
            string contenttype = "text/html", Dictionary<string, string> attributes = null)
        {
            StatusCode = status;
            _contentType = contenttype;
            Attributes = attributes;
            _statusReason = statusReason;
            Content = content;
        }

        public HttpResponse(string content = "", int status = 200, string statusReason = "OK",
            string contenttype = "text/html", Dictionary<string, string> attributes = null)
        {
            StatusCode = status;
            _contentType = contenttype;
            Attributes = attributes;
            _statusReason = statusReason;
            Content = Encoding.UTF8.GetBytes(content);
        }

        public int StatusCode { get; }

        public override void Write(Request request)
        {
            try
            {
                var header = string.Format("HTTP/1.1 {0} {1}", StatusCode, _statusReason);
                request.RawRequest.WriteResponseUtf8(header);

                if (!string.IsNullOrEmpty(_contentType))
                    request.RawRequest.WriteResponseUtf8(string.Format("\nContent-Type:{0}", _contentType));

                if (Attributes != null)
                    foreach (var kvp in Attributes)
                        request.RawRequest.WriteResponseUtf8(string.Format("\n{0}:{1}", kvp.Key, kvp.Value));

                request.RawRequest.WriteResponseASCII("\n\n");
                request.RawRequest.WriteResponse(Content);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}