using System;
using System.Collections.Generic;

namespace Quidjibo.Commands
{
    public interface IQuidjiboCommand
    {
        /// <summary>
        /// CorrelationId for tracking work through multiple applications. Work triggered by schedules will not use this CorrelationId.
        /// </summary>
        Guid? CorrelationId { get; set; }

        /// <summary>
        /// Metadata used for tracking unstructured information
        /// </summary>
        Dictionary<string, string> Metadata { get; set; }
    }
}