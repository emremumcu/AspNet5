namespace AspNet5.AppLib.Abstract
{
    public interface IAuthenticate
    {
        public bool AuthenticateUser(string userId, string password);
    }
}
