namespace HMProductInfoAPI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public CurrentUserService(IHttpContextAccessor contextAccessor)
        {
            this._contextAccessor = contextAccessor;
        }

        public string GetCurrentUserName()
        {
            return _contextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
