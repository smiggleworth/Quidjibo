using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Clients;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Exceptions;
using Quidjibo.Factories;
using Quidjibo.Misc;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;
using Quidjibo.Servers;

namespace Quidjibo
{
    public class QuidjiboBuilder
    {
        private bool _validated;
        private IQuidjiboConfiguration _configuration;
        private ICronProvider _cronProvider;
        private IWorkDispatcher _dispatcher;
        private ILoggerFactory _loggerFactory;
        private IWorkProviderFactory _workProviderFactory;
        private IProgressProviderFactory _progressProviderFactory;
        private IScheduleProviderFactory _scheduleProviderFactory;
        private IPayloadSerializer _serializer;
        private IPayloadProtector _protector;

        private IPipelineServiceProvider _provider = new PipelineServiceProvider();
        private readonly IList<PipelineStep> _steps = new List<PipelineStep>();

        public QuidjiboPipeline Build()
        {
            return new QuidjiboPipeline(_steps, _provider);
        }

        public QuidjiboBuilder Use(Func<IQuidjiboContext, Func<Task>, Task> middleware)
        {
            return Use(new PipelineMiddleware(middleware));
        }

        public QuidjiboBuilder Use<T>(T middleware = null) where T : class, IPipelineMiddleware
        {
            _steps.Add(new PipelineStep
            {
                Type = typeof(T),
                Instance = middleware
            });
            return this;
        }

        public QuidjiboBuilder ConfigurePipelineServiceProvider(IPipelineServiceProvider provider)
        {
            _provider = provider ?? new PipelineServiceProvider();
            return this;
        }

        /// <summary>
        ///     Build an instance of a QuidjiboServer as configured.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboServer BuildServer()
        {
            BackFillDefaults();
            var pipeline = Build();
            return new QuidjiboServer(_loggerFactory, _configuration, _workProviderFactory, _scheduleProviderFactory, _progressProviderFactory, _cronProvider, pipeline);
        }

        /// <summary>
        ///     Build the default QuidjiboClient. This is to support the typical use case.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboClient BuildClient()
        {
            BackFillDefaults();

            var client = new QuidjiboClient(_loggerFactory, _workProviderFactory, _scheduleProviderFactory, _serializer, _protector, _cronProvider);

            QuidjiboClient.Instance = client;

            return client;
        }

        /// <summary>
        ///     Build an keyed instance of the QuidjiboClient. This is used if you need to work with more than one Quidjibo
        ///     configuration.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboClient<TKey> BuildClient<TKey>()
            where TKey : IQuidjiboClientKey
        {
            BackFillDefaults();

            var client = new QuidjiboClient<TKey>(_loggerFactory, _workProviderFactory, _scheduleProviderFactory, _serializer, _protector, _cronProvider);

            QuidjiboClient<TKey>.Instance = client;

            return client;
        }

        /// <summary>
        ///     Apply a configurtion to the builder. Typically this is done in an extension method provided by the integration
        ///     implmentation
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public QuidjiboBuilder Configure(IQuidjiboConfiguration config)
        {
            _configuration = config;
            return this;
        }

        /// <summary>
        ///     Add the logger factory. (optional)
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureLogging(ILoggerFactory factory)
        {
            _loggerFactory = factory;
            return this;
        }

        /// <summary>
        /// </summary>
        /// <param name="assemblies">The assemblies that contain your QuidjiboHandlers</param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureDispatcher(params Assembly[] assemblies)
        {
            _dispatcher = new WorkDispatcher(new PayloadResolver(assemblies));
            return this;
        }

        /// <summary>
        ///     Configure Dispatcher with a custom resolver. Typically this is done in an extension method provided by the DI
        ///     framework implmentation
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureDispatcher(IPayloadResolver resolver)
        {
            _dispatcher = new WorkDispatcher(resolver);
            return this;
        }

        /// <summary>
        ///     Configure a custom serializer. (Optional)
        /// </summary>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureSerializer(IPayloadSerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        /// <summary>
        ///     Configure a custom protector. (Optional)
        /// </summary>
        /// <param name="protector"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureProtector(IPayloadProtector protector)
        {
            _protector = protector;
            return this;
        }

        /// <summary>
        ///     Configure a custom cron provider. (Optional)
        /// </summary>
        /// <param name="cronProvider"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureCron(ICronProvider cronProvider)
        {
            _cronProvider = cronProvider;
            return this;
        }

        /// <summary>
        ///     Configure a custome Progress Provider Factory. Typically this is done in an extension method provided by the
        ///     integration implmentation
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureProgressProviderFactory(IProgressProviderFactory factory)
        {
            _progressProviderFactory = factory;
            return this;
        }

        /// <summary>
        ///     Configure a custome Schedule Provider Factory. Typically this is done in an extension method provided by the
        ///     integration implmentation
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureScheduleProviderFactory(IScheduleProviderFactory factory)
        {
            _scheduleProviderFactory = factory;
            return this;
        }

        /// <summary>
        ///     Configure a custome Work Provider Factory. Typically this is done in an extension method provided by the
        ///     integration implmentation
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureWorkProviderFactory(IWorkProviderFactory factory)
        {
            _workProviderFactory = factory;
            return this;
        }

        private void BackFillDefaults()
        {
            if (_validated)
            {
                return;
            }
            _cronProvider = _cronProvider ?? new CronProvider();
            _dispatcher = _dispatcher ?? new WorkDispatcher(new PayloadResolver());
            _loggerFactory = _loggerFactory ?? new LoggerFactory();
            _serializer = _serializer ?? new PayloadSerializer();
            _protector = _protector ?? new PayloadProtector();
            Validate();
        }

        private void Validate()
        {
            var errors = new List<string>(3);

            if (_configuration == null)
            {
                errors.Add("Configuration is null");
            }
            if (_workProviderFactory == null)
            {
                errors.Add("Requires Work Provider Factory");
            }
            if (_progressProviderFactory == null)
            {
                errors.Add("Requires Progress Provider Factory");
            }
            if (_scheduleProviderFactory == null)
            {
                errors.Add("Requires Schedule Provider Factory");
            }

            if (errors.Any())
            {
                throw new QuidjiboBuilderException(errors, "Failed to validate. See list of errors for more detail.");
            }

            _validated = true;
        }
    }
}