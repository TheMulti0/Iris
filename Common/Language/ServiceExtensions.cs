using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;

namespace Common
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLanguages(
            this IServiceCollection services)
        {
            return services.AddSingleton(
                _ => new Languages
                {
                    Dictionary = new Dictionary<Language, LanguageDictionary>
                    {
                        {
                            Language.English,
                            JsonSerializer.Deserialize<LanguageDictionary>(
                                File.ReadAllText("en-US.json"))
                        },
                        {
                            Language.Hebrew,
                            JsonSerializer.Deserialize<LanguageDictionary>(
                                File.ReadAllText("he-IL.json"))
                        }
                    }
                });
        }
    }
}