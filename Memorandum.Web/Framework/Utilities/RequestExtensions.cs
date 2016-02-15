namespace Memorandum.Web.Framework.Utilities
{
    public static class RequestExtensions
    {
        public static string Method(this FastCGI.Request request)
        {
            return request.GetParameterASCII("REQUEST_METHOD");
        }
    }
}