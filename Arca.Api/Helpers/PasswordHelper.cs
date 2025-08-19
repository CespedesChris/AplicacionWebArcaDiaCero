using BCrypt.Net;

namespace Arca.Api.Helpers
{
    public static class PasswordHelper
    {
        // Genera hash seguro de la contraseña
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Verifica la contraseña contra el hash guardado
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}