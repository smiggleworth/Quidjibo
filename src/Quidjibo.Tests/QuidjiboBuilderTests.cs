using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Configurations;
using Quidjibo.Exceptions;
using Quidjibo.Factories;

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

            build.Should().Throw<QuidjiboBuilderException>()
                 .WithMessage("Failed to validate. See list of errors for more detail.")
                 .And.Errors.Should().Contain("Requires Work Provider Factory");
        }

        [TestMethod]
        public void BuilderRequiresScheduleProviderFactory()
        {
            var quidjibo = new QuidjiboBuilder();

            Action build = () => quidjibo.BuildServer();

            build.Should().Throw<QuidjiboBuilderException>()
                 .WithMessage("Failed to validate. See list of errors for more detail.")
                 .And.Errors.Should().Contain("Requires Schedule Provider Factory");
        }

        [TestMethod]
        public void BuilderRequiresProgressProviderFactory()
        {
            var quidjibo = new QuidjiboBuilder();

            Action build = () => quidjibo.BuildServer();

            build.Should().Throw<QuidjiboBuilderException>()
                 .WithMessage("Failed to validate. See list of errors for more detail.")
                 .And.Errors.Should().Contain("Requires Progress Provider Factory");
        }

        [TestMethod]
        public void When_Builder_IsNotConfigured_ShouldUseDefaults()
        {
            var quidjibo = new QuidjiboBuilder()
                           .Configure(Substitute.For<IQuidjiboConfiguration>())
                           .ConfigureWorkProviderFactory(Substitute.For<IWorkProviderFactory>())
                           .ConfigureScheduleProviderFactory(Substitute.For<IScheduleProviderFactory>())
                           .ConfigureProgressProviderFactory(Substitute.For<IProgressProviderFactory>());


            var server = quidjibo.BuildServer();
            var client = quidjibo.BuildClient();
            server.Should().NotBeNull();
            client.Should().NotBeNull();
        }
    }
}