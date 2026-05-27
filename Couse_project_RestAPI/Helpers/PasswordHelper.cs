using System.Security.Cryptography;
using System.Text;

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
            var pepper = _configuration["Pepper"];

            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(pepper));
            var preHashBytes
        }

        public bool VerifyPassword(string password)
        {

        }
    }
}
