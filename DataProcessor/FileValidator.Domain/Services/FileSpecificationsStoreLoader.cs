using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FileValidator.Domain.Services
{
    public static class FileSpecificationsStoreLoader
    {
        public static async Task LoadAsync(FileSpecificationsStore store, HttpClient httpClient)
        {
            var fileSpecConfigs = await httpClient.GetFromJsonAsync<FileSpecConfig[]>("file-specs/file-specs-config.json");
            foreach (var fileSpecConfig in fileSpecConfigs)
            {
                await LoadConfiguredFileSpecAsync(fileSpecConfig, store, httpClient);
            }
        }

        private static async Task LoadConfiguredFileSpecAsync(FileSpecConfig fileSpecConfig, FileSpecificationsStore store, HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync($"file-specs/{fileSpecConfig.Path}");
            var fileSpecification = new FileSpecification { Name = fileSpecConfig.Name, Description = fileSpecConfig.Description, Content = content };
            store.AddFileSpecification(fileSpecification);
        }

        private class FileSpecConfig
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Path { get; set; }
        }
    }
}
