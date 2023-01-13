namespace SimpleAuth.Application
{
    public class ServerSettingsOption
    {
        public const string ServerSettings = nameof(ServerSettings);

        public string ServerUrl { get; set; } = string.Empty;
        public string EncryptionCertificatePath { get; set; } = string.Empty;
        public string EncryptionCertificatePassword { get; set; } = string.Empty;
        public string SigninCertificatePath { get; set; } = string.Empty;
        public string SigninCertificatePassword { get; set; } = string.Empty;
    }
}
