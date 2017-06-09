using System.Reflection;
using Quidjibo.Attributes;
using Quidjibo.Commands;

namespace Quidjibo.Extensions
{
    public static class CommandExtensions
    {
        public static string GetQueueName(this IQuidjiboCommand command)
        {
            var attr = command.GetType().GetTypeInfo().GetCustomAttribute<QueueNameAttribute>();
            return attr == null ? "default" : attr.Name;
        }

        public static string GetQualifiedName(this IQuidjiboCommand command)
        {
            return command.GetType().AssemblyQualifiedName;
        }

        public static string GetName(this IQuidjiboCommand command)
        {
            return command.GetType().Name;
        }
    }
}