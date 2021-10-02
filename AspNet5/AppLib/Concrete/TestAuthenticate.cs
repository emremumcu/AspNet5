namespace AspNet5.AppLib.Concrete
{
    using AspNet5.AppLib.Abstract;

    public class TestAuthenticate : IAuthenticate
    {
        public bool AuthenticateUser(string userId, string password)
        {
            // TODO : Check user credentials
            return true;
        }
    }
}
