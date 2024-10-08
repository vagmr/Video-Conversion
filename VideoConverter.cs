// This is the main class of the VideoConverter project.
using System.Diagnostics;
using System.Text.RegularExpressions;
using Serilog;
using VideoConverter.Utilities;

namespace VideoConverter
{
    public class VideoConverter
    {
        private readonly ArgumentParser _args;
        private static readonly string[] ValidPresets = { "ultrafast", "superfast", "veryfast", "faster", "fast", "medium", "slow", "slower", "veryslow" };
        private static readonly string[] ValidAudioCodecs = { "aac", "copy", "mp3" };
        private const int MinCrf = 0;
        private const int MaxCrf = 51;

        public VideoConverter(ArgumentParser args)
        {
            _args = args;
        }

        public void Convert()
        {
            ValidateInputParameters();
            ValidateOutputFile();

            Console.WriteLine($"开始转换：{_args.InputFile} => {_args.OutputFile}");
            Log.Information("开始转换 {InputFile} => {OutputFile}", _args.InputFile, _args.OutputFile);

            var resolutionOption = string.IsNullOrEmpty(_args.Resolution) ? "" : $"-vf scale=-2:{_args.Resolution}";
            var ffmpegCommand = CreateFfmpegCommand(resolutionOption);
            ExecuteFfmpegCommand(ffmpegCommand);
        }

        private void ValidateInputParameters()
        {
            Validator.ValidateInputFile(_args.InputFile);
            Validator.ValidateCrf(_args.Crf, MinCrf, MaxCrf);
            Validator.ValidatePreset(_args.Preset, ValidPresets);
            Validator.ValidateAudioCodec(_args.AudioCodec, ValidAudioCodecs);
            Validator.ValidateResolution(_args.Resolution);
        }

        private void ValidateOutputFile()
        {
            Validator.ValidateOutputFile(_args.OutputFile);
        }

        private ProcessStartInfo CreateFfmpegCommand(string resolutionOption)
        {
            return new ProcessStartInfo
            {
                FileName = "ffmpeg",
                Arguments = $"-y -i \"{_args.InputFile}\" -c:v hevc_nvenc -preset {_args.Preset} {resolutionOption} -crf {_args.Crf} -c:a {_args.AudioCodec} -progress pipe:1 \"{_args.OutputFile}\"",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
        }

        private void ExecuteFfmpegCommand(ProcessStartInfo ffmpegCommand)
        {
            using var process = new Process { StartInfo = ffmpegCommand };
            var progressRegex = new Regex(@"out_time_ms=(\d+)");
            var durationRegex = new Regex(@"Duration: (\d{2}):(\d{2}):(\d{2}\.\d{2})");
            TimeSpan duration = TimeSpan.Zero;

            process.ErrorDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    Log.Debug(e.Data);
                    var match = durationRegex.Match(e.Data);
                    if (match.Success)
                    {
                        duration = TimeSpan.Parse($"{match.Groups[1].Value}:{match.Groups[2].Value}:{match.Groups[3].Value}");
                    }
                }
            };

            process.OutputDataReceived += (sender, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                {
                    var match = progressRegex.Match(e.Data);
                    if (match.Success && duration != TimeSpan.Zero)
                    {
                        var progress = TimeSpan.FromMilliseconds(long.Parse(match.Groups[1].Value));
                        var percentage = (progress.TotalSeconds / duration.TotalSeconds) * 100;
                        Console.Write($"\r转换进度: {percentage:F2}%");
                    }
                }
            };

            try
            {
                Console.WriteLine($"执行命令: {ffmpegCommand.FileName} {ffmpegCommand.Arguments}");
                Log.Information("执行FFmpeg命令");
                process.Start();
                process.BeginErrorReadLine();
                process.BeginOutputReadLine();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"\n转换成功：{_args.OutputFile}");
                    Log.Information("转换成功: {OutputFile}", _args.OutputFile);
                }
                else
                {
                    throw new Exception($"转换失败，FFmpeg 返回码：{process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n转换过程中发生错误：{ex.Message}");
                Log.Error(ex, "转换过程中发生错误");
                throw;
            }
        }
    }
}