using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Handlers;
using Quidjibo.StructureMap.Registries;
using Quidjibo.StructureMap.Tests.Samples;
using StructureMap;

namespace Quidjibo.StructureMap.Tests.Registries
{
    [TestClass]
    public class QuidjiboRegistryTests
    {
        private readonly IContainer _container;

        public QuidjiboRegistryTests()
        {
            var registry = new Registry();
            registry.IncludeRegistry(new QuidjiboRegistry());

            _container = new Container(registry);
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_GetInstance()
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                var handler = nestedContainer.GetInstance<IQuidjiboHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_GetInstance()
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                var handler = nestedContainer.GetInstance<IQuidjiboHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (var nestedContainer = _container.GetNestedContainer())
            {
                Action resolve = () => nestedContainer.GetInstance<IQuidjiboHandler<UnhandledCommand>>();
                resolve.ShouldThrow<StructureMapConfigurationException>("Handler was not registerd");
            }
        }
    }
}