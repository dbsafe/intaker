using DataProcessor.Domain.Models;

namespace DataProcessor.Decoders
{
    public class DecimalDecoder : FieldDecoder
    {
        public override void Decode(Field field)
        {
            base.Decode(field);
            decimal value = default;
            var isValid = IsMatch && decimal.TryParse(field.Raw, out value);
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
