using DataProcessor.Models;

namespace DataProcessor.Decoders
{
    public class IntegerDecoder : FieldDecoder
    {
        public override void Decode(Field field)
        {
            base.Decode(field);
            int value = default;
            var isValid = IsMatch && int.TryParse(field.Raw, out value);
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
