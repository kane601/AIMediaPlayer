﻿using System.Windows;
using System.Windows.Controls;
using AIMediaPlayer.ViewModels;

namespace AIMediaPlayer.Resources;

public partial class PopupMenu : ResourceDictionary
{
    public PopupMenu()
    {
        InitializeComponent();
    }

    private void PopUpMenu_OnOpened(object sender, RoutedEventArgs e)
    {
        // TODO: L: should validate that the clipboard content is a video file?
        bool canPaste = !string.IsNullOrEmpty(Clipboard.GetText());
        MenuPasteUrl.IsEnabled = canPaste;

        // Don't hide the seek bar while displaying the context menu
        if (sender is ContextMenu menu && menu.DataContext is FlyleafOverlayVM vm)
        {
            vm.FL.Player.Activity.IsEnabled = false;
        }
    }

    private void PopUpMenu_OnClosed(object sender, RoutedEventArgs e)
    {
        if (sender is ContextMenu menu && menu.DataContext is FlyleafOverlayVM vm)
        {
            vm.FL.Player.Activity.IsEnabled = true;
        }
    }
}
