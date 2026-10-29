using System.Security.Cryptography;
using System.Text;

namespace SchoolERPWebService.Utilities
{
    public static class SecurityHelper
    {
        // IMPORTANT:
        // Password hashing is now generated in SQL Server stored procedures.
        // This helper remains the verifier for hashes returned from SQL.
        //
        // Format expected from DB:
        // - PasswordSalt: Base64 of 32 random bytes
        // - PasswordHash: Base64 of SHA2-512(saltBytes || UTF8(password))
        private const int SaltBytes = 32;  // 256-bit salt
        private const int HashBytes = 64;  // 512-bit hash (SHA-512)

        // Legacy verifier support (older DB rows)
        private const int LegacyHashBytes = 32;
        private const int LegacyIterations = 100_000;
        private static readonly HashAlgorithmName LegacyAlgo = HashAlgorithmName.SHA256;

        /// <summary>
        /// Hashes a password using SHA2-512(salt || password).
        /// Note: the application no longer uses this for user creation;
        /// SQL stored procedures generate hash+salt. Kept for compatibility/tests.
        /// </summary>
        /// <param name="password">Plain text password</param>
        /// <returns>Tuple of (Base64 Hash, Base64 Salt)</returns>
        public static (string Hash, string Salt) HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password)) 
                throw new ArgumentException("Password cannot be empty", nameof(password));

            byte[] saltBytes = RandomNumberGenerator.GetBytes(SaltBytes);
            byte[] hashBytes = HashWithSalt(password, saltBytes);

            return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
        }

        /// <summary>
        /// Verifies a password against a stored hash and salt.
        /// </summary>
        public static bool VerifyPassword(string password, string storedHash, string storedSalt)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(storedHash) || string.IsNullOrEmpty(storedSalt))
                return false;

            try
            {
                byte[] saltBytes = Convert.FromBase64String(storedSalt);
                byte[] storedHashBytes = Convert.FromBase64String(storedHash);

                // New scheme: SHA2-512(salt || password)
                if (saltBytes.Length == SaltBytes && storedHashBytes.Length == HashBytes)
                {
                    byte[] computedHash = HashWithSalt(password, saltBytes);
                    return CryptographicOperations.FixedTimeEquals(computedHash, storedHashBytes);
                }

                // Legacy scheme fallback: PBKDF2-SHA256(password, salt, 100k, 32 bytes)
                if (saltBytes.Length == SaltBytes && storedHashBytes.Length == LegacyHashBytes)
                {
                    byte[] computedLegacy = Rfc2898DeriveBytes.Pbkdf2(
                        Encoding.UTF8.GetBytes(password),
                        saltBytes,
                        LegacyIterations,
                        LegacyAlgo,
                        LegacyHashBytes);
                    return CryptographicOperations.FixedTimeEquals(computedLegacy, storedHashBytes);
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private static byte[] HashWithSalt(string password, byte[] saltBytes)
        {
            // SHA512(saltBytes || passwordBytes)
            // IMPORTANT: In SQL Server, we generate @PwdBytes using:
            //   CONVERT(varbinary(max), @PasswordPlain)
            // where @PasswordPlain is NVARCHAR. That conversion uses UTF-16LE (i.e., .NET Encoding.Unicode).
            byte[] pwdBytes = Encoding.Unicode.GetBytes(password);
            byte[] salted = new byte[saltBytes.Length + pwdBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, salted, 0, saltBytes.Length);
            Buffer.BlockCopy(pwdBytes, 0, salted, saltBytes.Length, pwdBytes.Length);

            byte[] hash = SHA512.HashData(salted);
            if (hash.Length != HashBytes)
                throw new CryptographicException($"Unexpected SHA-512 length: {hash.Length}");
            return hash;
        }
    }
}
