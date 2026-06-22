using System;
using System.Security.Cryptography;

namespace Capa.Datos
{
    public static class PasswordHelper
    {
        private const int SaltSize = 16; 
        private const int KeySize = 32; 
        private const int Iterations = 10000;

        public static string HashPassword(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(password, SaltSize, Iterations, HashAlgorithmName.SHA256))
            {
                var key = algorithm.GetBytes(KeySize);
                var salt = algorithm.Salt;

                var hashBytes = new byte[SaltSize + KeySize];
                Array.Copy(salt, 0, hashBytes, 0, SaltSize);
                Array.Copy(key, 0, hashBytes, SaltSize, KeySize);

                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrEmpty(hashedPassword))
                return false;

            if (hashedPassword.Length < 30)
            {
                return password == hashedPassword;
            }

            try
            {
                var hashBytes = Convert.FromBase64String(hashedPassword);
                var salt = new byte[SaltSize];
                Array.Copy(hashBytes, 0, salt, 0, SaltSize);

                using (var algorithm = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256))
                {
                    var key = algorithm.GetBytes(KeySize);
                    for (int i = 0; i < KeySize; i++)
                    {
                        if (hashBytes[i + SaltSize] != key[i])
                            return false;
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
