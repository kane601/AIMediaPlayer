using System.Windows;
using AIMediaPlayer.Views;

namespace AIMediaPlayer.Extensions;

public partial class MyDialogWindow : Window, IDialogWindow
{
    public IDialogResult Result { get; set; }

    public MyDialogWindow()
    {
        InitializeComponent();

        MainWindow.SetTitleBarDarkMode(this);
    }
}
