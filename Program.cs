using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

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
                var converter = new VideoConverter();
                converter.ConvertToHevc(
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
            Console.WriteLine("  --resolution [值]          设置视频分辨率 (例如：720, 480)");
            Console.WriteLine("  -h, --help                 显示帮助信息");
            Console.WriteLine("\n示例:");
            Console.WriteLine("  VideoConverter input.mp4 -o output.mp4 --crf 24 --preset fast --audio-codec copy --resolution 720");
        }
    }

    class VideoConverter
    {
        public void ConvertToHevc(
            string inputFile,
            string? outputFile = null,
            int crf = 28,
            string preset = "slow",
            string audioCodec = "aac",
            string? resolution = null)
        {
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"未找到输入文件: {inputFile}");
            }

            outputFile ??= GenerateOutputFilename(inputFile);

            Console.WriteLine($"开始转换：{inputFile} => {outputFile}");
            var resolutionOption = string.IsNullOrEmpty(resolution) ? "" : $"scale=-2:{resolution}";
            var ffmpegCommand = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{inputFile}\"  -c:v hevc_nvenc -preset {preset} -vf {resolutionOption} -crf {crf} -c:a {audioCodec} \"{outputFile}\"",
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = ffmpegCommand };
            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Console.WriteLine(e.Data);
                }
            };

            try
            {
                Console.WriteLine($"执行命令: {ffmpegCommand.FileName} {ffmpegCommand.Arguments}");
                process.Start();
                process.BeginErrorReadLine();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"转换成功：{outputFile}");
                }
                else
                {
                    Console.WriteLine($"转换失败，返回码：{process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"转换过程中发生错误：{ex.Message}");
            }
        }

        private string GenerateOutputFilename(string inputFile)
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
            Crf = int.Parse(GetArgumentValue(args, "--crf") ?? "28");
            Preset = GetArgumentValue(args, "--preset") ?? "slow";
            AudioCodec = GetArgumentValue(args, "--audio-codec") ?? "aac";
            Resolution = GetArgumentValue(args, "--resolution");
        }

        private string? GetArgumentValue(string[] args, params string[] argumentNames)
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
