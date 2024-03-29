﻿using Polo.Abstractions.Commands;
using Polo.Abstractions.Exceptions;
using Polo.Abstractions.Options;
using Polo.Abstractions.Parameters;
using Polo.Extensions;

namespace Polo.Parameters
{
    public class WatermarkPathParameter : IParameter<string>
    {
        public string Name => "watermark-path";

        public IReadOnlyCollection<string> PossibleValues => new List<string> { @"""c:\sign\my_watermark.png""" };

        public string Description => "Watermark image full path.";

        public string Initialize(IReadOnlyDictionary<string, string> inputParameters, string defaultValue, IEnumerable<ICommand> commands = null!)
        {
            var outputValue = defaultValue;
            var parametersEmpty = inputParameters.IsNullOrEmpty();

            if (!parametersEmpty && inputParameters.TryGetValue(Name, out var parameterValue))
            {
                outputValue = parameterValue;
            }

            if (string.IsNullOrWhiteSpace(outputValue))
            {
                throw new ParameterAbsentException($"ERROR: Please provide '{CommandParser.ShortCommandPrefix}{Name}' parameter or setup setting value '{nameof(ApplicationSettingsReadOnly.WatermarkPath)}'.");
            }

            if (!File.Exists(outputValue))
            {
                throw new FileNotFoundException("ERROR: Watermark file not found.", outputValue);
            }

            return outputValue;
        }
    }
}