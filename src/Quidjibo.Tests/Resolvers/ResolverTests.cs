using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Handlers;
using Quidjibo.Resolvers;
using Quidjibo.Tests.Samples;

namespace Quidjibo.Tests.Resolvers
{
    [TestClass]
    public class ResolverTests
    {
        private DependencyResolver _resolver;

        [TestInitialize]
        public void Init()
        {
            _resolver = new DependencyResolver(null, GetType().Assembly);
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
                resolve.ShouldThrow<NullReferenceException>("Could not find a handler that matches your command.");
            }
        }

        [TestMethod]
        public void When_Handler_DoesNotHaveDefaultConstructor_Should_Throw()
        {
            using (_resolver.Begin())
            {
                Action resolve = () => _resolver.Resolve(typeof(IQuidjiboHandler<DependentCommand>));
                resolve.ShouldThrow<MissingMethodException>("No parameterless constructor defined for this object.");
            }
        }
    }
}