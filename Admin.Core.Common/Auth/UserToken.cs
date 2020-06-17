using System.Security.Claims;

namespace Admin.Core.Common.Auth
{
    public class UserToken:IUserToken
    {
        public string Build(Claim[] claims)
        {
            throw new System.NotImplementedException();
        }

        public Claim[] Decode(string jwtToken)
        {
            throw new System.NotImplementedException();
        }
    }
}