using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Dnx.Genny
{
    public class GennyServiceProvider : IServiceProvider
    {
        private Dictionary<Type, Object> Instances { get; }
        private IServiceProvider FallbackProvider { get; }

        public GennyServiceProvider(IServiceProvider fallback)
        {
            Instances = new Dictionary<Type, Object>();
            Instances[typeof(IServiceProvider)] = this;
            FallbackProvider = fallback;
        }

        public void Add<TService, TImplementation>()
        {
            Instances[typeof(TService)] = ActivatorUtilities.CreateInstance<TImplementation>(this);
        }
        public Object GetService(Type serviceType)
        {
            Object instance;
            if (Instances.TryGetValue(serviceType, out instance))
                return instance;

            return FallbackProvider.GetService(serviceType);
        }
        public TService GetService<TService>()
        {
            return (TService)GetService(typeof(TService));
        }
    }
}