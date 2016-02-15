namespace Memorandum.Web.Views.RestApi
{
    internal class ForbiddenApiResponse : ApiResponse
    {
        public ForbiddenApiResponse(string errmsg = "Forbidden")
            : base(new {Error = errmsg}, 403)
        {
        }
    }
}