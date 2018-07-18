using System;
using System.Collections.Generic;
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

        public Guid? CorrelationId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}