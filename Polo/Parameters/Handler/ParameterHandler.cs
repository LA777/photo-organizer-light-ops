using Polo.Abstractions.Commands;
using Polo.Abstractions.Parameters;
using Polo.Abstractions.Parameters.Handler;
using System.Text;

namespace Polo.Parameters.Handler
{
    public class ParameterHandler : IParameterHandler
    {
        public IParameter<int>? FsivThumbnailSizeParameter { get; init; }
        public IParameter<bool>? RecursiveParameter { get; init; }
        public IParameter<string> SourceParameter { get; init; } = null!;
        public IParameter<string>? WatermarkPathParameter { get; init; }
        public IParameter<string>? OutputFolderNameParameter { get; init; }
        public IParameter<string>? PositionParameter { get; init; }
        public IParameter<int>? TransparencyParameter { get; init; }
        public IParameter<int>? LongSideLimitParameter { get; init; }
        public IParameter<float>? MegaPixelsLimitParameter { get; init; }
        public IParameter<string>? DestinationParameter { get; init; }
        public IParameter<int>? ImageQualityParameter { get; init; }
        public IParameter<double>? TimeDifferenceParameter { get; init; }
        public IParameter<ICommand>? CommandParameter { get; init; }

        public string GetParametersDescription()
        {
            var parameters = GetParameters();
            var stringBuilder = new StringBuilder();

            foreach (var parameter in parameters)
            {
                if (parameter == null)
                {
                    continue;
                }

                var text = $"{CommandParser.ShortCommandPrefix}{parameter.Name}{CommandParser.ParameterDelimiter}{parameter.PossibleValues.First()} ";
                stringBuilder.AppendLine(text);
            }

            return stringBuilder.ToString();
        }

        public IReadOnlyCollection<IParameterInfo?> GetParameters()
        {
            var parameters = new List<IParameterInfo?>();

            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (SourceParameter != null)
            {
                parameters.Add(SourceParameter);
            }

            if (WatermarkPathParameter != null)
            {
                parameters.Add(WatermarkPathParameter);
            }

            if (OutputFolderNameParameter != null)
            {
                parameters.Add(OutputFolderNameParameter);
            }

            if (PositionParameter != null)
            {
                parameters.Add(PositionParameter);
            }

            if (TransparencyParameter != null)
            {
                parameters.Add(TransparencyParameter);
            }

            if (LongSideLimitParameter != null)
            {
                parameters.Add(LongSideLimitParameter);
            }

            if (MegaPixelsLimitParameter != null)
            {
                parameters.Add(MegaPixelsLimitParameter);
            }

            if (DestinationParameter != null)
            {
                parameters.Add(DestinationParameter);
            }

            if (ImageQualityParameter != null)
            {
                parameters.Add(ImageQualityParameter);
            }

            if (TimeDifferenceParameter != null)
            {
                parameters.Add(TimeDifferenceParameter);
            }

            if (RecursiveParameter != null)
            {
                parameters.Add(RecursiveParameter);
            }

            if (FsivThumbnailSizeParameter != null)
            {
                parameters.Add(FsivThumbnailSizeParameter);
            }

            if (CommandParameter != null)
            {
                parameters.Add(CommandParameter);
            }

            return parameters;
        }
    }
}