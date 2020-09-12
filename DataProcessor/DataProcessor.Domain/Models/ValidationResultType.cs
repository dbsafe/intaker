namespace DataProcessor.Domain.Models
{
    public enum ValidationResultType
    {
        Valid = 1,
        InvalidWarning = 2,
        InvalidError = 3,
        InvalidCritical = 4
    }
}
