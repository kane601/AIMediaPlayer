﻿using System.Windows;
using FlyleafLib.MediaPlayer;
using AIMediaPlayer.Views;

namespace AIMediaPlayer.Services;

public static class ErrorDialogHelper
{
    public static void ShowKnownErrorPopup(string message, string errorType)
    {
        var dialogService = ((App)Application.Current).Container.Resolve<DialogService>();

        DialogParameters p = new()
        {
            { "type", "known" },
            { "message", message },
            { "errorType", errorType }
        };

        dialogService.ShowDialog(nameof(ErrorDialog), p);
    }

    public static void ShowKnownErrorPopup(string message, KnownErrorType errorType)
    {
        ShowKnownErrorPopup(message, errorType.ToString());
    }

    public static void ShowUnknownErrorPopup(string message, string errorType, Exception? ex = null)
    {
        var dialogService = ((App)Application.Current).Container.Resolve<DialogService>();

        DialogParameters p = new()
        {
            { "type", "unknown" },
            { "message", message },
            { "errorType", errorType },
        };

        if (ex != null)
        {
            p.Add("exception", ex);
        }

        dialogService.ShowDialog(nameof(ErrorDialog), p);
    }

    public static void ShowUnknownErrorPopup(string message, UnknownErrorType errorType, Exception? ex = null)
    {
        ShowUnknownErrorPopup(message, errorType.ToString(), ex);
    }
}
