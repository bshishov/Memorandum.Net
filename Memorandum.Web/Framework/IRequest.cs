using System.Collections.Generic;
using System.Collections.Specialized;
using HttpMultipartParser;
using Memorandum.Web.Framework.Middleware.Session;

namespace Memorandum.Web.Framework
{
    internal interface IRequest
    {
        string Method { get; }
        string Path { get; }
        string ContentType { get; }

        /// <summary>
        ///     Cookies from request
        /// </summary>
        NameValueCollection Cookies { get; }

        /// <summary>
        ///     Query arguments
        /// </summary>
        NameValueCollection QuerySet { get; }

        /// <summary>
        ///    POST arguments if Method == POST
        /// </summary>
        NameValueCollection PostArgs { get; }

        IEnumerable<FilePart> Files { get; }

        ISessionContext Session { get; set; }
    }
}