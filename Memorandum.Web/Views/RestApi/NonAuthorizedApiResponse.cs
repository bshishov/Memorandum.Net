namespace Memorandum.Web.Views.RestApi
{
    internal class NonAuthorizedApiResponse : ApiResponse
    {
        public NonAuthorizedApiResponse() : base(new {Error = "Non authorized, Authorize using POST to /api/auth"}, 401)
        {
        }
    }
}