namespace VideoConverter
{
    class Program
    {
        private const string Version = "0.0.3";
        private const string Author = "vagmr";
        private const string RepositoryUrl = "https://github.com/vagmr/VideoConverter";

        static void Main(string[] args)
        {
            try
            {
                PrintVersionInfo();

                if (args.Contains("-h") || args.Contains("--help"))
                {
                    PrintHelp();
                    return;
                }

                var parser = new ArgumentParser(args);
                foreach (var inputFile in parser.InputFiles)
                {
                    VideoConverter.ConvertToHevc(
                        inputFile,
                        parser.OutputFile,
                        parser.Crf,
                        parser.Preset,
                        parser.AudioCodec,
                        parser.Resolution,
                        parser.Encoder,
                        parser.Bitrate
                    );
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
        }

        static void PrintVersionInfo()
        {
            Console.WriteLine($"VideoConverter CLI 版本: {Version}");
            Console.WriteLine($"作者: {Author}");
            Console.WriteLine($"仓库地址: {RepositoryUrl}");
            Console.WriteLine();
        }

        static void PrintHelp()
        {
            PrintVersionInfo();
            Console.WriteLine("用法: VideoConverter [输入文件] [选项]");
            Console.WriteLine("选项:");
            Console.WriteLine("  -o, --output [文件路径]    指定输出文件路径,可以省略 (默认随机生成)");
            Console.WriteLine("  -c, --crf [值]             设置视频质量 (范围: 0-51, 默认: 28)");
            Console.WriteLine("  -p, --preset [值]          设置编码速度 (根据编码器不同而异，见下方说明)");
            Console.WriteLine("  -ac, --audio-codec [值]    设置音频编码格式 (选项: aac, copy, mp3, 默认: aac)");
            Console.WriteLine("  -r, --resolution [值]      设置视频分辨率 (例如：720, 480)");
            Console.WriteLine("  -e, --encoder [值]         设置编码器 (选项: nvenc, libx265, 默认: nvenc)");
            Console.WriteLine("  -b, --bitrate [值]         设置视频比特率 (例如：5M, 10M,100k, 默认: 自动)");
            Console.WriteLine("  -h, --help                 显示帮助信息");
            Console.WriteLine("\n编码器预设 (preset) 说明,详细说明可查询文档或仓库:");
            Console.WriteLine("  对于 nvenc 编码器:");
            Console.WriteLine("    可用预设: default, slow, medium, fast, hp, hq, bd, ll, llhq, llhp, lossless,");
            Console.WriteLine("              losslesshp, p1, p2, p3, p4, p5, p6, p7");
            Console.WriteLine("    或者使用数字 0-18 (0:default, 1:slow, 2:medium, 3:fast, ...)");
            Console.WriteLine("  对于 libx265 编码器:");
            Console.WriteLine("    可用预设: ultrafast, superfast, veryfast, faster, fast, medium, slow, slower, veryslow");
            Console.WriteLine("    或者使用数字 0-8 (0:ultrafast, 1:superfast, 2:veryfast, ...)");
            Console.WriteLine("\n示例:");
            Console.WriteLine("  VideoConverter input.mp4 -o output.mp4 -c 24 -p fast -ac copy -r 720 -e nvenc -b 5M");
            Console.WriteLine("  VideoConverter input.mp4 -e libx265 -p medium -c 28");
        }
    }
}
