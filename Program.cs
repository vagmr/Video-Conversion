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
                        parser.Resolution,
                        parser.Encoder
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
            Console.WriteLine("  -p, --preset [值]          设置编码速度 (0-8 或名称, 默认: 3 即 'fast')");
            Console.WriteLine("                             0:default, 1:slow, 2:medium, 3:fast,");
            Console.WriteLine("                             4:hp, 5:hq, 12:p1, 13:p2, 14:p3, 15:p4,");
            Console.WriteLine("                             16:p5, 17:p6, 18:p7 等");
            Console.WriteLine("  -ac, --audio-codec [值]    设置音频编码格式 (选项: aac, copy, mp3, 默认: aac)");
            Console.WriteLine("  -r, --resolution [值]      设置视频分辨率 (例如：720, 480)");
            Console.WriteLine("  -h, --help                 显示帮助信息");
            Console.WriteLine("  -e, --encoder [值]        设置编码器 (选项: nvenc, libx265, 默认: nvenc)");
            Console.WriteLine("\n示例:");
            Console.WriteLine("  VideoConverter input.mp4 -o output.mp4 -c 24 -p 4 -ac copy -r 720");
        }
    }
}