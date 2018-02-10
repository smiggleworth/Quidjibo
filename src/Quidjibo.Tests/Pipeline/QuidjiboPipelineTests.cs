using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Quidjibo.Dispatchers;
using Quidjibo.Pipeline;
using Quidjibo.Pipeline.Contexts;
using Quidjibo.Pipeline.Middleware;
using Quidjibo.Pipeline.Misc;
using Quidjibo.Protectors;
using Quidjibo.Resolvers;
using Quidjibo.Serializers;

namespace Quidjibo.Tests.Pipeline
{
    [TestClass]
    public class QuidjiboPipelineTests
    {
        [TestMethod]
        public async Task StartAsync_ShouldInvokeMiddleware()
        {
            // Arrange
            var middlewareInvoked = false;
            var resolver = Substitute.For<IDependencyResolver>();
            resolver.Begin().Returns(Substitute.For<IDisposable>());
            var steps = new List<PipelineStep>
            {
                new PipelineStep
                {
                    Instance = new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked = true;
                        await next();
                    })
                }
            };
            var pipeline = new QuidjiboPipeline(steps, Substitute.For<ILoggerFactory>(), resolver,Substitute.For<IPayloadProtector>(),Substitute.For<IPayloadSerializer>(),Substitute.For<IWorkDispatcher>());
            var context = Substitute.For<IQuidjiboContext>();

            // Act
            await pipeline.StartAsync(context, CancellationToken.None);

            // Assert
            middlewareInvoked.Should().BeTrue("Start should invoke the middleware in the pipeline.");
        }

        [TestMethod]
        public async Task StartAsync_ShouldInvokeAllStepsMiddleware()
        {
            // Arrange
            var middlewareInvoked = 0;
            var resolver = Substitute.For<IDependencyResolver>();
            resolver.Begin().Returns(Substitute.For<IDisposable>());
            var steps = new List<PipelineStep>
            {
                new PipelineStep
                {
                    Instance = new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked += 1;
                        await next();
                    })
                },
                new PipelineStep
                {
                    Instance = new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked += 1;
                        await next();
                    })
                }
            };
            var pipeline = new QuidjiboPipeline(steps, Substitute.For<ILoggerFactory>(), resolver, Substitute.For<IPayloadProtector>(), Substitute.For<IPayloadSerializer>(), Substitute.For<IWorkDispatcher>());
            var context = Substitute.For<IQuidjiboContext>();

            // Act
            await pipeline.StartAsync(context, CancellationToken.None);

            // Assert
            middlewareInvoked.Should().Be(2, "Start should invoke the middleware in the pipeline.");
        }

        [TestMethod]
        public async Task StartAsync_ShouldNotInvokeAllStepsMiddlewareWhenNextIsNotInvoked()
        {
            // Arrange
            var middlewareInvoked = 0;
            var resolver = Substitute.For<IDependencyResolver>();
            resolver.Begin().Returns(Substitute.For<IDisposable>());
            var steps = new List<PipelineStep>
            {
                new PipelineStep
                {
                    Instance = new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked += 1;
                        await Task.CompletedTask;
                    })
                },
                new PipelineStep
                {
                    Instance = new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked += 1;
                        await next();
                    })
                }
            };
            var pipeline = new QuidjiboPipeline(steps, Substitute.For<ILoggerFactory>(), resolver, Substitute.For<IPayloadProtector>(), Substitute.For<IPayloadSerializer>(), Substitute.For<IWorkDispatcher>());
            var context = Substitute.For<IQuidjiboContext>();

            // Act
            await pipeline.StartAsync(context, CancellationToken.None);

            // Assert
            middlewareInvoked.Should().Be(1, "the first step did not call next.");
        }

        [TestMethod]
        public async Task StartAsync_ShouldInvokeMiddlewareByResolvingTheInstance()
        {
            // Arrange
            var middlewareInvoked = false;
            var resolver = Substitute.For<IDependencyResolver>();
            resolver.Begin().Returns(Substitute.For<IDisposable>());
            resolver.Resolve(typeof(QuidjiboMiddleware))
                    .Returns(new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked = true;
                        await next();
                    }));
            var steps = new List<PipelineStep>
            {
                new PipelineStep
                {
                    Instance = null,
                    Type = typeof(QuidjiboMiddleware)
                }
            };
            var pipeline = new QuidjiboPipeline(steps, Substitute.For<ILoggerFactory>(), resolver, Substitute.For<IPayloadProtector>(), Substitute.For<IPayloadSerializer>(), Substitute.For<IWorkDispatcher>());
            var context = Substitute.For<IQuidjiboContext>();

            // Act
            await pipeline.StartAsync(context, CancellationToken.None);

            // Assert
            middlewareInvoked.Should().BeTrue("Start should invoke the middleware in the pipeline.");
        }


        [TestMethod]
        public async Task StartAsync_ShouldNotInvokeStepIfCancellationTokenHasBeenSignalled()
        {
            // Arrange
            var middlewareInvoked = 0;
            var cts = new CancellationTokenSource();
            var resolver = Substitute.For<IDependencyResolver>();
            resolver.Begin().Returns(Substitute.For<IDisposable>());
            var steps = new List<PipelineStep>
            {
                new PipelineStep
                {
                    Instance = new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked += 1;
                        cts.Cancel();
                        await next();
                    })
                },
                new PipelineStep
                {
                    Instance = new QuidjiboMiddleware(async (ctx, next) =>
                    {
                        middlewareInvoked += 1;
                        await next();
                    })
                }
            };
            var pipeline = new QuidjiboPipeline(steps, Substitute.For<ILoggerFactory>(), resolver, Substitute.For<IPayloadProtector>(), Substitute.For<IPayloadSerializer>(), Substitute.For<IWorkDispatcher>());
            var context = Substitute.For<IQuidjiboContext>();

            // Act
            await pipeline.StartAsync(context, cts.Token);

            // Assert
            middlewareInvoked.Should().Be(1, "the pipeline was canceled before step two.");
        }
    }
}