using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Quidjibo.Builders;
using Quidjibo.Clients;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Exceptions;
using Quidjibo.Factories;
using Quidjibo.Misc;
using Quidjibo.Pipeline.Middleware;
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

        private bool _validated;

        private Assembly[] _assemblies;
        private ILoggerFactory _loggerFactory;
        private IQuidjiboConfiguration _configuration;
        private ICronProvider _cronProvider;
        private IWorkProviderFactory _workProviderFactory;
        private IProgressProviderFactory _progressProviderFactory;
        private IScheduleProviderFactory _scheduleProviderFactory;
        private IDependencyResolver _resolver;
        private IWorkDispatcher _dispatcher;
        private IPayloadSerializer _serializer;
        private IPayloadProtector _protector;

        /// <summary>
        ///     Build an instance of a QuidjiboServer as configured.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboServer BuildServer()
        {
            BackFillDefaults();
            var pipeline = _pipelineBuilder.Build();
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
            _loggerFactory = factory;
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

        public QuidjiboBuilder ConfigurePipeline(Action<IQuidjiboPipelineBuilder> pipelineBuilder)
        {
            return this;
        }


        private void BackFillDefaults()
        {
            if (_validated)
            {
                return;
            }
            _cronProvider = _cronProvider ?? new CronProvider();


            _dispatcher = _dispatcher ?? new WorkDispatcher(_resolver);
            _loggerFactory = _loggerFactory ?? new LoggerFactory();
            _serializer = _serializer ?? new PayloadSerializer();
            _protector = _protector ?? new PayloadProtector();

            _services.Add(typeof(ILoggerFactory), _loggerFactory);
            _services.Add(typeof(IPayloadProtector), _protector);
            _services.Add(typeof(IPayloadSerializer), _serializer);
            _services.Add(typeof(IWorkDispatcher), _dispatcher);
            _services.Add(typeof(QuidjiboHandlerMiddleware), new QuidjiboHandlerMiddleware(_loggerFactory, _dispatcher, _serializer, _protector));

            _resolver = _resolver ?? new DependencyResolver(_services, _assemblies);


            Validate();
        }

        private void Validate()
        {
            var errors = new List<string>(4);

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