using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DormitoryCounter.ViewModel
{
    public partial class MainWindowViewModel:ObservableObject
    {
        [ObservableProperty]
        private string user = string.Empty;
        [ObservableProperty]
        private string password = string.Empty;
        [ObservableProperty]
        private DateOnly startTime = DateOnly.FromDateTime(DateTime.Now);
        [ObservableProperty]
        private DateOnly endTime = DateOnly.FromDateTime(DateTime.Now);
        [ObservableProperty]
        private string outputFileName = string.Empty;
    }
}
