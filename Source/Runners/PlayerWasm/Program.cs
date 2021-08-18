using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Blazor.Extensions.Logging;
using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using PlayerWasm.Pages;
using PlayerWasm.ViewModels;
using ReactiveUI;
using RestEase.HttpClientFactory;
using Soundr.Commons.Clients;
using Splat;
using Splat.Autofac;
using Splat.Microsoft.Extensions.DependencyInjection;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace PlayerWasm
{
    public class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.ConfigureContainer(new AutofacServiceProviderFactory(ConfigureContainer));

            builder.Services
                .AddBlazorise( options =>
                {
                    options.ChangeTextOnKeyPress = false;
                } )
                .AddBootstrapProviders()
                .AddFontAwesomeIcons();

            var resolver = Locator.CurrentMutable;
            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            builder.RootComponents.Add<App>("#app");

            var services = builder.Services;
            services.AddScoped(
                sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});
            
            services.AddRestEaseClient<IMusicPlayerApiClient>(builder.HostEnvironment.BaseAddress);

            services.AddTransient<IHubConnectionBuilder, HubConnectionBuilder>();

            builder.Services.AddLogging(b => b
                .AddBrowserConsole()
                .SetMinimumLevel(LogLevel.Information)
            );

            var host = builder.Build();
            ServiceProvider = host.Services;
            await host.RunAsync();
        }

        private static void ConfigureContainer(ContainerBuilder container)
        {
            container.UseAutofacDependencyResolver();
            container.RegisterModule<AutofacModule>();
        }
    }
}