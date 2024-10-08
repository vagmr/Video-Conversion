namespace VideoConverter.Utilities
{
    public static class Validator
    {
        public static void ValidateInputFile(string inputFile)
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

        public static void ValidateCrf(int crf, int minCrf, int maxCrf)
        {
            if (crf < minCrf || crf > maxCrf)
            {
                throw new ArgumentOutOfRangeException(nameof(crf), $"CRF 值必须在 {minCrf} 到 {maxCrf} 之间");
            }
        }

        public static void ValidatePreset(string preset, string[] validPresets)
        {
            if (!Array.Exists(validPresets, p => p.Equals(preset, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"无效的 preset 值: {preset}。有效值为: {string.Join(", ", validPresets)}", nameof(preset));
            }
        }

        public static void ValidateAudioCodec(string audioCodec, string[] validAudioCodecs)
        {
            if (!Array.Exists(validAudioCodecs, c => c.Equals(audioCodec, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException($"无效的音频编码格式: {audioCodec}。有效值为: {string.Join(", ", validAudioCodecs)}", nameof(audioCodec));
            }
        }

        public static void ValidateResolution(string? resolution)
        {
            if (!string.IsNullOrEmpty(resolution))
            {
                if (!int.TryParse(resolution, out int resolutionValue) || resolutionValue <= 0)
                {
                    throw new ArgumentException("分辨率必须是正整数", nameof(resolution));
                }
            }
        }

        public static void ValidateOutputFile(string outputFile)
        {
            var outputDirectory = Path.GetDirectoryName(outputFile);
            if (!Directory.Exists(outputDirectory))
            {
                throw new DirectoryNotFoundException($"输出目录不存在: {outputDirectory}");
            }
        }
    }
}