using Microsoft.Framework.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dnx.Genny.Services
{
    public class ServiceProvider : IServiceProvider
    {
        private IServiceProvider Provider { get; }
        private Dictionary<Type, Object> Instances { get; }

        public ServiceProvider(IServiceProvider fallback)
        {
            Instances = new Dictionary<Type, Object>();
            Instances[typeof(IServiceProvider)] = this;
            Provider = fallback;
        }

        public void Add(Type type, Object instance)
        {
            Instances[type] = instance;
        }
        public void Add<TService, TImplementation>()
        {
            Add(typeof(TService), ActivatorUtilities.CreateInstance<TImplementation>(this));
        }

        public Object GetService(Type serviceType)
        {
            Object instance;
            if (Instances.TryGetValue(serviceType, out instance))
                return instance;

            return Provider.GetService(serviceType);
        }
    }
}