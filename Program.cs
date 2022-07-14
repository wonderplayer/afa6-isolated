using System.Security.Cryptography.X509Certificates;
using afa_net_core_3._1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PnP.Core.Auth.Services.Builder.Configuration;

namespace afa_net_core_3
{
    public class Program
    {
        private static void Main(string[] args)
        {
            ApplicationSettings ApplicationSettings = new();

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((context, services) =>
                {
                    services.AddSingleton(options =>
                    {
                        var configuration = context.Configuration;
                        configuration.Bind(ApplicationSettings);
                        return configuration;
                    });
                    services.AddPnPCore(options =>
                    {
                        options.PnPContext.GraphFirst = false;
                    });
                    services.AddPnPCoreAuthentication(options =>
                    {
                        // Load the certificate to use
                        X509Certificate2 cert = Utilities.LoadCertificate(ApplicationSettings);

                        // Configure certificate based auth
                        options.Credentials.Configurations.Add("CertAuth", new PnPCoreAuthenticationCredentialConfigurationOptions
                        {
                            ClientId = ApplicationSettings.ClientId,
                            TenantId = ApplicationSettings.TenantId,
                            X509Certificate = new PnPCoreAuthenticationX509CertificateOptions
                            {
                                Certificate = cert,
                            }
                        });
                        options.Credentials.DefaultConfiguration = "CertAuth";
                    });
                    services.AddSingleton(sp =>
                    {
                        return ApplicationSettings;
                    });
                })
                .Build();



            host.Run();
        }
    }
}