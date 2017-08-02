using System;
using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Autofac.Modules;
using Quidjibo.Autofac.Tests.Samples;
using Quidjibo.Clients;
using Quidjibo.Factories;
using Quidjibo.Handlers;
using Quidjibo.Providers;
using Quidjibo.Serializers;

namespace Quidjibo.Autofac.Tests.Modules
{
    [TestClass]
    public class QuidjiboModuleTests
    {
        private readonly IContainer _container;

        public QuidjiboModuleTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new QuidjiboModule(GetType().GetTypeInfo().Assembly));
            _container = builder.Build();
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<IQuidjiboHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<IQuidjiboHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (var scope = _container.BeginLifetimeScope())
            {
                Action resolve = () => scope.Resolve<IQuidjiboHandler<UnhandledCommand>>();
                resolve.ShouldThrow<ComponentNotRegisteredException>("Handler was not registerd");
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

            using (var scope = _container.BeginLifetimeScope())
            {
                var client = scope.Resolve<IQuidjiboClient>();
                client.Should().NotBeNull();
                client.Should().BeAssignableTo<IQuidjiboClient>();
                client.Should().Be(QuidjiboClient.Instance);

                var client1 = scope.Resolve<IQuidjiboClient<CustomClientKey1>>();
                client1.Should().NotBeNull();
                client1.Should().BeAssignableTo<IQuidjiboClient<CustomClientKey1>>();
                client1.Should().Be(QuidjiboClient<CustomClientKey1>.Instance);

                var client2 = scope.Resolve<IQuidjiboClient<CustomClientKey2>>();
                client2.Should().NotBeNull();
                client2.Should().BeAssignableTo<IQuidjiboClient<CustomClientKey2>>();
                client2.Should().Be(QuidjiboClient<CustomClientKey2>.Instance);
            }
        }
    }
}