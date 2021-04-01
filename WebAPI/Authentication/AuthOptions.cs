using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WebAPI.Authentication
{
    public class AuthOptions
    {
        public const string ISSUER = "AuthServer"; 
        public const string AUDIENCE = "AuthClient"; 
        const string KEY = "PpznvRziY5xzLab1FsW2gLCYUcKrO09m"; 
        public const int LIFETIME = 4; 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
