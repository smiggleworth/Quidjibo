using System;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.DependencyInjection.Extensions;
using Quidjibo.DependencyInjection.Tests.Samples;
using Quidjibo.Handlers;

namespace Quidjibo.DependencyInjection.Tests.Extensions
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceCollectionExtensionsTests()
        {
            var services = new ServiceCollection();
            services.AddQuidjibo(typeof(ServiceCollectionExtensionsTests).Assembly);
            _serviceProvider = services.BuildServiceProvider();
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IWorkHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IWorkHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var handler = scope.ServiceProvider.GetService<IWorkHandler<UnhandledCommand>>();
                handler.Should().BeNull("handler was not registerd");
            }
        }
    }
}