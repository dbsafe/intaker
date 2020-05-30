using DataProcessor.Domain.Models;

namespace DataProcessor.Decoders
{
    public class TextDecoder : FieldDecoder
    {
        public override void Decode(Field field)
        {
            base.Decode(field);
            if (IsMatch)
            {
                field.Value = field.Raw;
                field.ValidationResult = ValidationResultType.Valid;
            }
            else
            {
                field.ValidationResult = FailValidationResult;
            }
        }
    }
}
