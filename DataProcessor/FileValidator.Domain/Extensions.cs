using Newtonsoft.Json;

namespace FileValidator.Domain
{
    public static class Extensions
    {
        public static string ToJson<T>(this T source)
        {
            return JsonConvert.SerializeObject(source, Formatting.Indented);
        }
    }
}
