using System.Windows;
using System.Windows.Controls;
using AIMediaPlayer.ViewModels;

namespace AIMediaPlayer.Views;

public partial class SubtitlesDownloaderDialog : UserControl
{
    public SubtitlesDownloaderDialog()
    {
        InitializeComponent();

        DataContext = ((App)Application.Current).Container.Resolve<SubtitlesDownloaderDialogVM>();
    }
}
