using System.Reflection;
using Quidjibo.Attributes;
using Quidjibo.Commands;

namespace Quidjibo.Extensions
{
    public static class CommandExtensions
    {
        public static string GetQueueName(this IWorkCommand command)
        {
            var attr = command.GetType().GetTypeInfo().GetCustomAttribute<QueueNameAttribute>();
            return attr == null ? "default" : attr.Name;
        }

        public static string GetQualifiedName(this IWorkCommand command)
        {
            return command.GetType().AssemblyQualifiedName;
        }

        public static string GetName(this IWorkCommand command)
        {
            return command.GetType().Name;
        }
    }
}