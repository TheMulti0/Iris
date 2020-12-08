using Microsoft.Extensions.Configuration;

namespace Extensions
{
    public static class ConfigExtensions
    {
        public static T GetSection<T>(
            this IConfiguration config,
            string key)
        {
            return config.GetSection(key).Get<T>();
        }
    }
}