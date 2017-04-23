using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Handlers;
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
            _container.RegisterHandlers(typeof(ContainerExtensionsTests).Assembly);
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                var handler = _container.GetInstance<IWorkHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                var handler = _container.GetInstance<IWorkHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (AsyncScopedLifestyle.BeginScope(_container))
            {
                Action resolve = () => _container.GetInstance<IWorkHandler<UnhandledCommand>>();
                resolve.ShouldThrow<ActivationException>("Handler was not registerd");
            }
        }
    }
}