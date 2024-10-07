using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace VideoConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var parser = new ArgumentParser(args);
                var converter = new VideoConverter();
                converter.ConvertToHevc(
                    parser.InputFile,
                    parser.OutputFile,
                    parser.Crf,
                    parser.Preset,
                    parser.AudioCodec
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }
    }

    class VideoConverter
    {
        public void ConvertToHevc(
            string inputFile,
            string? outputFile = null,
            int crf = 28,
            string preset = "slow",
            string audioCodec = "aac")
        {
            if (!File.Exists(inputFile))
            {
                throw new FileNotFoundException($"未找到输入文件: {inputFile}");
            }

            outputFile ??= GenerateOutputFilename(inputFile);

            var ffmpegCommand = new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{inputFile}\" -c:v hevc_nvenc -preset {preset} -crf {crf} -c:a {audioCodec} \"{outputFile}\"",
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

        public ArgumentParser(string[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("必须提供输入文件路径");
            }

            InputFile = args[0];
            OutputFile = GetArgumentValue(args, "-o", "--output");
            Crf = int.Parse(GetArgumentValue(args, "--crf") ?? "28");
            Preset = GetArgumentValue(args, "--preset") ?? "slower";
            AudioCodec = GetArgumentValue(args, "--audio-codec") ?? "aac";
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
