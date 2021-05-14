using Polo.Abstractions.Parameters;
using System.Collections.Generic;

namespace Polo.Parameters.Handler
{
    public class ParameterHandler
    {
        public SourceParameter SourceParameter { get; init; }
        public WatermarkPathParameter WatermarkPathParameter { get; init; }
        public OutputFolderNameParameter OutputFolderNameParameter { get; init; }
        public PositionParameter PositionParameter { get; init; }
        public TransparencyParameter TransparencyParameter { get; init; }
        public LongSideLimitParameter LongSideLimitParameter { get; init; }
        public MegaPixelsLimitParameter MegaPixelsLimitParameter { get; init; }
        public DestinationParameter DestinationParameter { get; init; }
        public ImageQuality ImageQuality { get; init; }

        public IReadOnlyCollection<IParameterInfo> GetStringParameters()
        {
            var parameters = new List<IParameterInfo>();

            if (SourceParameter != null) parameters.Add(SourceParameter);
            if (WatermarkPathParameter != null) parameters.Add(WatermarkPathParameter);
            if (OutputFolderNameParameter != null) parameters.Add(OutputFolderNameParameter);
            if (PositionParameter != null) parameters.Add(PositionParameter);
            if (TransparencyParameter != null) parameters.Add(TransparencyParameter);
            if (LongSideLimitParameter != null) parameters.Add(LongSideLimitParameter);
            if (MegaPixelsLimitParameter != null) parameters.Add(MegaPixelsLimitParameter);
            if (DestinationParameter != null) parameters.Add(DestinationParameter);
            if (ImageQuality != null) parameters.Add(ImageQuality);

            return parameters;
        }
    }
}
