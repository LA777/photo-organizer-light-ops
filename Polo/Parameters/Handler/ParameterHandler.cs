using Polo.Abstractions.Commands;
using Polo.Abstractions.Parameters;
using Polo.Abstractions.Parameters.Handler;
using System.Text;

// ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
namespace Polo.Parameters.Handler
{
    public class ParameterHandler : IParameterHandler
    {
        public IParameter<string> ExtensionParameter { get; init; } = null!;
        public IParameter<int> FsivThumbnailSizeParameter { get; init; } = null!;
        public IParameter<bool> RecursiveParameter { get; init; } = null!;
        public IParameter<string> SourceParameter { get; init; } = null!;
        public IParameter<string> WatermarkPathParameter { get; init; } = null!;
        public IParameter<string> OutputFolderNameParameter { get; init; } = null!;
        public IParameter<string> PositionParameter { get; init; } = null!;
        public IParameter<int> TransparencyParameter { get; init; } = null!;
        public IParameter<int> LongSideLimitParameter { get; init; } = null!;
        public IParameter<float> MegaPixelsLimitParameter { get; init; } = null!;
        public IParameter<string> DestinationParameter { get; init; } = null!;
        public IParameter<int> ImageQualityParameter { get; init; } = null!;
        public IParameter<double> TimeDifferenceParameter { get; init; } = null!;
        public IParameter<ICommand> CommandParameter { get; init; } = null!;

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

            if (ExtensionParameter != null)
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