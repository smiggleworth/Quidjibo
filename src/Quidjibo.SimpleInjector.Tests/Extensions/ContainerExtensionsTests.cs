using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Clients;
using Quidjibo.Factories;
using Quidjibo.Handlers;
using Quidjibo.Providers;
using Quidjibo.Serializers;
using Quidjibo.SimpleInjector.Extensions;
using Quidjibo.SimpleInjector.Tests.Samples;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace Quidjibo.SimpleInjector.Tests.Extensions
{
    [TestClass]
    public class ContainerExtensionsTests
    {
        private readonly Container _container;

        public ContainerExtensionsTests()
        {
            _container = new Container();
            _container.RegisterQuidjibo(typeof(ContainerExtensionsTests).GetTypeInfo().Assembly);
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                var handler = _container.GetInstance<IQuidjiboHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                var handler = _container.GetInstance<IQuidjiboHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                Action resolve = () => _container.GetInstance<IQuidjiboHandler<UnhandledCommand>>();
                resolve.ShouldThrow<ActivationException>("Handler was not registerd");
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
                Substitute.For<ICronProvider>());

            QuidjiboClient<CustomClientKey1>.Instance = new QuidjiboClient<CustomClientKey1>(
                Substitute.For<ILoggerFactory>(),
                Substitute.For<IWorkProviderFactory>(),
                Substitute.For<IScheduleProviderFactory>(),
                Substitute.For<IPayloadSerializer>(),
                Substitute.For<ICronProvider>());

            QuidjiboClient<CustomClientKey2>.Instance = new QuidjiboClient<CustomClientKey2>(
                Substitute.For<ILoggerFactory>(),
                Substitute.For<IWorkProviderFactory>(),
                Substitute.For<IScheduleProviderFactory>(),
                Substitute.For<IPayloadSerializer>(),
                Substitute.For<ICronProvider>());

            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                var client = _container.GetInstance<IQuidjiboClient>();
                client.Should().NotBeNull();
                client.Should().BeAssignableTo<IQuidjiboClient>();
                client.Should().Be(QuidjiboClient.Instance);

                var client1 = _container.GetInstance<IQuidjiboClient<CustomClientKey1>>();
                client1.Should().NotBeNull();
                client1.Should().BeAssignableTo<IQuidjiboClient<CustomClientKey1>>();
                client1.Should().Be(QuidjiboClient<CustomClientKey1>.Instance);

                var client2 = _container.GetInstance<IQuidjiboClient<CustomClientKey2>>();
                client2.Should().NotBeNull();
                client2.Should().BeAssignableTo<IQuidjiboClient<CustomClientKey2>>();
                client2.Should().Be(QuidjiboClient<CustomClientKey2>.Instance);
            }
        }
    }
}