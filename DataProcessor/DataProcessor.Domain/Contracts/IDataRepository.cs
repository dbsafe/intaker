using DataProcessor.Domain.Models;
using System.Collections.Generic;

namespace DataProcessor.Domain.Contracts
{
    public interface IDataRepository
    {
        InitializeFileResult InitializeFile(InitializeFileRequest request);

        void SetFileStatusWithWileLoadError(SetFileStatusWithFileLoadErrorRequest request);

        void InsertHeader(InsertRowRequest request);
        void InsertData(InsertRowRequest request);
        void InsertTrailer(InsertRowRequest request);
        void SetFileStatusWithFileLoadedCompleted(SetFileStatusWithFileLoadedCompletedRequest request);
    }

    public class SetFileStatusWithFileLoadedCompletedRequest
    {
        public long FileID { get; set; }
        public IList<string> Errors { get; set; }
        public ValidationResultType ValidationResult { get; set; }
    }

    public class SetFileStatusWithFileLoadErrorRequest
    {
        public long FileID { get; set; }
        public string Error { get; set; }
    }

    public class InsertRowRequest
    {
        public long FileID { get; set; }
        public ValidationResultType? ValidationResult { get; set; }
        public string Raw { get; set; }
        public string Decoded { get; set; }
        public IList<string> Errors { get; set; }
    }

    public class InitializeFileResult
    {
        public long FileID { get; set; }
    }

    public class InitializeFileRequest
    {
        public string Path { get; set; }
    }
}
