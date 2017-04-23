using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Handlers;
using Quidjibo.Resolvers;
using Quidjibo.SimpleInjector.Packages;
using Quidjibo.SimpleInjector.Resolvers;
using Quidjibo.SimpleInjector.Tests.Samples;
using SimpleInjector;

namespace Quidjibo.SimpleInjector.Tests.Resolvers
{
    [TestClass]
    public class SimpleInjectorPayloadResolverTests
    {
        private readonly IPayloadResolver _resolver;

        public SimpleInjectorPayloadResolverTests()
        {
            var container = new Container();
            QuidjiboPackage.RegisterHandlers(container, GetType().Assembly);
            _resolver = new SimpleInjectorPayloadResolver(container);
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (_resolver.Begin())
            {
                var handler = _resolver.Resolve(typeof(IWorkHandler<BasicCommand>));
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (_resolver.Begin())
            {
                var handler = _resolver.Resolve(typeof(IWorkHandler<SimpleJob.Command>));
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (_resolver.Begin())
            {
                Action resolve = () => _resolver.Resolve(typeof(IWorkHandler<UnhandledCommand>));
                resolve.ShouldThrow<ActivationException>("Handler was not registerd");
            }
        }
    }
}