namespace Memorandum.Web.Views.RestApi
{
    internal class NonAuthorizedApiResponse : ApiResponse
    {
        public NonAuthorizedApiResponse() :
            base(null, 401, "Non authorized, Authorize using POST to /api/auth")
        {
        }
    }
}