using Polo.Abstractions.Commands;

namespace Polo.Abstractions.Parameters.Handler
{
    public interface IParameterHandler
    {
        public IParameter<string> SourceParameter { get; init; }
        public IParameter<string> WatermarkPathParameter { get; init; }
        public IParameter<string> OutputFolderNameParameter { get; init; }
        public IParameter<string> PositionParameter { get; init; }
        public IParameter<int> TransparencyParameter { get; init; }
        public IParameter<int> LongSideLimitParameter { get; init; }
        public IParameter<float> MegaPixelsLimitParameter { get; init; }
        public IParameter<string> DestinationParameter { get; init; }
        public IParameter<int> ImageQualityParameter { get; init; }
        public IParameter<double> TimeDifferenceParameter { get; init; }
        public IParameter<ICommand> CommandParameter { get; init; }

        public string GetParametersDescription();

        public IReadOnlyCollection<IParameterInfo> GetParameters();
    }
}