using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac;
using Autofac.Core.Registration;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Autofac.Modules;
using Quidjibo.Handlers;

namespace Quidjibo.Autofac.Tests.Modules
{
    [TestClass]
    public class AutofacModuleTests
    {
        private readonly IContainer container;

        public AutofacModuleTests()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new QuidjiboModule(GetType().Assembly));
            container = builder.Build();
        }

        [TestMethod]
        public void When_Handler_IsRegistered_Should_Resolve()
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<IWorkHandler<BasicCommand>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<BasicHandler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsRegistered_InNestedClass_Should_Resolve()
        {
            using (var scope = container.BeginLifetimeScope())
            {
                var handler = scope.Resolve<IWorkHandler<SimpleJob.Command>>();
                handler.Should().NotBeNull("there should be a matching handler");
                handler.Should().BeOfType<SimpleJob.Handler>("the handler should match the command");
            }
        }

        [TestMethod]
        public void When_Handler_IsNotRegistered_Should_Throw()
        {
            using (var scope = container.BeginLifetimeScope())
            {
                Action resolve = () => scope.Resolve<IWorkHandler<UnhandledCommand>>();
                resolve.ShouldThrow<ComponentNotRegisteredException>("Handler was not registerd");
            }
        }
    }
}
