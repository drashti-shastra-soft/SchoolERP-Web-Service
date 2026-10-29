using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace SchoolERPWebService.Utilities
{
    public class EncryptionHelper
    {
        private readonly byte[] _key;

        public EncryptionHelper(IConfiguration configuration)
        {
            var keyString = configuration["SecuritySettings:EncryptionKey"];
            if (string.IsNullOrEmpty(keyString))
            {
                // For development, use a default key if not provided, 
                // but in production this MUST be in appsettings.json
                keyString = "SchoolERP_Default_AES_Key_2026_!@"; 
            }
            
            // Ensure key is 32 bytes (256 bits)
            _key = Encoding.UTF8.GetBytes(keyString.PadRight(32).Substring(0, 32));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;

            using Aes aes = Aes.Create();
            aes.Key = _key;
            aes.GenerateIV();
            byte[] iv = aes.IV;

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using var ms = new MemoryStream();
            
            // Prepend IV to the stream
            ms.Write(iv, 0, iv.Length);

            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            using (var sw = new StreamWriter(cs))
            {
                sw.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;

            try
            {
                byte[] fullCipher = Convert.FromBase64String(cipherText);
                using Aes aes = Aes.Create();
                aes.Key = _key;

                byte[] iv = new byte[aes.BlockSize / 8];
                byte[] cipher = new byte[fullCipher.Length - iv.Length];

                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                Array.Copy(fullCipher, iv.Length, cipher, 0, cipher.Length);

                aes.IV = iv;

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using var ms = new MemoryStream(cipher);
                using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
                using var sr = new StreamReader(cs);

                return sr.ReadToEnd();
            }
            catch
            {
                // If decryption fails (e.g. data not encrypted yet), return original
                return cipherText;
            }
        }
    }
}
