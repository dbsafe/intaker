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
            var fileSpecificationsStore = new FileSpecificationsStore();
            await FileSpecificationsStoreLoader.LoadAsync(fileSpecificationsStore, httpClient);

            builder.Services.AddSingleton(httpClient);

            var appState = CreateAppState();
            builder.Services.AddSingleton(appState);
            builder.Services.AddSingleton(appState.HomePage);
            builder.Services.AddSingleton(appState.LoadedFilePage);
            builder.Services.AddSingleton(appState.LoadedFileJsonPage);
            builder.Services.AddSingleton(appState.LoadedFileGroupsJsonPage);
            builder.Services.AddSingleton(appState.FileSpecificationsPage);

            builder.Services.AddSingleton<ApplicationsEvents>();
            builder.Services.AddSingleton<IFileSpecificationsStore>(fileSpecificationsStore);
            builder.Services.AddScoped<IFileDecoder, FileDecoder>();

            builder.Services.AddMatToaster(config =>
            {
                config.Position = MatToastPosition.BottomCenter;
                config.PreventDuplicates = false;
                config.NewestOnTop = true;
                config.ShowCloseButton = true;
                config.MaximumOpacity = 95;
                config.VisibleStateDuration = 3000;
            });

            await builder.Build().RunAsync();
        }

        private static AppState CreateAppState()
        {
            var appState = new AppState();
            appState.HomePage.InputDataContent = @"HEADER,09212013,ABCDCompLndn,0012
BALANCE,1001,111-22-1001,Araceli,Larson,10212000,1000.00,AA
BALANCE,1002,111-22-1002,Kate,Hoover,11252000,2000.00,AA
BALANCE,1003,111-22-1003,Donald,Stephens,10261930,3000.00,AA
BALANCE,1004,111-22-1004,Reese,Lin,10011971,4000.00,AA
BALANCE,1005,111-22-1005,Hector,Coffey,07202002,5000.00,AA
BALANCE,1006,111-22-1006,Felix,Rasmussen,01012000,1000.00,BB
BALANCE,1007,111-22-1007,Haleigh,Hooper,12231983,2000.00,BB
BALANCE,1008,111-22-1008,Reynaldo,Bryan,05111987,3000.00,BB
BALANCE,1009,111-22-1009,Tamara,Robinson,02011967,4000.00,BB
BALANCE,1010,111-22-1010,Jakob,Kelley,04061999,5000.00,BB
BALANCE,1011,111-22-1011,Maggie,Washington,03301993,1000.00,CC
BALANCE,1012,111-22-1012,Albert,Romero,05051985,2000.00,CC
BALANCE,1013,111-22-1013,Makenna,Oconnell,10101990,3000.00,CC
BALANCE,1014,111-22-1014,Aarav,Terry,12311999,4000.00,CC
BALANCE,1015,111-22-1015,Maggie,Briggs,01012000,5000.00,CC
BALANCE,1016,111-22-1016,Frances,Parrish,02111997,1000.00,DD
BALANCE,1017,111-22-1017,Cody,Callahan,08301968,2000.00,DD
BALANCE,1018,111-22-1018,Malaki,Pena,01122000,3000.00,DD
BALANCE,1019,111-22-1019,Michaela,Arroyo,04141984,4000.00,DD
BALANCE,1020,111-22-1020,Noemi,Michael,11212000,5000.00,DD
TRAILER,60000.00,20
";

            return appState;
        }
    }
}
