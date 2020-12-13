using System.Collections.Generic;
using System.Linq;

namespace FileValidator.Domain.Services
{
    public class FileSpecificationOption
    {
        public int Id { get; }
        public string Name { get; }
        public string Description { get; }

        public FileSpecificationOption(int id, string name, string description)
        {
            Id = id;
            Name = name;
            Description = description;
        }
    }

    public class FileSpecification
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Content { get; set; }

        public FileSpecification Clone()
        {
            return new FileSpecification
            {
                Id = Id,
                Name = Name,
                Description = Description,
                Content = Content
            };
        }
    }

    public interface IFileSpecificationsStore
    {
        OperationResult<int> AddFileSpecification(FileSpecification fileSpecification);
        OperationResult UpdateFileSpecification(FileSpecification fileSpecification);
        OperationResult DeleteFileSpecification(int id);
        OperationResult<IEnumerable<FileSpecificationOption>> GetAllFileSpecificationOptions();
        OperationResult<FileSpecification> GetFileSpecificationById(int id);
    }

    public class FileSpecificationsStore : IFileSpecificationsStore
    {
        private int _lastGeneratedId;
        private readonly object _lockObj = new object();
        private readonly List<FileSpecification> _fileSpecifications = new List<FileSpecification>();

        public OperationResult<int> AddFileSpecification(FileSpecification fileSpecification)
        {
            if (string.IsNullOrWhiteSpace(fileSpecification.Name))
            {
                return OperationResult<int>.CreateFailed("Invalid Name");
            }

            var name = fileSpecification.Name.Trim();
            lock(_lockObj)
            {
                var fileWithSameNameFound = _fileSpecifications.Where(a => a.Name == name).Any();
                if (fileWithSameNameFound)
                {
                    return OperationResult<int>.CreateFailed($"A file spec with the same name already exists. Name: '{name}'");
                }

                var newFileSpecification = new FileSpecification
                {
                    Id = ++_lastGeneratedId,
                    Name = fileSpecification.Name,
                    Description = fileSpecification.Description,
                    Content = fileSpecification.Content
                };

                _fileSpecifications.Add(newFileSpecification);
                return OperationResult<int>.CreateSucced(newFileSpecification.Id);
            }            
        }

        public OperationResult DeleteFileSpecification(int id)
        {
            lock (_lockObj)
            {
                var fileSpecification = _fileSpecifications.Where(a => a.Id == id).FirstOrDefault();
                if (fileSpecification == null)
                {
                    return OperationResult.CreateFailed($"File spec not found. Id: '{id}'");
                }

                _fileSpecifications.Remove(fileSpecification);
                return OperationResult.CreateSucced();
            }
        }

        public OperationResult UpdateFileSpecification(FileSpecification fileSpecification)
        {
            if (string.IsNullOrWhiteSpace(fileSpecification.Name))
            {
                return OperationResult.CreateFailed("Invalid Name");
            }

            var name = fileSpecification.Name.Trim();

            lock (_lockObj)
            {
                var currentFileSpecification = _fileSpecifications.Where(a => a.Id == fileSpecification.Id).FirstOrDefault();
                if (currentFileSpecification == null)
                {
                    return OperationResult.CreateFailed($"File spec not found. Id: '{fileSpecification.Id}'");
                }

                _fileSpecifications.Remove(currentFileSpecification);                
                _fileSpecifications.Add(fileSpecification);
                return OperationResult.CreateSucced();
            }
        }

        public OperationResult<IEnumerable<FileSpecificationOption>> GetAllFileSpecificationOptions()
        {
            lock (_lockObj)
            {
                var data = _fileSpecifications.Select(a => new FileSpecificationOption(a.Id, a.Name, a.Description));
                return OperationResult<IEnumerable<FileSpecificationOption>>.CreateSucced(data);
            }
        }

        public OperationResult<FileSpecification> GetFileSpecificationById(int id)
        {
            FileSpecification data = null;
            lock (_lockObj)
            {
                data = _fileSpecifications.Where(a => a.Id == id).FirstOrDefault();
            }

            if (data == null)
            {
                return OperationResult<FileSpecification>.CreateFailed("File specification not found");
            }
            else
            {
                return OperationResult<FileSpecification>.CreateSucced(data);
            }
        }
    }
}
