namespace DataProcessor.Domain.Contracts
{
    public interface IDataRepository
    {
        InitializeFileResult InitializeFile(InitializeFileRequest request);
    }

    public class InitializeFileResult
    {

    }

    public class InitializeFileRequest
    {

    }
}
