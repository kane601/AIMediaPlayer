using System.Windows;
using System.Windows.Controls;
using AIMediaPlayer.ViewModels;

namespace AIMediaPlayer.Views;

public partial class SubtitlesExportDialog : UserControl
{
    public SubtitlesExportDialog()
    {
        InitializeComponent();

        DataContext = ((App)Application.Current).Container.Resolve<SubtitlesExportDialogVM>();
    }
}
