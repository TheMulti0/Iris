using System;
using System.IO;
using System.Text.Json;

namespace Updates.Api
{
    public static class JsonExtensions
    {
        public static T Read<T>(string fileName)
        {
            try
            {
                return JsonSerializer
                    .Deserialize<T>(File.ReadAllText(fileName));
            }
            catch (Exception)
            {
                File.WriteAllText(fileName, "{ }");
                return default;
            }
        }

        public static void Write<T>(T data, string fileName)
        {
            File.Delete(fileName);
            File.WriteAllText(fileName, JsonSerializer.Serialize(data));
        }
    }
}