using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Pipeline.Builders;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Resolvers;

namespace Quidjibo.Tests.Pipeline.Builders
{
    [TestClass]
    public class QuidjiboPipelineBuilderTests
    {
        [TestMethod]
        public void Use_ShouldCreateValidPipelineStepFromDelegate()
        {
            // Arrange
            var delegateInvoked = false;
            var resolver = Substitute.For<IDependencyResolver>();
            var context = Substitute.For<IQuidjiboContext>();
            var builder = new QuidjiboPipelineBuilder();

            // Act
            builder.Use((ctx, next) =>
            {
                delegateInvoked = true;
                return next();
            });
            var pipeline = builder.Build(resolver);
            pipeline.StartAsync(context, CancellationToken.None);

            // Assert
            delegateInvoked.Should().BeTrue("delegates should be invoked by the pipeline.");
        }


        [TestMethod]
        public void Use_ShouldCreateValidPipelineStepFromType()
        {
            // Arrange
            SampleMiddleware.Invoked = false;
            var resolver = Substitute.For<IDependencyResolver>();
            var context = Substitute.For<IQuidjiboContext>();
            var builder = new QuidjiboPipelineBuilder();

            resolver.Resolve(typeof(SampleMiddleware)).Returns(new SampleMiddleware());

            // Act
            builder.Use<SampleMiddleware>();
            var pipeline = builder.Build(resolver);
            pipeline.StartAsync(context, CancellationToken.None);

            // Assert
            SampleMiddleware.Invoked.Should().BeTrue("middleware should be resolved and invoked by the pipeline.");
        }

        [TestMethod]
        public void Use_ShouldCreateValidPipelineStepFromInstance()
        {
            // Arrange
            SampleMiddleware.Invoked = false;
            var resolver = Substitute.For<IDependencyResolver>();
            var context = Substitute.For<IQuidjiboContext>();
            var builder = new QuidjiboPipelineBuilder();

            // Act
            builder.Use(new SampleMiddleware());
            var pipeline = builder.Build(resolver);
            pipeline.StartAsync(context, CancellationToken.None);

            // Assert
            SampleMiddleware.Invoked.Should().BeTrue("middleware should be invoked by the pipeline.");
        }


        private class SampleMiddleware : IPipelineMiddleware
        {
            public static bool Invoked { get; set; }

            public Task InvokeAsync(IQuidjiboContext context, Func<Task> next, CancellationToken cancellationToken)
            {
                Invoked = true;
                return next();
            }
        }
    }
}