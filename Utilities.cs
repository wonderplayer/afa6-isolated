using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace afa_net_core_3._1
{
    public static class Utilities
    {
        public static X509Certificate2 GetCertificate(string thumbprint)
        {
            X509Certificate2 certificate;
            X509Store certStore = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            certStore.Open(OpenFlags.ReadOnly);
            try
            {
                certificate = certStore.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false)[0];
            }
            catch
            {
                throw new CryptographicException("No certificate found with thumbprint: " + thumbprint);
            }
            finally
            {
                certStore.Close();
            }
            return certificate;
        }

        public static X509Certificate2 LoadCertificate(ApplicationSettings azureFunctionAppSettings)
        {
            Console.WriteLine("Loading certificate.");
            // Will only be populated correctly when running in the Azure Function host
            string certBase64Encoded = azureFunctionAppSettings.CertificateFromKeyVault;

            if (!string.IsNullOrEmpty(certBase64Encoded))
            {
                return CloudFlow(certBase64Encoded);
            }
            else
            {
                return LocalFlow(azureFunctionAppSettings);
            }
        }

        private static X509Certificate2 LocalFlow(ApplicationSettings azureFunctionAppSettings)
        {
            Console.WriteLine("Using local flow.");
            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            X509Certificate2Collection certificateCollection = store.Certificates.Find(X509FindType.FindByThumbprint, azureFunctionAppSettings.Thumbprint, false);
            store.Close();

            return certificateCollection[0];
        }

        private static X509Certificate2 CloudFlow(string certBase64Encoded)
        {
            Console.WriteLine($"Using Azure Function flow. '{certBase64Encoded}'");
            // Azure Function flow
            return new X509Certificate2(Convert.FromBase64String(certBase64Encoded),
                                        "",
                                        X509KeyStorageFlags.Exportable |
                                        X509KeyStorageFlags.MachineKeySet |
                                        X509KeyStorageFlags.EphemeralKeySet);
        }
    }
}
