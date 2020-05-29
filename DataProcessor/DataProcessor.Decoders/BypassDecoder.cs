using DataProcessor.Domain.Models;

namespace DataProcessor.Decoders
{
    public class BypassDecoder : FieldDecoder
    {
        public override void Decode(Field field)
        {
            field.ValidationResult = ValidationResultType.Valid;
        }
    }
}
