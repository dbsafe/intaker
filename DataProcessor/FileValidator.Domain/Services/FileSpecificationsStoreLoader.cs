using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FileValidator.Domain.Services
{
    public static class SampleFileStoreLoader
    {
        private const string SAMPLE_FILES_LOCATION = "sample-files";

        public static async Task LoadFileSpecsAsync(FileSpecificationsStore store, HttpClient httpClient)
        {
            var fileSpecConfigs = await httpClient.GetFromJsonAsync<FileSpecConfig[]>($"{SAMPLE_FILES_LOCATION}/file-specs-config.json");
            foreach (var fileSpecConfig in fileSpecConfigs)
            {
                await LoadConfiguredFileSpecAsync(fileSpecConfig, store, httpClient);
            }
        }

        public static async Task LoadSampleFileAsync(SampleFileStore store, HttpClient httpClient)
        {
            var sampleFileConfigs = await httpClient.GetFromJsonAsync<SampleFileConfig[]>($"{SAMPLE_FILES_LOCATION}/sample-files-config.json");
            foreach (var sampleFileConfig in sampleFileConfigs)
            {
                await LoadConfiguredSampleFileAsync(sampleFileConfig, store, httpClient);
            }
        }

        private static async Task LoadConfiguredSampleFileAsync(SampleFileConfig sampleFileConfig, SampleFileStore store, HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync($"{SAMPLE_FILES_LOCATION}/{sampleFileConfig.Path}");
            var fileSpecification = new SampleFile { Name = sampleFileConfig.Name, Content = content };
            store.AddSampleFile(fileSpecification);
        }

        private static async Task LoadConfiguredFileSpecAsync(FileSpecConfig fileSpecConfig, FileSpecificationsStore store, HttpClient httpClient)
        {
            var content = await httpClient.GetStringAsync($"{SAMPLE_FILES_LOCATION}/{fileSpecConfig.Path}");
            var fileSpecification = new FileSpecification { Name = fileSpecConfig.Name, Description = fileSpecConfig.Description, Content = content };
            store.AddFileSpecification(fileSpecification);
        }

        private class FileSpecConfig
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Path { get; set; }
        }

        private class SampleFileConfig
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
}
