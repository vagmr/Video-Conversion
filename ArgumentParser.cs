// This is the class that parses the command line arguments and sets the properties of the Config class.
namespace VideoConverter
{
    public class ArgumentParser
    {
        public string InputFile { get; }
        public string OutputFile { get; }
        public int Crf { get; }
        public string Preset { get; }
        public string AudioCodec { get; }
        public string? Resolution { get; }

        public ArgumentParser(string[] args, Config config)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("必须提供输入文件路径");
            }

            InputFile = args[0];
            OutputFile = GetArgumentValue(args, "-o", "--output") ?? GenerateOutputFilename(InputFile);
            Crf = ParseIntArgumentOrDefault(args, "--crf", config.Crf ?? 28);
            Preset = GetArgumentValue(args, "--preset") ?? config.Preset ?? "slow";
            AudioCodec = GetArgumentValue(args, "--audio-codec") ?? config.AudioCodec ?? "aac";
            Resolution = GetArgumentValue(args, "-r", "--resolution") ?? config.Resolution;
        }

        private static int ParseIntArgumentOrDefault(string[] args, string argName, int defaultValue)
        {
            var value = GetArgumentValue(args, argName);
            if (value == null)
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out int result))
            {
                Console.WriteLine($"无效的 {argName} 值: {value}。应为一个整数,将使用默认值: {defaultValue}");
                return defaultValue;
            }

            return result;
        }

        private static string? GetArgumentValue(string[] args, params string[] argumentNames)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (argumentNames.Contains(args[i]))
                {
                    return args[i + 1];
                }
            }
            return null;
        }

        private static string GenerateOutputFilename(string inputFile)
        {
            var random = new Random();
            var chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var randomString = new string(Enumerable.Repeat(chars, 6)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return Path.Combine(
                Path.GetDirectoryName(inputFile) ?? "",
                $"{Path.GetFileNameWithoutExtension(inputFile)}_hevc_{randomString}{Path.GetExtension(inputFile)}"
            );
        }
    }
}