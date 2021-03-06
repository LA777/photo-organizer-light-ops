﻿using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Parameters;
using Polo.Extensions;
using System;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class ImageQuality : IParameter<int>, IParameterInfo
    {
        public static int Min => 0;// TODO LA - Combine min and max with Options validation
        public static int Max => 100;

        public static string Name => "image-quality";

        public static IReadOnlyCollection<int> PossibleValues => new List<int>() { Min, 42, Max };

        public static string Description => "Image quality.";

        public int Initialize(IReadOnlyDictionary<string, string> inputParameters, int defaultValue)
        {
            // TODO LA - Cover with UTs

            var outputValue = defaultValue;
            var parametersEmpty = inputParameters.IsNullOrEmpty();

            if (!parametersEmpty && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                if (int.TryParse(parameterValue, out var number))
                {
                    outputValue = number;
                }
                else
                {
                    throw new ParameterParseException($"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' is not a number.");
                }
            }

            if (outputValue < Min || outputValue > Max)
            {
                throw new ArgumentOutOfRangeException($"{CommandParser.ShortCommandPrefix}{Name}", $"ERROR: Parameter '{CommandParser.ShortCommandPrefix}{Name}' should be in the range from '{Min}' to '{Max}'.");
            }

            return outputValue;
        }
    }
}
