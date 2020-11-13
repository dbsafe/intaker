using System.Diagnostics;

namespace DataProcessor.Utils
{
    public class DataProcessorGlobal
    {
        public static bool IsDebugEnabled { get; set; }
        public static void Debug(string message)
        {
            if (IsDebugEnabled)
            {
                Trace.WriteLine(message, "DataProcessor");
            }
        }
    }
}
