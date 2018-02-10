﻿using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Handlers;
using Quidjibo.Resolvers;
using Quidjibo.StructureMap.Registries;
using Quidjibo.StructureMap.Resolvers;
using Quidjibo.StructureMap.Tests.Samples;
using StructureMap;

namespace Quidjibo.StructureMap.Tests.Resolvers
{
    [TestClass]
    public class StructureMapPayloadResolverTests
    {
        private readonly IDependencyResolver _resolver;

        public StructureMapPayloadResolverTests()
        {
            var registry = new Registry();
            registry.IncludeRegistry(new QuidjiboRegistry());
            var container = new Container(registry);
            _resolver = new StructureMapDependencyResolver(container);
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
                Action resolve = () => _resolver.Resolve(typeof(IQuidjiboHandler<UnhandledCommand>));
                resolve.Should().Throw<StructureMapConfigurationException>("Handler was not registerd");
            }
        }
    }
}