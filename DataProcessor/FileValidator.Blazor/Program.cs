using FileValidator.Domain.Services;
using MatBlazor;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileValidator.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");

            var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };

            builder.Services.AddSingleton(httpClient);

            var fileSpecificationsStore = await BuildFileSpecificationsStoreAsync(httpClient);
            builder.Services.AddSingleton<IFileSpecificationsStore>(fileSpecificationsStore);

            var sampleFileStore = await BuildSampleFileStoreAsync(httpClient);
            builder.Services.AddSingleton<ISampleFileStore>(sampleFileStore);

            builder.Services.AddScoped<IFileDecoder, FileDecoder>();

            var appState = CreateAppState(sampleFileStore);
            builder.Services.AddSingleton(appState);
            builder.Services.AddSingleton(appState.HomePage);
            builder.Services.AddSingleton(appState.LoadedFilePage);
            builder.Services.AddSingleton(appState.LoadedFileJsonPage);
            builder.Services.AddSingleton(appState.LoadedFileGroupsJsonPage);
            builder.Services.AddSingleton(appState.FileSpecificationsPage);

            builder.Services.AddSingleton<ApplicationsEvents>();

            ConfigureToaster(builder.Services);
            await builder.Build().RunAsync();
        }

        private static void ConfigureToaster(IServiceCollection services)
        {
            services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.BottomCenter;
                config.PreventDuplicates = false;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 3000;
            });
        }

        private static async Task<FileSpecificationsStore> BuildFileSpecificationsStoreAsync(HttpClient httpClient)
        {
            var fileSpecificationsStore = new FileSpecificationsStore();
            await SampleFileStoreLoader.LoadFileSpecsAsync(fileSpecificationsStore, httpClient);
            return fileSpecificationsStore;
        }

        private static async Task<SampleFileStore> BuildSampleFileStoreAsync(HttpClient httpClient)
        {
            var sampleFileStore = new SampleFileStore();
            await SampleFileStoreLoader.LoadSampleFileAsync(sampleFileStore, httpClient);
            return sampleFileStore;
        }

        private static AppState CreateAppState(SampleFileStore sampleFileStore)
        {
            var appState = new AppState();

            var getSampleFileByIdResult = sampleFileStore.GetSampleFileById(1);
            if (getSampleFileByIdResult.Succeed)
            {
                appState.HomePage.InputDataContent = getSampleFileByIdResult.Data.Content;
            }

            return appState;
        }
    }
}
