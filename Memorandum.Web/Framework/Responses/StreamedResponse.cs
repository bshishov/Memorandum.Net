using System.Collections.Generic;
using System.IO;

namespace Memorandum.Web.Framework.Responses
{
    class StreamedHttpResponse : HttpResponse
    {
        private const int BufferSize = 64*1024;
        private readonly Stream _stream;

        public StreamedHttpResponse(Stream stream, int status = 200, string statusReason = "OK",
            string contenttype = "text/html", Dictionary<string, string> headers = null)
            : base(new byte[0], status, statusReason, contenttype, headers)
        {
            _stream = stream;
        }

        public override void WriteBody(Stream stream)
        {
            if (_stream.CanSeek)
                _stream.Seek(0, SeekOrigin.Begin);
            _stream.CopyTo(stream, BufferSize);
        }

        public override byte[] GetBody()
        {
            using (var ms = new MemoryStream())
            {
                WriteBody(ms);
                return ms.ToArray();
            }
        }

        public override void Dispose()
        {
            _stream?.Close();
            _stream?.Dispose();
        }
    }
}
