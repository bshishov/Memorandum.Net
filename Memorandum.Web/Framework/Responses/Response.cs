using System;
using System.IO;

namespace Memorandum.Web.Framework.Responses
{
    internal abstract class Response : IDisposable
    {
        public abstract void WriteBody(Stream stream);
        public abstract byte[] GetBody();
        public abstract void Dispose();
    }
}