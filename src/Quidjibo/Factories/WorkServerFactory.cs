using System.Reflection;
using Microsoft.Extensions.Logging;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;
using Quidjibo.Servers;

namespace Quidjibo.Factories
{
    public class WorkServerFactory
    {
        public static IQuidjiboServer Create(
            Assembly assembly,
            IQuidjiboConfiguration configuration,
            IWorkProviderFactory workProviderFactory,
            IScheduleProviderFactory scheduleProviderFactory,
            IProgressProviderFactory progressProviderFactory,
            ILoggerFactory loggerFactory = null)
        {
            var assemblies = new[]
            {
                assembly
            };
            return Create(assemblies, configuration, workProviderFactory, scheduleProviderFactory, progressProviderFactory, loggerFactory);
        }

        public static IQuidjiboServer Create(
            Assembly[] assemblies,
            IQuidjiboConfiguration configuration,
            IWorkProviderFactory workProviderFactory,
            IScheduleProviderFactory scheduleProviderFactory,
            IProgressProviderFactory progressProviderFactory,
            ILoggerFactory loggerFactory = null)
        {
            var resolver = new PayloadResolver(assemblies);
            var protector = new PayloadProtector();
            return Create(resolver, protector, configuration, workProviderFactory, scheduleProviderFactory, progressProviderFactory, loggerFactory);
        }

        public static IQuidjiboServer Create(
            IPayloadResolver resolver,
            IPayloadProtector protector,
            IQuidjiboConfiguration configuration,
            IWorkProviderFactory workProviderFactory,
            IScheduleProviderFactory scheduleProviderFactory,
            IProgressProviderFactory progressProviderFactory,
            ILoggerFactory loggerFactory = null)
        {
            loggerFactory = loggerFactory ?? new LoggerFactory();
            var dispatcher = new WorkDispatcher(resolver);
            var serializer = new PayloadSerializer(protector);
            var cronProvider = new CronProvider();
            return new QuidjiboServer(loggerFactory, configuration, workProviderFactory, scheduleProviderFactory, progressProviderFactory, dispatcher, serializer, cronProvider);
        }
    }
}