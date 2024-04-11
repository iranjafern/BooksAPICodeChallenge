using Newtonsoft.Json;
using System.Text.Json;

namespace Books.API.Security
{
    public class OktaToken
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }    

        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }        
    }
}
