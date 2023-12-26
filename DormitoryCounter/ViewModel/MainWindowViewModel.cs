using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace DormitoryCounter.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
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
        [ObservableProperty]
        private bool orderByDescending = false;
    }
}
