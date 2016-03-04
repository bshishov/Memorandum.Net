using System.Collections.Generic;
using System.Collections.Specialized;
using HttpMultipartParser;
using Memorandum.Core;
using Memorandum.Core.Domain;
using Memorandum.Web.Middleware;

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

        /// <summary>
        /// TODO: MOVE TO CUSTOM REQUEST OR WHATEVER
        /// </summary>
        SessionContext Session { get; set; }

        /// <summary>
        /// TODO: MOVE TO CUSTOM REQUEST OR WHATEVER
        /// </summary>
        UnitOfWork UnitOfWork { get; set; }

        /// <summary>
        /// TODO: MOVE TO CUSTOM REQUEST OR WHATEVER
        /// </summary>
        int? UserId { get; set; }

        /// <summary>
        /// TODO: MOVE TO CUSTOM REQUEST OR WHATEVER
        /// </summary>
        User User { get; }
    }
}