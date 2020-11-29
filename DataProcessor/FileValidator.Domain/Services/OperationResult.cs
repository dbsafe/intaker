namespace FileValidator.Domain.Services
{
    public interface IOperationResult
    {
        bool Succeed { get; }
        string Message { get; }
    }

    public class OperationResult : IOperationResult
    {
        public bool Succeed { get; private set; }
        public string Message { get; private set; }

        public static OperationResult CreateSucced() => new OperationResult { Succeed = true };
        public static OperationResult CreateFailed(string message) => new OperationResult { Message = message };
    }

    public class OperationResult<TData> : IOperationResult
    {
        public bool Succeed { get; private set; }
        public string Message { get; private set; }

        public TData Data { get; private set; }

        public static OperationResult<TData> CreateSucced(TData data) => new OperationResult<TData> { Succeed = true, Data = data };
        public static OperationResult<TData> CreateFailed(string message) => new OperationResult<TData> { Message = message };
    }

}
