using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Quidjibo.Exceptions;

namespace Quidjibo.Tests
{
    [TestClass]
    public class QuidjiboBuilderTests
    {
        [TestMethod]
        public void BuilderRequiresWorkProviderFactory()
        {
            var quidjibo = new QuidjiboBuilder();

            Action build = () => quidjibo.BuildServer();

            build.ShouldThrow<QuidjiboBuilderException>().WithMessage("Requires Work Provider Factory");
        }

        [TestMethod]
        public void BuilderRequiresScheduleProviderFactory()
        {
            var quidjibo = new QuidjiboBuilder();

            Action build = () => quidjibo.BuildServer();

            build.ShouldThrow<QuidjiboBuilderException>().WithMessage("Requires Schedule Provider Factory");
        }

        [TestMethod]
        public void BuilderRequiresProgressProviderFactory()
        {
            var quidjibo = new QuidjiboBuilder();

            Action build = () => quidjibo.BuildServer();

            build.ShouldThrow<QuidjiboBuilderException>().WithMessage("Requires Progress Provider Factory");
        }

        [TestMethod]
        public void When_Builder_IsNotConfigured_ShouldUseDefaults()
        {
            var quidjibo = new QuidjiboBuilder();


            var server = quidjibo.BuildServer();
            var client = quidjibo.BuildClient();
            server.Should().NotBeNull();
            client.Should().NotBeNull();
        }
    }
}