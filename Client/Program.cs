using ClientInterfaces;
using Components;
using Components.Shared;
using Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Mocks;
using SureCheck.Components;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using ToolFrameworkPackage;
using static SureCheck.Components.ComponentsLocalizer;

namespace Client
{
    public class Program
    {


        public static async Task Main(string[] args)
        {
            var config = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
            var Configuration = config.Build();

            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.RootComponents.Add<App>("#app");

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) })
            .AddSingleton<ISystemInformation, SystemInformation>()
            .AddSingleton<ClientInterfaces.ILocalizeContext, LocalizeContext>()
            .AddSingleton<ClientInterfaces.ILocalizer, ComponentsLocalizer>()
            .AddSingleton<ITestLogService, TestLogService>()
            .AddSingleton<LocalizedContent>()
            .AddSingleton<ISystemInfo, MockSystemInfo>()
            .AddSingleton<Interfaces.IPlatformInfo, PlatformInfo>()
            .AddSingleton<IHTMLLogger, FakeHtmlLogger>()
            .AddSingleton<ILocalizedSystemInfo, LocalizedSystemInfo>()
            .AddSingleton<IHistory, History>()
            .AddSingleton<IPassId, PassId>()
            .AddSingleton<BackgroundInit<IHistory>>()
            .AddSingleton<LogViewModel>()
            .AddSingleton(Task.Run(() => { }))  // Load task Mock
            .AddSingleton<IFrameworkController, FrameworkController>()
            .AddSingleton<IToolFramework, Framework>()
            .AddSingleton<MessageService>()
            .AddSingleton<TestService>()
            .AddSingleton<IMainWindow,MainWindow>()
            .Configure<AppSettings>(Configuration!.GetSection(nameof(AppSettings)))
            .AddSingleton<AppSettings>(s => s.GetRequiredService<IOptions<AppSettings>>().Value)
            .AddSingleton(ComponentsLocalizer.SupportedLanguages);

            await builder.Build().RunAsync();
        }
    }
}
