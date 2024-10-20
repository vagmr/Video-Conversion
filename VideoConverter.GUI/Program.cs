
using System.Windows;

namespace VideoConverter.GUI
{
    public class Program
    {
        [STAThread]
        public static void Main()
        {
            var application = new Application
            {
                StartupUri = new Uri("MainWindow.xaml", UriKind.Relative)
            };
            application.Run();
        }
    }
}
