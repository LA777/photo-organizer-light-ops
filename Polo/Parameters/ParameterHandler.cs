using Polo.Abstractions.Parameters;
using System.Collections.Generic;

namespace Polo.Parameters
{
    public class ParameterHandler
    {
        public SourceParameter SourceParameter { get; set; } = null; // TODO LA - Refactor - use init
        public WatermarkPathParameter WatermarkPathParameter { get; set; } = null;
        public OutputFolderNameParameter OutputFolderNameParameter { get; set; } = null;
        public PositionParameter PositionParameter { get; set; } = null;
        public TransparencyParameter TransparencyParameter { get; set; } = null;
        public LongSideLimitParameter LongSideLimitParameter { get; set; } = null;

        public IReadOnlyCollection<IParameterInfo> GetStringParameters()
        {
            var parameters = new List<IParameterInfo>();

            if (SourceParameter != null) parameters.Add(SourceParameter);
            if (WatermarkPathParameter != null) parameters.Add(WatermarkPathParameter);
            if (OutputFolderNameParameter != null) parameters.Add(OutputFolderNameParameter);
            if (PositionParameter != null) parameters.Add(PositionParameter);
            if (TransparencyParameter != null) parameters.Add(TransparencyParameter);
            if (LongSideLimitParameter != null) parameters.Add(LongSideLimitParameter);

            return parameters;
        }
    }
}
