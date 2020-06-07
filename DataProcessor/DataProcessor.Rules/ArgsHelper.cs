using System;

namespace DataProcessor.Rules
{
    public static class ArgsHelper
    {
        public static void EnsureDecodedArgs(string ruleName, string ruleDescription, string args, params object[] decodedArgProperties)
        {
            foreach (var decodedArgProperty in decodedArgProperties)
            {
                if (decodedArgProperty == null)
                {
                    throw new InvalidOperationException($"RuleName: {ruleName}, RuleDescription: {ruleDescription} - Invalid args [{args}]");
                }
            }
        }
    }
}
