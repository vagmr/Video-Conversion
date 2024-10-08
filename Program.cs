using System.Diagnostics;

namespace VideoConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 检查是否包含帮助标志
                if (args.Contains("-h") || args.Contains("--help"))
                {
                    PrintHelp();
                    return;
                }

                var parser = new ArgumentParser(args);
                VideoConverter.ConvertToHevc(
                    parser.InputFile,
                    parser.OutputFile,
                    parser.Crf,
                    parser.Preset,
                    parser.AudioCodec,
                    parser.Resolution
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }

        static void PrintHelp()
        {
            Console.WriteLine("用法: VideoConverter [输入文件] [选项]");
            Console.WriteLine("选项:");
            Console.WriteLine("  -o, --output [文件路径]    指定输出文件路径,可以省略 (默认随机生成)");
            Console.WriteLine("  --crf [值]                 设置视频质量 (默认: 28)");
            Console.WriteLine("  --preset [值]              设置编码速度 (默认: slow)");
            Console.WriteLine("  --audio-codec [值]         设置音频编码格式 (默认: aac)");
            Console.WriteLine("  -r, --resolution [值]      设置视频分辨率 (例如：720, 480)");
            Console.WriteLine("  -h, --help                 显示帮助信息");
            Console.WriteLine("\n示例:");
            Console.WriteLine("  VideoConverter input.mp4 -o output.mp4 --crf 24 --preset fast --audio-codec copy --resolution 720");
        }
    }

    static class VideoConverter
    {

        private static readonly string[] ValidPresets = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow" };
        private static readonly string[] ValidAudioCodecs = { "aac", "copy", "mp3" };
        private const int MinCrf = 0;
        private const int MaxCrf = 51;

        public static void ConvertToHevc(
          string inputFile,
          string? outputFile = null,
          int crf = 28,
          string preset = "slow",
          string audioCodec = "aac",
          string? resolution = null)
        {
            Console.WriteLine($"调试信息：分辨率参数 = {resolution ?? "未设置"}");
            ValidateInputParameters(inputFile, crf, preset, audioCodec, resolution);

            outputFile ??= GenerateOutputFilename(inputFile);

            ValidateOutputFile(outputFile);

            Console.WriteLine($"开始转换：{inputFile} => {outputFile}");

            // 修复分辨率参数处理
            var resolutionOption = string.IsNullOrEmpty(resolution) ? "" : $"-vf scale=-2:{resolution}";


            var ffmpegCommand = CreateFfmpegCommand(inputFile, outputFile, crf, preset, audioCodec, resolutionOption);
            ExecuteFfmpegCommand(ffmpegCommand);
        }
        private static void ValidateInputParameters(string inputFile, int crf, string preset, string audioCodec, string? resolution)
        {
            ValidateInputFile(inputFile);
            ValidateCrf(crf);
            ValidatePreset(preset);
            ValidateAudioCodec(audioCodec);
            ValidateResolution(resolution);
        }
        private static void ValidateInputFile(string inputFile)
        {
            if (string.IsNullOrWhiteSpace(inputFile))
            {
                throw new ArgumentException("输入文件路径不能为空", nameof(inputFile));
            }
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"未找到输入文件: {inputFile}");
            }
        }

        private static void ValidateCrf(int crf)
        {
            if (crf < MinCrf || crf > MaxCrf)
            {
                throw new ArgumentOutOfRangeException(nameof(crf), $"CRF 值必须在 {MinCrf} 到 {MaxCrf} 之间");
            }
        }

        private static void ValidatePreset(string preset)
        {
            if (!ValidPresets.Contains(preset.ToLower()))
            {
                throw new ArgumentException($"无效的 preset 值: {preset}。有效值为: {string.Join(", ", ValidPresets)}", nameof(preset));
            }
        }

        private static void ValidateAudioCodec(string audioCodec)
        {
            if (!ValidAudioCodecs.Contains(audioCodec.ToLower()))
            {
                throw new ArgumentException($"无效的音频编码格式: {audioCodec}。有效值为: {string.Join(", ", ValidAudioCodecs)}", nameof(audioCodec));
            }
        }

        private static void ValidateResolution(string? resolution)
        {
            if (!string.IsNullOrEmpty(resolution))
            {
                if (!int.TryParse(resolution, out int resolutionValue) || resolutionValue <= 0)
                {
                    throw new ArgumentException("分辨率必须是正整数", nameof(resolution));
                }
            }
        }

        private static void ValidateOutputFile(string outputFile)
        {
            var outputDirectory = Path.GetDirectoryName(outputFile);
            if (!Directory.Exists(outputDirectory))
            {
                throw new DirectoryNotFoundException($"输出目录不存在: {outputDirectory}");
            }
        }
        private static ProcessStartInfo CreateFfmpegCommand(string inputFile, string outputFile, int crf, string preset, string audioCodec, string resolutionOption)
        {
            return new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{inputFile}\" -c:v hevc_nvenc -preset {preset} {resolutionOption} -crf {crf} -c:a {audioCodec} \"{outputFile}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        private static void ExecuteFfmpegCommand(ProcessStartInfo ffmpegCommand)
        {
            using var process = new Process { StartInfo = ffmpegCommand };
            process.ErrorDataReceived += (sender, e) => Console.WriteLine(e.Data);
            process.OutputDataReceived += (sender, e) => Console.WriteLine(e.Data);

            try
            {
                Console.WriteLine($"执行命令: {ffmpegCommand.FileName} {ffmpegCommand.Arguments}");
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"转换成功：{ffmpegCommand.Arguments.Split('"').LastOrDefault()}");
                }
                else
                {
                    throw new Exception($"转换失败，FFmpeg 返回码：{process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"转换过程中发生错误：{ex.Message}");
                throw;
            }
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

    class ArgumentParser
    {
        public string InputFile { get; }
        public string? OutputFile { get; }
        public int Crf { get; }
        public string Preset { get; }
        public string AudioCodec { get; }
        public string? Resolution { get; }

        public ArgumentParser(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("必须提供输入文件路径,使用-h或--help查看帮助信息");
            }

            InputFile = args[0];
            OutputFile = GetArgumentValue(args, "-o", "--output");
            Crf = ParseIntArgumentOrDefault(args, "--crf", 28);
            Preset = GetArgumentValue(args, "--preset") ?? "slow";
            AudioCodec = GetArgumentValue(args, "--audio-codec") ?? "aac";
            Resolution = GetArgumentValue(args, "-r", "--resolution");
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
    }
}
