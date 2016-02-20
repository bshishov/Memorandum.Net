namespace Memorandum.Web.Views.RestApi
{
    internal class ResourceNotFoundApiResponse : ApiResponse
    {
        public ResourceNotFoundApiResponse(string error = "Resource not found") : 
            base(null, 404, error)
        {
        }
    }
}