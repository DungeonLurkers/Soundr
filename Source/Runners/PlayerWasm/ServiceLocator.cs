using Microsoft.Extensions.DependencyInjection;

namespace PlayerWasm
{
    public static class ServiceLocator
    {
        public static T Get<T>() where T : notnull
        {
            return Program.ServiceProvider.GetRequiredService<T>();
        }
    }
}