using DormitoryCounter.Implementation;
using DormitoryCounter.ViewModel;
using Microsoft.Win32;
using System;
using System.Windows;

namespace DormitoryCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel)DataContext;
            ParseArgs(Environment.GetCommandLineArgs());
            App.Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
        }

        public void ParseArgs(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                var nextArg = i + 1 == args.Length ? string.Empty : args[i + 1];
                if (arg == "--user")
                {
                    _viewModel.User = nextArg;
                }
                if (arg == "--password")
                {
                    _viewModel.Password = nextArg;
                }
                if (arg == "--filename")
                {
                    _viewModel.OutputFileName = nextArg;
                }
            }
        }

        private async void StartCaptureData_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_viewModel.OutputFileName))
            {
                MessageBox.Show("请选择导出位置", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var result = await RequestClient.Query(_viewModel.User, _viewModel.Password, _viewModel.StartTime, _viewModel.EndTime, _viewModel.OutputFileName, _viewModel.OrderByDescending);
            if (result) MessageBox.Show("导出完成", "信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EditOutputTargetButton_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "*.xlsx|Xlsx",
                Title = "保存",
                DefaultExt = "xlsx",
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                _viewModel.OutputFileName = saveFileDialog.FileName;
            }
        }
    }
}
