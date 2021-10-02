namespace AspNet5.AppLib.Abstract
{
    using Microsoft.AspNetCore.Authentication;

    public interface IAuthorize
    {
        public AuthenticationTicket GetTicket(string userId);
    }
}
