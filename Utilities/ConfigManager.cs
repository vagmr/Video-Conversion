using Newtonsoft.Json;

namespace VideoConverter.Utilities
{
    public static class ConfigManager
    {
        public static Config LoadConfig()
        {
            const string configPath = "config.json";
            if (File.Exists(configPath))
            {
                var json = File.ReadAllText(configPath);
                return JsonConvert.DeserializeObject<Config>(json) ?? new Config();
            }
            return new Config();
        }
    }
}
