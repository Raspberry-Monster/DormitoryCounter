using DormitoryCounter.Implementation;
using DormitoryCounter.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DormitoryCounter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = (MainWindowViewModel)DataContext;           
            ParseArgs(Environment.GetCommandLineArgs());
        }
        public void ParseArgs(string[] args)
        {
            MessageBox.Show(args.Length.ToString());
            for (int i =0; i <args.Length; i++)
            {
                var arg = args[i];
                var nextArg = i+1 == args.Length ? string.Empty: args[i+1];
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
            var result = await RequestClient.Query(_viewModel.User, _viewModel.Password, _viewModel.StartTime, _viewModel.EndTime, _viewModel.OutputFileName);
        }
    }
}
