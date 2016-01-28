using Memorandum.Web.Framework.Responses;
using Newtonsoft.Json;

namespace Memorandum.Web.Views.RestApi
{
    class ApiResponse : HttpResponse
    {
        public ApiResponse(object o, int statusCode = 200) : base(content: 
            JsonConvert.SerializeObject(o, new JsonSerializerSettings() { Formatting = Formatting.Indented }), 
            status:statusCode,
            contenttype: "text/plain; charset=utf-8")
        {
            
        }
    }
}