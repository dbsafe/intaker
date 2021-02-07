using System;

namespace FileValidator.Blazor.Formatters
{
    public class DateTimeFormatter : IFormatter
    {
        public string Format(object value, string format)
        {
            if (value is not DateTime)
            {
                throw new InvalidOperationException($"{nameof(DateTimeFormatter)} - Value must be a {nameof(DateTime)}");
            }

            if (format is null)
            {
                throw new ArgumentNullException($"{nameof(DateTimeFormatter)} - Parameter {nameof(format)} is required");
            }

            var dateTime = Convert.ToDateTime(value);
            return dateTime.ToString(format);
        }
    }
}
