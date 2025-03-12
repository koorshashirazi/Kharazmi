namespace Kharazmi.AspNetCore.Core.Cryptography
{
    /// <summary>
    /// Add it as services.AddSingleton(IProtectionProvider, ProtectionProvider)
    /// </summary>
    public interface IProtectionProvider
    {
        /// <summary>
        /// Decrypts the message
        /// </summary>
        string Decrypt(string inputText);

        /// <summary>
        /// Encrypts the message
        /// </summary>
        string Encrypt(string inputText);
    }
}