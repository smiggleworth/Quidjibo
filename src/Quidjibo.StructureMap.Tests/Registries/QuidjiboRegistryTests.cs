using System;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Clients;
using Quidjibo.Factories;
using Quidjibo.Handlers;
using Quidjibo.Protectors;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.StructureMap.Registries;
using Quidjibo.StructureMap.Tests.Samples;
using StructureMap;

namespace Quidjibo.StructureMap.Tests.Registries
{
    [TestClass]
    public class QuidjiboRegistryTests
    {
        private readonly IContainer _container;

        public QuidjiboRegistryTests()
        {
            var registry = new Registry();
            registry.IncludeRegistry<QuidjiboRegistry>();

            _container = new Container(registry);
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_GetInstance()
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                var handler = nestedContainer.GetInstance<IQuidjiboHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_GetInstance()
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                var handler = nestedContainer.GetInstance<IQuidjiboHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                Action resolve = () => nestedContainer.GetInstance<IQuidjiboHandler<UnhandledCommand>>();
                resolve.ShouldThrow<StructureMapConfigurationException>("Handler was not registerd");
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

            using (var nestedContainer = _container.GetNestedContainer())
            {
                var client = nestedContainer.GetInstance<IQuidjiboClient>();
                client.Should().NotBeNull();
                client.Should().BeAssignableTo<IQuidjiboClient>();
                client.Should().Be(QuidjiboClient.Instance);
            }
        }
    }
}