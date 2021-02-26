using System.Collections.Generic;

namespace FileValidator.Domain.Services
{
    public class SampleFileOption
    {
        public int Id { get; }
        public string Name { get; }

        public SampleFileOption(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public class SampleFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
    }

    public interface ISampleFileStore
    {
        OperationResult<SampleFile> GetSampleFileById(int id);
        OperationResult<int> AddSampleFile(SampleFile sampleFile);
    }

    public class SampleFileStore : ISampleFileStore
    {
        private int _lastGeneratedId;
        private List<SampleFile> _sampleFiles = new List<SampleFile>();

        public OperationResult<int> AddSampleFile(SampleFile sampleFile)
        {
            var newSampleFile = new SampleFile
            {
                Id = ++_lastGeneratedId,
                Name = sampleFile.Name,
                Content = sampleFile.Content
            };

            _sampleFiles.Add(newSampleFile);

            return OperationResult<int>.CreateSucced(newSampleFile.Id);
        }

        public OperationResult<SampleFile> GetSampleFileById(int id)
        {
            if (id > _lastGeneratedId)
            {
                return OperationResult<SampleFile>.CreateFailed("File specification not found");
            }
            else
            {
                return OperationResult<SampleFile>.CreateSucced(_sampleFiles[id]);
            }
        }
    }
}
