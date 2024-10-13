namespace VideoConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
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
                        parser.Resolution
                    );
                }

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
            Console.WriteLine("  -c, --crf [值]             设置视频质量 (范围: 0-51, 默认: 28)");
            Console.WriteLine("  -p, --preset [值]          设置编码速度 (0-8 或名称, 默认: 6 即 'slow')");
            Console.WriteLine("                             0:ultrafast, 1:superfast, 2:veryfast, 3:faster,");
            Console.WriteLine("                             4:fast, 5:medium, 6:slow, 7:slower, 8:veryslow");
            Console.WriteLine("  -ac, --audio-codec [值]    设置音频编码格式 (选项: aac, copy, mp3, 默认: aac)");
            Console.WriteLine("  -r, --resolution [值]      设置视频分辨率 (例如：720, 480)");
            Console.WriteLine("  -h, --help                 显示帮助信息");
            Console.WriteLine("\n示例:");
            Console.WriteLine("  VideoConverter input.mp4 -o output.mp4 -c 24 -p 4 -ac copy -r 720");
        }
    }
}