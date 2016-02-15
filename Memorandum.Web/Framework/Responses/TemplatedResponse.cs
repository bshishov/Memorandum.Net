using System.IO;
using System.Text;
using DotLiquid;

namespace Memorandum.Web.Framework.Responses
{
    internal class TemplatedResponse : HttpResponse
    {
        public TemplatedResponse(string templatePath, object context, int status = 200)
            : base(status: status, contenttype: "text/html; charset=utf-8")
        {
            var tpl = Template.Parse(File.ReadAllText("Templates/" + templatePath + ".liquid"));
            Content = Encoding.UTF8.GetBytes(tpl.Render(Hash.FromAnonymousObject(context)));
        }
    }
}