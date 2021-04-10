using Polo.Abstractions.Parameters;
using System.Collections.Generic;

namespace Polo.Parameters.Handler
{
    public class ParameterHandler
    {
        public SourceParameter SourceParameter { get; set; } // TODO LA - Refactor - use init
        public WatermarkPathParameter WatermarkPathParameter { get; set; }
        public OutputFolderNameParameter OutputFolderNameParameter { get; set; }
        public PositionParameter PositionParameter { get; set; }
        public TransparencyParameter TransparencyParameter { get; set; }
        public LongSideLimitParameter LongSideLimitParameter { get; set; }

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
