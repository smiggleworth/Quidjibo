﻿using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.DependencyInjection.Extensions;
using Quidjibo.DependencyInjection.Resolvers;
using Quidjibo.DependencyInjection.Tests.Samples;
using Quidjibo.Handlers;
using Quidjibo.Resolvers;

namespace Quidjibo.DependencyInjection.Tests.Resolvers
{
    [TestClass]
    public class DependencyInjectionPayloadResolverTests
    {
        private readonly IDependencyResolver _resolver;

        public DependencyInjectionPayloadResolverTests()
        {
            var services = new ServiceCollection();
            services.AddQuidjibo(GetType().GetTypeInfo().Assembly);
            var serviceProvider = services.BuildServiceProvider();
            _resolver = new DependencyInjectionDependencyResolver(serviceProvider);
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using(_resolver.Begin())
            {
                var handler = _resolver.Resolve(typeof(IQuidjiboHandler<BasicCommand>));
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using(_resolver.Begin())
            {
                var handler = _resolver.Resolve(typeof(IQuidjiboHandler<SimpleJob.Command>));
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using(_resolver.Begin())
            {
                var handler = _resolver.Resolve(typeof(IQuidjiboHandler<UnhandledCommand>));
                handler.Should().BeNull("handler was not registerd");
            }
        }
    }
}