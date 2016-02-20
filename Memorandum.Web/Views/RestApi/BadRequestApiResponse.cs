namespace Memorandum.Web.Views.RestApi
{
    internal class BadRequestApiResponse : ApiResponse
    {
        public BadRequestApiResponse(string error = "Bad request") : base(null, 400, error)
        {
        }
    }
}