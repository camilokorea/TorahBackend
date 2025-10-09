namespace TorahBackend.Domain.Entities
{
    public class JwtConfig
    {
        public string? Secret { get; set; }
        public string? Issuer { get; set; }
        public TimeSpan ExpiryTime { get; set; }
    }
}
