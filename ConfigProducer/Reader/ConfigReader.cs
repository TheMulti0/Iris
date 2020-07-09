using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ConfigProducer
{
    internal class ConfigReader
    {
        private readonly ConfigReaderConfig _config;
        
        public ConfigReader(ConfigReaderConfig config)
        {
            _config = config;
        }

        public Dictionary<string, string> ReadAllConfigs()
        {
            string extension = $".{_config.ConfigsFileExtension}";
            
            string[] filePaths = Directory
                .GetFiles(_config.ConfigsFolder, $"*{extension}");

            string ToFileName(string filePath)
            {
                return filePath
                    .Split("\\")
                    .LastOrDefault()?
                    .Replace(extension, string.Empty);
            }

            return filePaths.ToDictionary(
                ToFileName,
                File.ReadAllText);
        }
    }
}