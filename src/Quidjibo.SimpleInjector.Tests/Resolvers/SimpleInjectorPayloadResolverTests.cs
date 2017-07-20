using System;
using System.Reflection;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Handlers;
using Quidjibo.Resolvers;
using Quidjibo.SimpleInjector.Extensions;
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
            container.RegisterQuidjibo(GetType().GetTypeInfo().Assembly);
            _resolver = new SimpleInjectorPayloadResolver(container);
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
                resolve.ShouldThrow<ActivationException>("Handler was not registerd");
            }
        }
    }
}