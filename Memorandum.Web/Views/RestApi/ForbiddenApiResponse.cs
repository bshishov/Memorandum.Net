namespace Memorandum.Web.Views.RestApi
{
    internal class ForbiddenApiResponse : ApiResponse
    {
        public ForbiddenApiResponse(string error = "Forbidden")
            : base(null, 403, error)
        {
        }
    }
}