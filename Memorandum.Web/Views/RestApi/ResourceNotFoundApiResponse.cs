namespace Memorandum.Web.Views.RestApi
{
    class ResourceNotFoundApiResponse : ApiResponse
    {
        public ResourceNotFoundApiResponse(string errmsg = "Resource not found")
            : base(new { Error = errmsg }, 404)
        {
        }
    }
}