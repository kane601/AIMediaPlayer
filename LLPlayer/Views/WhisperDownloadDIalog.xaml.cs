using System.Windows;
using System.Windows.Controls;
using AIMediaPlayer.ViewModels;

namespace AIMediaPlayer.Views;

public partial class WhisperDownloadDialog : UserControl
{
    public WhisperDownloadDialog()
    {
        InitializeComponent();

        DataContext = ((App)Application.Current).Container.Resolve<WhisperDownloadDialogVM>();
    }
}
