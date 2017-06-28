using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private IQuidjiboConfiguration _configuration;
        private ICronProvider _cronProvider;
        private IWorkDispatcher _dispatcher;
        private ILoggerFactory _loggerFactory;
        private IProgressProviderFactory _progressProviderFactory;
        private IScheduleProviderFactory _scheduleProviderFactory;
        private IPayloadSerializer _serializer;
        private bool _validated;
        private IWorkProviderFactory _workProviderFactory;

        /// <summary>
        ///     Build an instance of a QuidjiboServer as configured.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboServer BuildServer()
        {
            BackFillDefaults();

            return new QuidjiboServer(
                _loggerFactory,
                _configuration,
                _workProviderFactory,
                _scheduleProviderFactory,
                _progressProviderFactory,
                _dispatcher,
                _serializer,
                _cronProvider);
        }

        /// <summary>
        ///     Build the default QuidjiboClient. This is to support the typical use case.
        /// </summary>
        /// <returns></returns>
        public IQuidjiboClient BuildClient()
        {
            BackFillDefaults();

            var client = new QuidjiboClient(_workProviderFactory, _scheduleProviderFactory, _serializer, _cronProvider);

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

            var client = new QuidjiboClient<TKey>(_workProviderFactory, _scheduleProviderFactory, _serializer, _cronProvider);

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
            _serializer = _serializer ?? new PayloadSerializer(new PayloadProtector());

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