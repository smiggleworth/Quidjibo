using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Clients;
using Quidjibo.DependencyInjection.Extensions;
using Quidjibo.DependencyInjection.Tests.Samples;
using Quidjibo.Factories;
using Quidjibo.Handlers;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Serializers;

namespace Quidjibo.DependencyInjection.Tests.Extensions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceCollectionExtensionsTests()
        {
            var services = new ServiceCollection();
            services.AddQuidjibo(typeof(ServiceCollectionExtensionsTests).GetTypeInfo().Assembly);
            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IQuidjiboHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IQuidjiboHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IQuidjiboHandler<UnhandledCommand>>();
                handler.Should().BeNull("handler was not registerd");
            }
        }

        [TestMethod]
        public void RegisterQuidjiboClientTest()
        {
            QuidjiboClient.Instance = new QuidjiboClient(
                Substitute.For<ILoggerFactory>(),
                Substitute.For<IWorkProviderFactory>(),
                Substitute.For<IScheduleProviderFactory>(),
                Substitute.For<IPayloadSerializer>(),
                Substitute.For<IPayloadProtector>(),
                Substitute.For<ICronProvider>());

            using (var scope = _serviceProvider.CreateScope())
            {
                var client = scope.ServiceProvider.GetService<IQuidjiboClient>();
                client.Should().NotBeNull();
                client.Should().BeAssignableTo<IQuidjiboClient>();
                client.Should().Be(QuidjiboClient.Instance);
            }
        }
    }
}