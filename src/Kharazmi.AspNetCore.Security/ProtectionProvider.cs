using System;
using System.Security.Cryptography;
using System.Text;
using Kharazmi.AspNetCore.Core.Cryptography;
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.Extensions;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Logging;

namespace Kharazmi.AspNetCore.Security
{
    /// <summary>
    /// Protection Provider Service
    /// </summary>
    public class ProtectionProvider : IProtectionProvider, ISingletonDependency
    {
        private readonly ILogger<ProtectionProvider> _logger;
        private readonly IDataProtector _dataProtector;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataProtectionProvider"></param>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProtectionProvider(
            IDataProtectionProvider dataProtectionProvider,
            ILogger<ProtectionProvider> logger)
        {
            if (dataProtectionProvider == null)
            {
                throw new ArgumentNullException(nameof(dataProtectionProvider));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataProtector = dataProtectionProvider.CreateProtector(typeof(ProtectionProvider).FullName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string Decrypt(string inputText)
        {
            if (inputText == null)
            {
                throw new ArgumentNullException(nameof(inputText));
            }

            try
            {
                var inputBytes = Convert.FromBase64String(inputText);
                var bytes = _dataProtector.Unprotect(inputBytes);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex.Message, "Invalid base 64 string. Fall through.");
            }
            catch (CryptographicException ex)
            {
                _logger.LogError(ex.Message, "Invalid protected payload. Fall through.");
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputText"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public string Encrypt(string inputText)
        {
            inputText.CheckArgumentIsNull(nameof(inputText));

            var inputBytes = Encoding.UTF8.GetBytes(inputText);
            var bytes = _dataProtector.Protect(inputBytes);
            return Convert.ToBase64String(bytes);
        }
    }
}