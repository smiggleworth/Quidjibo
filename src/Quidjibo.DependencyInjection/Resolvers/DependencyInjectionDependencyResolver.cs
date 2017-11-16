﻿using System;
using Microsoft.Extensions.DependencyInjection;
using Quidjibo.Resolvers;

namespace Quidjibo.DependencyInjection.Resolvers
{
    public class DependencyInjectionDependencyResolver : IDependencyResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private IServiceScope _serviceScope;

        public DependencyInjectionDependencyResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IDisposable Begin()
        {
            _serviceScope = _serviceProvider.CreateScope();
            return _serviceScope;
        }

        public object Resolve(Type type)
        {
            return _serviceScope.ServiceProvider.GetService(type);
        }
    }
}