using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Auth0DeviceAuthorizationFlowSample.ConsoleApp
{
    public static class Program
    {
        public static async Task Main()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                                         {
                                             builder.AddSimpleConsole(options =>
                                                                      {
                                                                          options.SingleLine = true;
                                                                          options.TimestampFormat = "hh:mm:ss ";
                                                                      });
                                         })
                             .AddTransient<App>();
            ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();
            var app = serviceProvider.GetService<App>();
            await app.RunAsync();
            Console.ReadKey();
        }
    }
}