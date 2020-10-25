namespace DataProcessor.Domain.Models
{
    public enum ValidationResultType
    {
        /// <summary>
        /// Validation succeed.
        /// </summary>
        Valid = 1,
        
        /// <summary>
        /// Validation failed. Adds flexibility by treating some validation fails as warnings.
        /// </summary>
        Warning = 2,

        /// <summary>
        /// Validation failed.
        /// </summary>
        Error = 3,

        /// <summary>
        /// Validation failed. Causes the decoding process to abort.
        /// </summary>
        Critical = 4
    }
}
