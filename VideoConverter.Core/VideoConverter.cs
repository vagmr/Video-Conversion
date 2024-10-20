using System.Diagnostics;

namespace VideoConverter.Core
{
    public static class VideoConverter
    {
        public static void ConvertToHevc(
            string inputFile,
            string? outputFile = null,
            int crf = 28,
            string preset = "fast",
            string audioCodec = "aac",
            string? resolution = null,
            string encoder = "nvenc",
            string? bitrate = null)
        {
            Console.WriteLine($"调试信息：分辨率参数 = {resolution ?? "未设置"}");
            ValidateInputParameters(inputFile, crf, preset, audioCodec, resolution, encoder);

            outputFile ??= GenerateOutputFilename(inputFile);

            Console.WriteLine($"开始转换：{inputFile} => {outputFile}");

            var resolutionOption = string.IsNullOrEmpty(resolution) ? "" : $"-vf scale=-2:{resolution}";

            var ffmpegCommand = CreateFfmpegCommand(inputFile, outputFile, crf, preset, audioCodec, resolutionOption, encoder, bitrate);
            ExecuteFfmpegCommand(ffmpegCommand);
        }

        private static void ValidateInputParameters(string inputFile, int crf, string preset, string audioCodec, string? resolution, string encoder)
        {
            ValidateInputFile(inputFile);
            ValidateCrf(crf);
            ValidatePreset(preset, encoder);
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

        private static void ValidatePreset(string preset, string encoder)
        {
            if (encoder != "libx265" && encoder != "nvenc")
            {
                throw new ArgumentException("不支持的编码器, 请使用 nvenc 或 libx265", nameof(encoder));
            }
            string[] validPresets = encoder == "libx265" ? Constants.ValidPresetsForLibx265 : Constants.ValidPresetsForNvenc;

            if (int.TryParse(preset, out int presetIndex))
            {
                if (presetIndex < 0 || presetIndex >= validPresets.Length)
                {
                    throw new ArgumentException($"无效的 preset 值: {preset}。数字应在 0 到 {validPresets.Length - 1} 之间", nameof(preset));
                }
            }
            else if (!validPresets.Contains(preset.ToLower()))
            {
                throw new ArgumentException($"无效的 preset 值: {preset}。有效值为: {string.Join(", ", validPresets)} 或 0-{validPresets.Length - 1}", nameof(preset));
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

        private static ProcessStartInfo CreateFfmpegCommand(string inputFile, string outputFile, int crf, string preset, string audioCodec, string resolutionOption, string encoder, string? bitrate)
        {
            string[] validPresets = encoder == "libx265" ? Constants.ValidPresetsForLibx265 : Constants.ValidPresetsForNvenc;

            if (int.TryParse(preset, out int presetIndex))
            {
                preset = validPresets[presetIndex];
            }

            string encoderOption = encoder == "libx265" ? "-c:v libx265" : "-c:v hevc_nvenc";

            string bitrateOption = string.IsNullOrEmpty(bitrate) ? "" : $"-b:v {bitrate}";

            return new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{inputFile}\" {encoderOption} -preset {preset} {resolutionOption} {bitrateOption} -crf {crf} -c:a {audioCodec} \"{outputFile}\"",
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
                $"{Path.GetFileNameWithoutExtension(inputFile)}_h265_{randomString}{Path.GetExtension(inputFile)}"
            );
        }

        public static async Task ConvertAsync(
            string inputFile,
            string? outputFile = null,
            int crf = 28,
            string preset = "fast",
            string audioCodec = "aac",
            string? resolution = null,
            string encoder = "nvenc",
            string? bitrate = null)
        {
            await Task.Run(() => ConvertToHevc(inputFile, outputFile, crf, preset, audioCodec, resolution, encoder, bitrate));
        }
    }
}
