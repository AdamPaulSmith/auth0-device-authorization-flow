using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Auth0DeviceAuthorizationFlowSample.ConsoleApp
{
    public class App
    {
        private readonly ILogger _logger;

        public App(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("Auth0DeviceAuthorizationFlowSample");
        }

        public async Task RunAsync()
        {
            var httpClient = new HttpClient();
            var deviceCodeFlowRequestBodyContent = new FormUrlEncodedContent(new[]
                                                                             {
                                                                                 new KeyValuePair<string, string>("client_id", "SRVobuIu5ly2jGc4ukOMvcEchIz9B8x2"),
                                                                                 new KeyValuePair<string, string>("scope", "create:users"),
                                                                                 new KeyValuePair<string, string>("audience", "https://adam-smith-test-tenant.eu.auth0.com/api/v2/")
                                                                             });
            HttpResponseMessage deviceCodeFlowResponseMessage = await httpClient.PostAsync("https://adam-smith-test-tenant.eu.auth0.com/oauth/device/code", deviceCodeFlowRequestBodyContent);
            string deviceCodeFlowResponseBody = await deviceCodeFlowResponseMessage.Content.ReadAsStringAsync();
            if(deviceCodeFlowResponseMessage.IsSuccessStatusCode)
            {
                var deviceCodeResponseBody = JsonSerializer.Deserialize<DeviceCodeResponseBody>(deviceCodeFlowResponseBody);
                _logger.LogInformation($"Navigate to {deviceCodeResponseBody.VerificationUriComplete} to sign in. The process will continue once you have successfully signed in with an account that has the correct permissions.");

                var loginSuccessful = false;
                DateTime validateLoginWindowExpiryTime = DateTime.Now.AddSeconds(deviceCodeResponseBody.ExpiresIn);
                while(!loginSuccessful && DateTime.Now <= validateLoginWindowExpiryTime)
                {
                    await Task.Delay(TimeSpan.FromSeconds(deviceCodeResponseBody.IntervalInSeconds));
                    var loginValidationRequestBodyContent = new FormUrlEncodedContent(new[]
                                                                                      {
                                                                                          new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:device_code"),
                                                                                          new KeyValuePair<string, string>("client_id", "SRVobuIu5ly2jGc4ukOMvcEchIz9B8x2"),
                                                                                          new KeyValuePair<string, string>("device_code", deviceCodeResponseBody.DeviceCode)
                                                                                      });
                    HttpResponseMessage loginValidationResponseMessage = await httpClient.PostAsync("https://adam-smith-test-tenant.eu.auth0.com/oauth/token", loginValidationRequestBodyContent);
                    if(loginValidationResponseMessage.IsSuccessStatusCode)
                    {
                        _logger.LogInformation("User logged in successfully. Continuing.");
                        string loginValidationResponseContent = await loginValidationResponseMessage.Content.ReadAsStringAsync();
                        _logger.LogInformation(loginValidationResponseContent);
                        loginSuccessful = true;
                    }
                    else
                    {
                        _logger.LogInformation($"User has not logged in yet. Waiting another {deviceCodeResponseBody.IntervalInSeconds} seconds to check again.");
                    }
                }
            }
            else
            {
                _logger.LogError("An error occured when calling the auth server.");
                _logger.LogError($"The http requests error code returned was {deviceCodeFlowResponseMessage.StatusCode}.");
                _logger.LogError($"The http requests body contains: {deviceCodeFlowResponseBody}");
            }

            _logger.LogInformation("Done.");
        }
    }
}