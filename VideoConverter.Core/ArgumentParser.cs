namespace VideoConverter.Core
{
    public class ArgumentParser
    {
        //Todo 实现批量(已完成)
        public List<string> InputFiles { get; }
        public string? OutputFile { get; }
        public int Crf { get; }
        public string Preset { get; }
        public string AudioCodec { get; }
        public string? Resolution { get; }
        public string Encoder { get; }
        public string? Bitrate { get; }
        public ArgumentParser(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("必须提供输入文件路径,使用-h或--help查看帮助信息");
            }

            InputFiles = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-") || args[i].StartsWith("--"))
                {
                    break;
                }

                InputFiles.Add(args[i]);
            }
            if (InputFiles.Count == 0)
            {
                throw new ArgumentException("至少需要提供一个输入文件路径,使用-h或--help查看帮助信息");
            }
            OutputFile = GetArgumentValue(args, "-o", "--output");
            Crf = ParseIntArgumentOrDefault(args, "-c", "--crf", 28);
            Preset = GetArgumentValue(args, "-p", "--preset") ?? "fast";
            AudioCodec = GetArgumentValue(args, "-ac", "--audio-codec") ?? "aac";
            Resolution = GetArgumentValue(args, "-r", "--resolution");
            Encoder = GetArgumentValue(args, "-e", "--encoder") ?? "nvenc";
            Bitrate = GetArgumentValue(args, "-b", "--bitrate");
        }

        private static int ParseIntArgumentOrDefault(string[] args, string shortArg, string longArg, int defaultValue)
        {
            var value = GetArgumentValue(args, shortArg, longArg);
            if (value == null)
            {
                return defaultValue;
            }

            if (!int.TryParse(value, out int result))
            {
                Console.WriteLine($"无效的 {longArg} 值: {value}。应为一个整数,将使用默认值: {defaultValue}");
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
    }
}
