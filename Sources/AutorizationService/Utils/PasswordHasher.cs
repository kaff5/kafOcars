using System;
using System.Security.Cryptography;
using System.Text;

namespace KafOCars.Utils
{
    public static class PasswordHasher
    {
        private const int SaltSize = 32;
        private const int KeySize = 64;
        private const int Iterations = 10000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

        public static string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            using (var rng = new RNGCryptoServiceProvider())
            {
                // Генерация соли
                byte[] salt = new byte[SaltSize];
                rng.GetBytes(salt);

                // Генерация хэша
                byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

                // Кодирование в Base64
                return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hash)}";
            }
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));

            if (string.IsNullOrWhiteSpace(hashedPassword))
                throw new ArgumentException("Hashed password cannot be null or empty.", nameof(hashedPassword));

            var parts = hashedPassword.Split(':');
            if (parts.Length != 2)
                throw new FormatException("Invalid hashed password format.");

            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] hash = Convert.FromBase64String(parts[1]);

            // Генерация хэша для входного пароля
            byte[] inputHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

            // Сравнение хэшей
            return CryptographicOperations.FixedTimeEquals(inputHash, hash);
        }
    }
}
