using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Kharazmi.AspNetCore.Core.Cryptography
{
    /// <summary>
    /// 
    /// </summary>
    public static class CryptographyExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Decrypt(this string inputText, string key, string salt)
        {
            var inputBytes = Convert.FromBase64String(inputText);
            var pdb = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(salt));

            using var ms = new MemoryStream();
            using var alg = Aes.Create();
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            using (var cs = new CryptoStream(ms, alg.CreateDecryptor(), CryptoStreamMode.Write))
            {
                cs.Write(inputBytes, 0, inputBytes.Length);
            }

            return Encoding.UTF8.GetString(ms.ToArray());
        }

       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputText"></param>
        /// <param name="key"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        public static string Encrypt(this string inputText, string key, string salt)
        {
            var inputBytes = Encoding.UTF8.GetBytes(inputText);
            var pdb = new Rfc2898DeriveBytes(key, Encoding.UTF8.GetBytes(salt));
            using var ms = new MemoryStream();
            using var alg = Aes.Create();
            alg.Key = pdb.GetBytes(32);
            alg.IV = pdb.GetBytes(16);

            using (var cs = new CryptoStream(ms, alg.CreateEncryptor(), CryptoStreamMode.Write))
            {
                cs.Write(inputBytes, 0, inputBytes.Length);
            }

            return Convert.ToBase64String(ms.ToArray());
        }
    }
}