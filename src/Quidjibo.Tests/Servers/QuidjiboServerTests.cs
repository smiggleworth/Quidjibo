using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Configurations;
using Quidjibo.Dispatchers;
using Quidjibo.Factories;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.Servers;

namespace Quidjibo.Tests.Servers
{
    [TestClass]
    public class QuidjiboServerTests
    {
        private QuidjiboServer _sut;

        private ICronProvider _cronProvider;
        private IWorkDispatcher _dispatcher;
        private ILogger _logger;
        private ILoggerFactory _loggerFactory;
        private IProgressProviderFactory _progressProviderFactory;
        private IQuidjiboConfiguration _quidjiboConfiguration;
        private IScheduleProviderFactory _scheduleProviderFactory;
        private IPayloadSerializer _serializer;
        private IWorkProviderFactory _workProviderFactory;

        [TestInitialize]
        public void Init()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _quidjiboConfiguration = Substitute.For<IQuidjiboConfiguration>();
            _workProviderFactory = Substitute.For<IWorkProviderFactory>();
            _scheduleProviderFactory = Substitute.For<IScheduleProviderFactory>();
            _progressProviderFactory = Substitute.For<IProgressProviderFactory>();
            _dispatcher = Substitute.For<IWorkDispatcher>();
            _serializer = Substitute.For<IPayloadSerializer>();
            _cronProvider = Substitute.For<ICronProvider>();

            _sut = new QuidjiboServer(
                _loggerFactory,
                _quidjiboConfiguration,
                _workProviderFactory,
                _scheduleProviderFactory,
                _progressProviderFactory,
                _dispatcher,
                _serializer,
                _cronProvider);
        }

    }
}
