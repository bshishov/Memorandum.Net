using System;
using System.Collections.Generic;
using System.IO;

namespace Memorandum.Web.Framework.Responses
{
    class StreamedHttpResponse : HttpResponse, IDisposable
    {
        private const int BufferSize = 64*1024;
        private readonly Stream _stream;

        public StreamedHttpResponse(Stream stream, int status = 200, string statusReason = "OK",
            string contenttype = "text/html", Dictionary<string, string> headers = null)
            : base(new byte[0], status, statusReason, contenttype, headers)
        {
            _stream = stream;
            //_stream.Seek(0, SeekOrigin.Begin);
        }

        public override void WriteBody(Stream stream)
        {
            var buffer = new byte[BufferSize];
            int read;
            while (_stream.CanRead && (read = _stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                stream.Write(buffer, 0, read);
            }
        }

        public override byte[] GetBody()
        {
            using (var ms = new MemoryStream())
            {
                WriteBody(ms);
                return ms.ToArray();
            }
        }

        public override void Close()
        {
            _stream?.Close();
        }

        public void Dispose()
        {
            _stream?.Close();
        }
    }
}
