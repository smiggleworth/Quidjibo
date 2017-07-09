using Quidjibo.Commands;

namespace Quidjibo.Tests.Samples
{
    public class ComplexCommand : IQuidjiboCommand
    {
        public ModelData ModelModelData { get; }

        public ComplexCommand(ModelData modelData)
        {
            ModelModelData = modelData;
        }
    }
}