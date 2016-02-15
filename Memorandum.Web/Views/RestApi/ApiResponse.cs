using Memorandum.Web.Framework.Responses;
using Newtonsoft.Json;

namespace Memorandum.Web.Views.RestApi
{
    internal class ApiResponse : HttpResponse
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };

        public ApiResponse(object o, int statusCode = 200) : base(JsonConvert.SerializeObject(o, Settings),
            statusCode,
            contenttype: "text/plain; charset=utf-8")
        {
        }
    }
}