namespace VideoConverter.Utilities
{
    public static class ConsoleHelper
    {
        public static void PrintHelp(string version)
        {
            Console.WriteLine($"VideoConverter 版本 {version}");
            Console.WriteLine("用法: VideoConverter [输入文件] [选项]");
            Console.WriteLine("选项:");
            Console.WriteLine("  -o, --output [文件路径]    指定输出文件路径 (默认随机生成)");
            Console.WriteLine("  --crf [值]                 设置视频质量 (默认: 28)");
            Console.WriteLine("  --preset [值]              设置编码速度 (默认: slow)");
            Console.WriteLine("  --audio-codec [值]         设置音频编码格式 (默认: aac)");
            Console.WriteLine("  -r, --resolution [值]      设置视频分辨率 (例如：720, 480)");
            Console.WriteLine("  -c, --config [文件路径]    指定配置文件路径");
            Console.WriteLine("  -v, --version              显示版本信息");
            Console.WriteLine("  -h, --help                 显示帮助信息");
            Console.WriteLine("\n示例:");
            Console.WriteLine("  VideoConverter input.mp4 -o output.mp4 --crf 24 --preset fast --audio-codec copy --resolution 720");
        }
    }
}