using System.Diagnostics;

namespace VideoConverter
{
    public static class VideoConverter
    {
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

            Console.WriteLine($"开始转换：{inputFile} => {outputFile}");

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
            if (crf < Constants.MinCrf || crf > Constants.MaxCrf)
            {
                throw new ArgumentOutOfRangeException(nameof(crf), $"CRF 值必须在 {Constants.MinCrf} 到 {Constants.MaxCrf} 之间");
            }
        }

        private static void ValidatePreset(string preset)
        {
            if (int.TryParse(preset, out int presetIndex))
            {
                if (presetIndex < 0 || presetIndex >= Constants.ValidPresets.Length)
                {
                    throw new ArgumentException($"无效的 preset 值: {preset}。数字应在 0 到 {Constants.ValidPresets.Length - 1} 之间", nameof(preset));
                }
            }
            else if (!Constants.ValidPresets.Contains(preset.ToLower()))
            {
                throw new ArgumentException($"无效的 preset 值: {preset}。有效值为: {string.Join(", ", Constants.ValidPresets)} 或 0-{Constants.ValidPresets.Length - 1}", nameof(preset));
            }
        }

        private static void ValidateAudioCodec(string audioCodec)
        {
            if (!Constants.ValidAudioCodecs.Contains(audioCodec.ToLower()))
            {
                throw new ArgumentException($"无效的音频编码格式: {audioCodec}。有效值为: {string.Join(", ", Constants.ValidAudioCodecs)}", nameof(audioCodec));
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

        private static ProcessStartInfo CreateFfmpegCommand(string inputFile, string outputFile, int crf, string preset, string audioCodec, string resolutionOption)
        {
            if (int.TryParse(preset, out int presetIndex))
            {
                preset = Constants.ValidPresets[presetIndex];
            }

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
}