// Name: Brayden Kim
// Student Admin No: 252268E
// Tutorial Group: 07

using System.Security.Cryptography;

namespace ArcaneVaultAPI.Helpers
{
    public static class PasswordHelper
    {
        private const int SaltSize = 32;
        private const int HashSize = 64;
        private const int Iterations = 100000;

        /// <summary>
        /// Hashes a password using PBKDF2 with a random per-user salt.
        /// </summary>
        public static (string hash, string salt) HashPassword(string password)
        {
            byte[] saltBytes = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            string hash = Convert.ToBase64String(
                Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    saltBytes,
                    Iterations,
                    HashAlgorithmName.SHA256,
                    HashSize
                )
            );

            return (hash, Convert.ToBase64String(saltBytes));
        }

        /// <summary>
        /// Verifies a password against a stored hash and salt.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            string computedHash = Convert.ToBase64String(
                Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    saltBytes,
                    Iterations,
                    HashAlgorithmName.SHA256,
                    HashSize
                )
            );

            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(computedHash),
                Convert.FromBase64String(storedHash)
            );
        }
    }
}
