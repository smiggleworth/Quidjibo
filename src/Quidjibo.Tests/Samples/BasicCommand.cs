using Quidjibo.Attributes;
using Quidjibo.Commands;

namespace Quidjibo.Tests.Samples
{
    public class BasicCommand : IQuidjiboCommand
    {
    }

    [QueueName("custom-queue-name")]
    public class BasicWithAttributeCommand : IQuidjiboCommand
    {
    }
}