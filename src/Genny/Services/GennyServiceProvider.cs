using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Genny
{
    public class GennyServiceProvider : IServiceProvider
    {
        private Dictionary<Type, Object> Instances { get; }

        public GennyServiceProvider()
        {
            Instances = new Dictionary<Type, Object>();
            Instances[typeof(IServiceProvider)] = this;
        }

        public void Add<TService>(TService implementation)
        {
            Instances[typeof(TService)] = implementation;
        }
        public void Add<TService, TImplementation>()
        {
            Instances[typeof(TService)] = ActivatorUtilities.CreateInstance<TImplementation>(this);
        }
        public Object GetService(Type serviceType)
        {
            return Instances.ContainsKey(serviceType) ? Instances[serviceType] : null;
        }
        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }
    }
}
