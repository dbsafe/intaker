using DataProcessor.Domain.Models;
using System;
using System.Globalization;

namespace DataProcessor.Decoders
{
    public class DateDecoder : FieldDecoder
    {
        public override void Decode(Field field)
        {
            EnsureThatPropertiesAreInitialized();
            var isValid = DateTime.TryParseExact(field.Raw, Pattern, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime value);
            if (isValid)
            {
                field.Value = value;
                field.ValidationResult = ValidationResultType.Valid;
            }
            else
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
