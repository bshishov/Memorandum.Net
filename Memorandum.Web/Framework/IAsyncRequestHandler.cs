using System.Threading.Tasks;
using Memorandum.Web.Framework.Responses;

namespace Memorandum.Web.Framework
{
    interface IAsyncRequestHandler
    {
        /// <summary>
        /// Main execution method of the handler which returns an HTTP response intent.
        /// </summary>
        /// <returns></returns>
        Task<Response> Execute(IRequest request);
    }
}
