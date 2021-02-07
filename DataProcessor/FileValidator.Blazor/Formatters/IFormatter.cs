namespace FileValidator.Blazor.Formatters
{
    public interface IFormatter
    {
        string Format(object value, string format = null);
    }
}
