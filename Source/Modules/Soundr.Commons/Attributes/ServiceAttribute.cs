using System;
using Microsoft.Extensions.DependencyInjection;

namespace Soundr.Commons.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ServiceAttribute : Attribute
    {
        public ServiceLifetime Lifetime { get; }

        public ServiceAttribute(ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            Lifetime = lifetime;
        }
    }
}