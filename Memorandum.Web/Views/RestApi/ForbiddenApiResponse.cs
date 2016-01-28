namespace Memorandum.Web.Views.RestApi
{
    class ForbiddenApiResponse : ApiResponse
    {
        public ForbiddenApiResponse(string errmsg = "Forbidden")
            : base(new { Error = errmsg }, 403)
        {
        }
    }
}