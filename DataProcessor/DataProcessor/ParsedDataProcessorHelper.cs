﻿using DataProcessor.Domain.Models;
using System;

namespace DataProcessor
{
    public static class ParsedDataProcessorHelper
    {
        public static ValidationResultType GetMaxValidationResult(ValidationResultType value1, ValidationResultType value2)
        {
            switch (value1)
            {
                case ValidationResultType.Valid:
                    return value2 == ValidationResultType.Valid ? ValidationResultType.Valid : value2;
                case ValidationResultType.Warning:
                    return value2 == ValidationResultType.Critical ? ValidationResultType.Critical : ValidationResultType.Warning;
                case ValidationResultType.Critical:
                    return ValidationResultType.Critical;
                default:
                    throw new InvalidOperationException($"Invalid {nameof(ValidationResultType)} {value1}");
            }
        }
    }
}
