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

        public ApiResponse(object payload = null, int statusCode = 200, string statusMessage = "") : base(JsonConvert.SerializeObject(new
        {
            StatusCode = statusCode,
            StatusMessage = statusMessage,
            Data = payload
        }, Settings),
            statusCode,
            contenttype: "text/plain; charset=utf-8")
        {
        }
    }
}