namespace SimpleAuth.Application.Server
{
    public class AuthProvidersOption
    {
        public const string AuthProviders = nameof(AuthProviders);

        public GoogleProvider? Google { get; set; }

        public class GoogleProvider
        {
            public string? ClientId { get; set; } = string.Empty;
            public string? ClientSecret { get; set; } = string.Empty;
        }
    }
}
