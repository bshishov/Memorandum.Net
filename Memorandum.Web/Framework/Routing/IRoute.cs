using System.Text.RegularExpressions;

namespace Memorandum.Web.Framework.Routing
{
    interface IRoute
    {
        Regex Regex { get; }
    }
}