using System;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Quidjibo.Attributes;
using Quidjibo.Clients;
using Quidjibo.Commands;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Factories;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.Servers;

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

    public static class QuidjiboBuilderExtensions
    {
        public static QuidjiboBuilder UseAesProtector(this QuidjiboBuilder builder, Func<byte[]> aesKey)
        {
            builder.PayloadProtector = new AesPayloadProtector(aesKey());
            return builder;
        }

    }


    public class QuidjiboBuilder
    {
        public IPayloadProtector PayloadProtector { get; set; }
        public ICronProvider CronProvider { get; set; }
        public IWorkDispatcher Dispatcher { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }
        public IProgressProviderFactory ProgressProviderFactory { get; set; }
        public IScheduleProviderFactory ScheduleProviderFactory { get; set; }
        public IPayloadSerializer Serializer { get; set; }
        public IWorkConfiguration WorkConfiguration { get; set; }
        public IWorkProviderFactory WorkProviderFactory { get; set; }

        public IQuidjiboServer BuildServer()
        {

            return default(QuidjiboServer);
        }

        public IQuidjiboClient BuildClients()
        {
            return null;
        }
    }
}