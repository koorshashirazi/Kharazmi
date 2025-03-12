using System;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.Cryptography
{
    /// <summary>
    ///
    /// 
    /// </summary>
    public static class CertificateHelper
    {
        /// <summary>
        /// StoreLocation CurrentUser
        /// StoreName My
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertificateFromFile(string filePath, string password)
        {
            if (filePath.IsEmpty()) throw new NullReferenceException(nameof(filePath));
            if (password.IsEmpty()) throw new NullReferenceException(nameof(password));
            if (!File.Exists(filePath)) throw new Exception($"Validation key file: {filePath} not found");

            try
            {
                using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(new X509Certificate2(filePath, password, X509KeyStorageFlags.Exportable));
                }

                var certificate = new X509Certificate2(
                    filePath,
                    password,
                    X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable |
                    X509KeyStorageFlags.UserKeySet);
                return certificate;
            }
            catch (CryptographicException e)
            {
                throw new Exception(
                    $"There was an error adding the key file - during the creation of the validation key {e.Message}");
            }
           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="password"></param>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <param name="keyStorageFlags"></param>
        /// <returns></returns>
        public static X509Certificate2 LoadCertificateFromFile(string filePath, string password, StoreName storeName,
            StoreLocation storeLocation, X509KeyStorageFlags keyStorageFlags )
        {
            if (filePath.IsEmpty()) throw new NullReferenceException(nameof(filePath));
            if (password.IsEmpty()) throw new NullReferenceException(nameof(password));
            if (!File.Exists(filePath)) throw new Exception($"Validation key file: {filePath} not found");

            try
            {
                using (var store = new X509Store(storeName, storeLocation))
                {
                    store.Open(OpenFlags.ReadWrite);
                    store.Add(new X509Certificate2(filePath, password, X509KeyStorageFlags.Exportable));
                }

                var certificate = new X509Certificate2(
                    filePath,
                    password,
                    keyStorageFlags);
           
                return certificate;
            }
            catch (CryptographicException e)
            {
                throw new Exception(
                    $"There was an error adding the key file - during the creation of the validation key {e.Message}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thumbPrint"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static X509Certificate2 LoadCertificateFromStore(string thumbPrint)
        {
            if(thumbPrint.IsEmpty()) throw  new NullReferenceException(nameof(thumbPrint));

            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);

            var certCollection =
                store.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, true);

            if (certCollection.Count == 0) throw new Exception("The specified certificate wasn't found.");

            return certCollection[0];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="thumbPrint"></param>
        /// <param name="storeName"></param>
        /// <param name="storeLocation"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static X509Certificate2 LoadCertificateFromStore(string thumbPrint, StoreName storeName,
            StoreLocation storeLocation)
        {
            if(thumbPrint.IsEmpty()) throw  new NullReferenceException(nameof(thumbPrint));
            
            using var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            var certCollection =
                store.Certificates.Find(X509FindType.FindByThumbprint, thumbPrint, true);

            if (certCollection.Count == 0) throw new Exception("The specified certificate wasn't found.");

            return certCollection[0];
        }
    }
}