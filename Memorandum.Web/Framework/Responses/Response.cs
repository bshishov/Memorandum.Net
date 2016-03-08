using System.IO;

namespace Memorandum.Web.Framework.Responses
{
    internal abstract class Response
    {
        public abstract void WriteBody(Stream stream);
        public abstract byte[] GetBody();
        public abstract void Close();
    }
}