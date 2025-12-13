using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;

namespace UserManagementService.Services
{
    public class PasswordService
    {
        const int SaltSize = 16;
        const int Iterations = 10000;
        const int HashSize = 32;
        private readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA256;
        private const char stringDelimiter = '.';

        public string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(SaltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, HashSize);

            return string.Join(stringDelimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        public bool VerifyPassword(string password, string storedHash)
        {
            var parts = storedHash.Split(stringDelimiter);
            var salt = Convert.FromBase64String(parts[0]);
            var hash = Convert.FromBase64String(parts[1]);

            var newHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, HashSize);
            return newHash.SequenceEqual(hash);
        }
    }
}
