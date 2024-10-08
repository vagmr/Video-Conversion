// This is the main program of the VideoConverter project.
using Serilog;
using VideoConverter.Utilities;

namespace VideoConverter
{
    class Program
    {
        private const string Version = "1.1.0";

        static void Main(string[] args)
        {
            try
            {
                LoggerConfig.SetupLogger();

                if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
                {
                    ConsoleHelper.PrintHelp(Version);
                    return;
                }

                if (args.Contains("-v") || args.Contains("--version"))
                {
                    Console.WriteLine($"VideoConverter 版本 {Version}");
                    return;
                }

                var config = ConfigManager.LoadConfig();
                var parser = new ArgumentParser(args, config);
                var converter = new VideoConverter(parser);
                converter.Convert();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                Log.Error(ex, "程序执行过程中发生错误");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}