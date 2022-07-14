namespace afa_net_core_3._1
{
    public class ApplicationSettings
    {
        public string AzureWebJobsStorage { get; set; }
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Thumbprint { get; set; }
        public string CertificateFromKeyVault { get; set; }
    }
}
