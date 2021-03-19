using System.Text.Json.Serialization;

namespace Auth0DeviceAuthorizationFlowSample.ConsoleApp
{
    public class DeviceCodeResponseBody
    {
        [JsonPropertyName("device_code")]
        public string DeviceCode { get; set; }

        [JsonPropertyName("user_code")]
        public string UserCode { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
        
        [JsonPropertyName("verification_uri")]
        public string VerificationUri { get; set; }

        [JsonPropertyName("interval")]
        public int IntervalInSeconds { get; set; }

        [JsonPropertyName("verification_uri_complete")]
        public string VerificationUriComplete { get; set; }
    }
}