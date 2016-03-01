using System.Text;
using DotLiquid;
using Memorandum.Web.Framework.Utilities;

namespace Memorandum.Web.Framework.Responses
{
    internal class TemplatedResponse : HttpResponse
    {
        public TemplatedResponse(string templatePath, object context, int status = 200)
            : base(status: status, contenttype: "text/html; charset=utf-8")
        {
            Content = Encoding.UTF8.GetBytes(TemplateUtilities.RenderTemplate(templatePath, context));
        }
    }
}