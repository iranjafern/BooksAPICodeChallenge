namespace Books.API.Security
{
    public class OktaJwtVerificationOptions
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Issuer { get; set; }
    }
}
