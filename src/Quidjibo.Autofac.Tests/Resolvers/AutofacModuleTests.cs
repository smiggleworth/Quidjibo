using System;
using System.Reflection;
using Autofac;
using Autofac.Core.Registration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Autofac.Modules;
using Quidjibo.Autofac.Resolvers;
using Quidjibo.Autofac.Tests.Samples;
using Quidjibo.Handlers;
using Quidjibo.Resolvers;

namespace Quidjibo.Autofac.Tests.Resolvers
{
    [TestClass]
    public class AutofacModuleTests
    {
        private readonly IDependencyResolver _resolver;

        public AutofacModuleTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new QuidjiboModule(GetType().GetTypeInfo().Assembly));
            var container = builder.Build();
            _resolver = new AutofacDependencyResolver(container);
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (_resolver.Begin())
            {
                var handler = _resolver.Resolve(typeof(IQuidjiboHandler<BasicCommand>));
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (_resolver.Begin())
            {
                var handler = _resolver.Resolve(typeof(IQuidjiboHandler<SimpleJob.Command>));
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (_resolver.Begin())
            {
                Action resolve = () => _resolver.Resolve(typeof(IQuidjiboHandler<UnhandledCommand>));
                resolve.Should().Throw<ComponentNotRegisteredException>("Handler was not registerd");
            }
        }
    }
}