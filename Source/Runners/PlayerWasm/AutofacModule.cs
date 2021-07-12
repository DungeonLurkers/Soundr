using System;
using System.Linq;
using Autofac;
using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;
using Soundr.Commons.Attributes;

namespace PlayerWasm
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            var allTypes = typeof(AutofacModule).Assembly.GetTypes();
            var views = allTypes.Where(t => t.IsAssignableTo(typeof(IViewFor<>)));
            var viewModels = allTypes.Where(t => t.IsAssignableTo(typeof(ReactiveObject)));

            foreach (var type in views.Concat(viewModels))
            {
                builder.RegisterType(type).AsSelf().AsImplementedInterfaces();
            }

            var serviceTypes = typeof(AutofacModule).Assembly.GetTypes()
                .Select(x => (attr: x.GetCustomAttributes(true).OfType<ServiceAttribute>().SingleOrDefault(), type: x))
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