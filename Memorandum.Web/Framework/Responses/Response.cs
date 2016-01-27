namespace Memorandum.Web.Framework.Responses
{
    abstract class Response
    {
        protected Response() { }

        public abstract void Write(Request request);
    }
}