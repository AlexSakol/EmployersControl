using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CRUDMVC.Models
{
    public class AuthOptions
    {
        public const string ISSUER = "MyAuthServer"; 
        public const string AUDIENCE = "MyAuthClient"; 
        private const string KEY = $"MySecret:MySecretSecretKey:0358!";   
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
