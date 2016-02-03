using System;
using System.Collections.Generic;
using System.Text;

namespace Memorandum.Web.Framework.Responses
{
    class HttpResponse : Response
    {
        protected byte[] Content;
        private readonly int _status;
        private readonly string _statusReason;
        private readonly string _contentType;

        public int StatusCode { get { return _status; } }

        public Dictionary<string, string> Attributes;

        public HttpResponse(byte[] content, int status = 200, string statusReason = "OK", string contenttype = "text/html", Dictionary<string, string> attributes = null)
        {
            _status = status;
            _contentType = contenttype;
            Attributes = attributes;
            _statusReason = statusReason;
            Content = content;
        }

        public HttpResponse(string content = "", int status = 200, string statusReason = "OK", string contenttype = "text/html", Dictionary<string, string> attributes = null)
        {
            _status = status;
            _contentType = contenttype;
            Attributes = attributes;
            _statusReason = statusReason;
            Content = Encoding.UTF8.GetBytes(content);
        }

        public override void Write(Request request)
        {
            try
            {
                var header = string.Format("HTTP/1.1 {0} {1}", _status, _statusReason);
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