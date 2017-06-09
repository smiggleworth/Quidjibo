using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Quidjibo.Clients;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Exceptions;
using Quidjibo.Factories;
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
        private IWorkProviderFactory _workProviderFactory;

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

        public IQuidjiboClient BuildClient()
        {
            BackFillDefaults();

            var client = new QuidjiboClient(_workProviderFactory, _scheduleProviderFactory, _serializer, _cronProvider);

            QuidjiboClient.Instance = client;
            return client;
        }

        public QuidjiboBuilder Configure(IQuidjiboConfiguration config)
        {
            _configuration = config;
            return this;
        }

        public QuidjiboBuilder ConfigureLogging(ILoggerFactory factory)
        {
            _loggerFactory = factory;
            return this;
        }

        public QuidjiboBuilder ConfigureDispatcher(params Assembly[] assemblies)
        {
            _dispatcher = new WorkDispatcher(new PayloadResolver(assemblies));
            return this;
        }

        public QuidjiboBuilder ConfigureDispatcher(IPayloadResolver resolver)
        {
            _dispatcher = new WorkDispatcher(resolver);
            return this;
        }

        public QuidjiboBuilder ConfigureSerializer(IPayloadSerializer serializer)
        {
            _serializer = serializer;
            return this;
        }

        public QuidjiboBuilder ConfigureCron(ICronProvider cronProvider)
        {
            _cronProvider = cronProvider;
            return this;
        }

        public QuidjiboBuilder ConfigureProgressProviderFactory(IProgressProviderFactory factory)
        {
            _progressProviderFactory = factory;
            return this;
        }

        public QuidjiboBuilder ConfigureScheduleProviderFactory(IScheduleProviderFactory factory)
        {
            _scheduleProviderFactory = factory;
            return this;
        }

        public QuidjiboBuilder ConfigureWorkProviderFactory(IWorkProviderFactory factory)
        {
            _workProviderFactory = factory;
            return this;
        }

        private void BackFillDefaults()
        {
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
                errors.Add("");
            }
            if (_progressProviderFactory == null)
            {
                errors.Add("");
            }
            if (_scheduleProviderFactory == null)
            {
                errors.Add("");
            }

            if (errors.Any())
            {
                throw new QuidjiboBuilderException(errors, "Validation Failed");
            }
        }
    }
}