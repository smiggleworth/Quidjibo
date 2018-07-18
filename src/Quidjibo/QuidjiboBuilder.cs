using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Quidjibo.Clients;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Exceptions;
using Quidjibo.Factories;
using Quidjibo.Pipeline.Builders;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;
using Quidjibo.Servers;

namespace Quidjibo
{
    public class QuidjiboBuilder
    {
        private readonly IQuidjiboPipelineBuilder _pipelineBuilder = new QuidjiboPipelineBuilder();
        private readonly IDictionary<Type, object> _services = new Dictionary<Type, object>();

        private Assembly[] _assemblies;

        private IQuidjiboConfiguration _configuration;
        private ICronProvider _cronProvider;
        private IWorkDispatcher _dispatcher;
        private IPayloadProtector _protector;
        private IDependencyResolver _resolver;
        private IPayloadSerializer _serializer;

        private bool _validated;

        public ILoggerFactory LoggerFactory { get; private set; }
        public IWorkProviderFactory WorkProviderFactory { get; private set; } = new WorkProviderFactory();
        public IProgressProviderFactory ProgressProviderFactory { get; private set; } = new ProgressProviderFactory();
        public IScheduleProviderFactory ScheduleProviderFactory { get; private set; } = new ScheduleProviderFactory();

        /// <summary>
        ///     Build an instance of a QuidjiboServer as configured.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboServer BuildServer()
        {
            BackFillDefaults();
            var pipeline = _pipelineBuilder.Build(LoggerFactory, _resolver, _protector, _serializer, _dispatcher);
            return new QuidjiboServer(LoggerFactory, _configuration, WorkProviderFactory, ScheduleProviderFactory, ProgressProviderFactory, _cronProvider, pipeline);
        }

        /// <summary>
        ///     Build the default QuidjiboClient. This is to support the typical use case.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboClient BuildClient()
        {
            BackFillDefaults();

            var client = new QuidjiboClient(LoggerFactory, WorkProviderFactory, ScheduleProviderFactory, _serializer, _protector, _cronProvider);

            QuidjiboClient.Instance = client;

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

        public QuidjiboBuilder ConfigureAssemblies(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
            return this;
        }

        public QuidjiboBuilder ConfigureDependency(Type type, object instance)
        {
            _services.Add(type, instance);
            return this;
        }

        /// <summary>
        ///     Add the logger factory. (optional)
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureLogging(ILoggerFactory factory)
        {
            LoggerFactory = factory;
            return this;
        }

        /// <summary>
        ///     Configure the resolver. Typically this is done in an extension method provided by the DI
        ///     framework implmentation
        /// </summary>
        /// <param name="resolver"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureResolver(IDependencyResolver resolver)
        {
            _resolver = resolver;
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
        ///     Configure a custom Progress WorkProvider Factory. Typically this is done in an extension method provided by the
        ///     integration implmentation
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureProgressProviderFactory(IProgressProviderFactory factory)
        {
            ProgressProviderFactory = factory;
            return this;
        }

        /// <summary>
        ///     Configure a custom Schedule WorkProvider Factory. Typically this is done in an extension method provided by the
        ///     integration implmentation
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureScheduleProviderFactory(IScheduleProviderFactory factory)
        {
            ScheduleProviderFactory = factory;
            return this;
        }

        /// <summary>
        ///     Configure a custom Work WorkProvider Factory. Typically this is done in an extension method provided by the
        ///     integration implmentation
        /// </summary>
        /// <param name="factory"></param>
        /// <returns></returns>
        public QuidjiboBuilder ConfigureWorkProviderFactory(IWorkProviderFactory factory)
        {
            WorkProviderFactory = factory;
            return this;
        }

        public QuidjiboBuilder ConfigurePipeline(Action<IQuidjiboPipelineBuilder> pipeline)
        {
            pipeline.Invoke(_pipelineBuilder);
            return this;
        }

        private void BackFillDefaults()
        {
            if (_validated)
            {
                return;
            }

            LoggerFactory = LoggerFactory ?? new LoggerFactory();
            _serializer = _serializer ?? new PayloadSerializer();
            _protector = _protector ?? new PayloadProtector();
            _cronProvider = _cronProvider ?? new CronProvider();
            _resolver = _resolver ?? new DependencyResolver(_services, GetConfiguredAssemblies());
            _dispatcher = _dispatcher ?? new WorkDispatcher(_resolver);


            _services.Add(typeof(ILoggerFactory), LoggerFactory);
            _services.Add(typeof(IPayloadProtector), _protector);
            _services.Add(typeof(IPayloadSerializer), _serializer);
            _services.Add(typeof(IWorkDispatcher), _dispatcher);

            Validate();

            var logger = LoggerFactory.CreateLogger<QuidjiboBuilder>();
            logger.LogDebug("Work Factory : {0}", WorkProviderFactory.GetType().Name);
            logger.LogDebug("Schedule Factory : {0}", ScheduleProviderFactory.GetType().Name);
            logger.LogDebug("Progress Factory : {0}", ProgressProviderFactory.GetType().Name);
            logger.LogDebug("Serializer : {0}", _serializer.GetType().Name);
            logger.LogDebug("Protector : {0}", _protector.GetType().Name);
            logger.LogDebug("Cron : {0}", _cronProvider.GetType().Name);
            logger.LogDebug("Resolver: {0}", _resolver.GetType().Name);
            logger.LogDebug("Dispatcher: {0}", _dispatcher.GetType().Name);
        }

        private void Validate()
        {
            var errors = new List<string>(4);

            if (_configuration == null)
            {
                errors.Add("Configuration is null");
            }

            if (WorkProviderFactory == null)
            {
                errors.Add("Requires Work Provider Factory");
            }

            if (ProgressProviderFactory == null)
            {
                errors.Add("Requires Progress Provider Factory");
            }

            if ((_configuration?.EnableScheduler ?? true) && ScheduleProviderFactory == null)
            {
                errors.Add("Requires Schedule Provider Factory");
            }

            if (errors.Any())
            {
                throw new QuidjiboBuilderException(errors, "Failed to validate. See list of errors for more detail.");
            }

            _validated = true;
        }

        private Assembly[] GetConfiguredAssemblies()
        {
            if (_assemblies == null || _assemblies.Length < 1)
            {
                // if the worker isn't running we don't care if assemblies are configured. 
                return !_configuration.EnableWorker ? new[] { GetType().Assembly } : null;
            }

            return _assemblies;
        }
    }
}