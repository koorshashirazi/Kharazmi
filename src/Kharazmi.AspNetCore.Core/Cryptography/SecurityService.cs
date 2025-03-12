using System;
using System.Security.Cryptography;
using System.Text;
using Kharazmi.AspNetCore.Core.Dependency;

namespace Kharazmi.AspNetCore.Core.Cryptography
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISecurityService : ISingletonDependency
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Guid NewSecureGuid();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        string ComputeSha256Hash(string input);
    }

    internal sealed class SecurityService : ISecurityService
    {
        private readonly RandomNumberGenerator _rand = RandomNumberGenerator.Create();

        public Guid NewSecureGuid()
        {
            var bytes = new byte[16];
            _rand.GetBytes(bytes);
            return new Guid(bytes);
        }

        public string ComputeSha256Hash(string input)
        {
            using var hashAlgorithm = SHA256.Create();
            var byteValue = Encoding.UTF8.GetBytes(input);
            var byteHash = hashAlgorithm.ComputeHash(byteValue);
            return Convert.ToBase64String(byteHash);
        }
    }
}