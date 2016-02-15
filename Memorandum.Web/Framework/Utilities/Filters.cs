using System.Net;

namespace Memorandum.Web.Framework.Utilities
{
    internal static class Filters
    {
        public static string UrlEncode(string input)
        {
            return WebUtility.UrlEncode(input);
        }
    }
}