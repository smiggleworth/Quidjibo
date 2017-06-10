using System;
using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Autofac.Modules;
using Quidjibo.Autofac.Tests.Samples;
using Quidjibo.Handlers;

namespace Quidjibo.Autofac.Tests.Modules
{
    [TestClass]
    public class AutofacModuleTests
    {
        private readonly IContainer _container;

        public AutofacModuleTests()
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
    }
}