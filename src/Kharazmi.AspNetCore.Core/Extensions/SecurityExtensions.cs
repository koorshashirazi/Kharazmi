using System;
using System.Security.Cryptography;
using System.Text;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    /// <summary>
    ///     Information hashing algorithm.
    ///     We use both DataLayer and Services as a way to hash passwords.
    /// </summary>
    public static partial class Core
    {
        public static string GetSha256Hash(this string text)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(text);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);
                return Convert.ToBase64String(byteHash);
            }
        }
        
        public static string GetMd5Hash(this string input)
        {
            using (var md5 = MD5.Create())
            {
                var bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(input));

                var sBuilder = new StringBuilder();

                foreach (var dataByte in bytes)
                {
                    sBuilder.Append(dataByte.ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }
    }
}