namespace Memorandum.Web.Framework.Responses
{
    internal abstract class Response
    {
        public abstract void Write(Request request);
    }
}