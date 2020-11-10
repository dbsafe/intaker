using System;

namespace DataProcessor.DataSource
{
    public class LineParser
    {
        public string Delimiter { get; set; } = ",";
        public bool HasFieldsEnclosedInQuotes { get; set; } = false;

        public string[] Parse(string line)
        {
            line = line.Trim();
            if (string.IsNullOrEmpty(line))
            {
                throw new InvalidOperationException("Line cannot be empty or null");
            }

            string[] delimiter;

            if (HasFieldsEnclosedInQuotes)
            {
                EnsureFirstAndLastQuotes(line);
                delimiter = new string[] { $"\"{Delimiter}\"" };
            }
            else
            {
                delimiter = new string[] { Delimiter };
            }

            var lines = line.Split(delimiter, StringSplitOptions.None);

            if (HasFieldsEnclosedInQuotes)
            {
                lines[0] = lines[0].Substring(1);
                var lastField = lines[lines.Length - 1];
                lines[lines.Length - 1] = lastField.Substring(0, lastField.Length - 1);
            }

            return lines;
        }

        private void EnsureFirstAndLastQuotes(string line)
        {
            if (line[0] != '"')
            {
                throw new FormatException("First field should start with quotes");
            }

            if (line[line.Length - 1] != '"')
            {
                throw new FormatException("Last field should end with quotes");
            }
        }
    }
}
