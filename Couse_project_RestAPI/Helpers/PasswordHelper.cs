using System.Security.Cryptography;
using System.Text;
using BCrypt.Net;

namespace Couse_project_RestAPI.Helpers
{
    public class PasswordHelper
    {
        private readonly IConfiguration _configuration;

        public PasswordHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string HashPassword(string password)
        {
            try
            {
                var pepper = _configuration["Pepper"];

                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(pepper)))
                {
                    var preHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));

                    var preHashPassword = Convert.ToBase64String(preHashBytes);

                    return HashPassword(preHashPassword);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public bool VerifyPassword(string passwordInput, string passwordFromDb)
        {
            try
            {
                var pepper = _configuration["Pepper"];

                using (var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(pepper)))
                {
                    var preHashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(passwordInput));

                    var preHashPassword = Convert.ToBase64String(preHashBytes);

                    return VerifyPassword(preHashPassword, passwordFromDb);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
