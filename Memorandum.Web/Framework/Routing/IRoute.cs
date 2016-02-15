using System.Text.RegularExpressions;

namespace Memorandum.Web.Framework.Routing
{
    internal interface IRoute
    {
        Regex Regex { get; }
    }
}