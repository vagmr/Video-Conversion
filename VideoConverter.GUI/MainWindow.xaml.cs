using VideoConverter.Core;
using System.Windows;
using MessageBox = System.Windows.MessageBox;

namespace VideoConverter.GUI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitializeComboBoxes();
        }

        private  void InitializeComboBoxes()
        {
            PresetComboBox.ItemsSource = Constants.ValidPresetsForNvenc.Concat(Constants.ValidPresetsForLibx265);
            AudioCodecComboBox.ItemsSource = Constants.ValidAudioCodecs;
            ResolutionComboBox.ItemsSource = new[] { "原始", "720", "480" };
            EncoderComboBox.ItemsSource = new[] { "nvenc", "libx265" };

            PresetComboBox.SelectedIndex = 0;
            AudioCodecComboBox.SelectedIndex = 0;
            ResolutionComboBox.SelectedIndex = 0;
            EncoderComboBox.SelectedIndex = 0;
        }

        private void SelectFiles_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true,
                Filter = "视频文件|*.mp4;*.avi;*.mkv|所有文件|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                FileListBox.ItemsSource = openFileDialog.FileNames;
            }
        }

        private async void StartConversion_Click(object sender, RoutedEventArgs e)
        {
            if (FileListBox.Items.Count == 0)
            {
                MessageBox.Show("请先选择要转换的文件。", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var inputFiles = FileListBox.Items.Cast<string>().ToList();
            var crf = (int)CrfSlider.Value;
            var preset = PresetComboBox.SelectedItem as string;
            var audioCodec = AudioCodecComboBox.SelectedItem as string;
            var resolution = ResolutionComboBox.SelectedItem as string;
            var encoder = EncoderComboBox.SelectedItem as string;

            try
            {
                foreach (var inputFile in inputFiles)
                {
                    await Core.VideoConverter.ConvertAsync(inputFile, null, crf, preset!, audioCodec!, resolution == "原始" ? null : resolution, encoder!);
                }
                MessageBox.Show("转换完成！", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"转换过程中发生错误：{ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
