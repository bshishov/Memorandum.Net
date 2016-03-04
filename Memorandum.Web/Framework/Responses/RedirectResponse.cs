using System.Collections.Generic;

namespace Memorandum.Web.Framework.Responses
{
    internal class RedirectResponse : HttpResponse
    {
        public RedirectResponse(string url)
            : base(status: 302, statusReason: "Found", contenttype: "", headers: new Dictionary<string, string>
            {
                {"Location", url}
            })
        {
        }
    }
}