using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Soundr.Commons.Attributes;
using Soundr.YouTube.Services;
using Module = Autofac.Module;

namespace Soundr.PlayerApi
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {

            var serviceTypes = typeof(IYoutubePlayer).Assembly.GetTypes()
                .Select(x => (attr: x.GetCustomAttribute<ServiceAttribute>(), type: x))
                .Where(x => x.attr is not null)
                .ToList();

            foreach (var (attr, type) in serviceTypes)
            {
                switch (attr!.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        builder.RegisterType(type).AsImplementedInterfaces().AsSelf().SingleInstance();
                        break;
                    case ServiceLifetime.Scoped:
                        builder.RegisterType(type).AsImplementedInterfaces().AsSelf().InstancePerLifetimeScope();
                        break;
                    case ServiceLifetime.Transient:
                        builder.RegisterType(type).AsImplementedInterfaces().AsSelf();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            base.Load(builder);
        }
    }
}