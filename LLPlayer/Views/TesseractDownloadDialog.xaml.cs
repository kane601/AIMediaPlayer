using System.Windows;
using System.Windows.Controls;
using AIMediaPlayer.ViewModels;

namespace AIMediaPlayer.Views;
public partial class TesseractDownloadDialog : UserControl
{
    public TesseractDownloadDialog()
    {
        InitializeComponent();

        DataContext = ((App)Application.Current).Container.Resolve<TesseractDownloadDialogVM>();
    }
}
