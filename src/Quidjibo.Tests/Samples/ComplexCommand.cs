using System;
using Quidjibo.Commands;

namespace Quidjibo.Tests.Samples
{
    public class ComplexCommand : IQuidjiboCommand
    {
        public Guid Id { get; }

        public ModelData Data { get; }

        public ComplexCommand(Guid id, ModelData data)
        {
            Id = id;
            Data = data;
        }
    }
}