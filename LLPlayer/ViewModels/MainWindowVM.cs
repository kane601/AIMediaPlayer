﻿using System.IO;
using System.Windows.Shell;
using FlyleafLib;
using FlyleafLib.MediaPlayer;
using LLPlayer.Extensions;
using LLPlayer.Services;
using InputType = FlyleafLib.InputType;

namespace LLPlayer.ViewModels;

public class MainWindowVM : Bindable
{
    public FlyleafManager FL { get; }
    private readonly LogHandler Log;

    public MainWindowVM(FlyleafManager fl)
    {
        FL = fl;
        Log = new LogHandler("[App] [MainWindowVM  ] ");
    }

    public string Title { get; set => Set(ref field, value); } = App.Name;

    #region Progress in TaskBar
    public double TaskBarProgressValue
    {
        get;
        set
        {
            double v = value;
            if (v < 0.01)
            {
                // Set to 1% because it is not displayed.
                v = 0.01;
            }
            Set(ref field, v);
        }
    }

    public TaskbarItemProgressState TaskBarProgressState { get; set => Set(ref field, value); }
    #endregion

    // ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
    public DelegateCommand CmdOnLoaded => field ??= new(() =>
    {
        // error handling
        FL.Player.KnownErrorOccurred += (sender, args) =>
        {
            Utils.UI(() =>
            {
                Log.Error($"Known error occurred in Flyleaf: {args.Message} ({args.ErrorType.ToString()})");
                ErrorDialogHelper.ShowKnownErrorPopup(args.Message, args.ErrorType);
            });
        };

        FL.Player.UnknownErrorOccurred += (sender, args) =>
        {
            Utils.UI(() =>
            {
                Log.Error($"Unknown error occurred in Flyleaf: {args.Message}: {args.Exception}");
                ErrorDialogHelper.ShowUnknownErrorPopup(args.Message, args.ErrorType, args.Exception);
            });
        };

        FL.Player.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == nameof(FL.Player.CurTime))
            {
                double prevValue = TaskBarProgressValue;
                double newValue = (double)FL.Player.CurTime / FL.Player.Duration;

                if (Math.Abs(newValue - prevValue) >= 0.01) // prevent frequent update
                {
                    TaskBarProgressValue = newValue;
                }
            }
            if (args.PropertyName == nameof(FL.Player.Status))
            {
                switch (FL.Player.Status)
                {
                    case Status.Stopped:
                        // reset
                        Title = App.Name;
                        TaskBarProgressState = TaskbarItemProgressState.None;
                        TaskBarProgressValue = 0;
                        break;
                    case Status.Playing:
                        TaskBarProgressState = TaskbarItemProgressState.Normal;
                        break;
                    case Status.Opening:
                        TaskBarProgressState = TaskbarItemProgressState.Indeterminate;
                        TaskBarProgressValue = 0;
                        break;
                    case Status.Paused:
                        TaskBarProgressState = TaskbarItemProgressState.Paused;
                        break;
                    case Status.Ended:
                        TaskBarProgressState = TaskbarItemProgressState.Paused;
                        TaskBarProgressValue = 1;
                        break;
                    case Status.Failed:
                        TaskBarProgressState = TaskbarItemProgressState.Error;
                        break;
                }
            }
        };

        FL.Player.OpenCompleted += (sender, args) =>
        {
            if (!args.Success || args.IsSubtitles)
            {
                return;
            }

            string name = Path.GetFileName(args.Url);
            if (FL.Player.Playlist.InputType == InputType.Web)
            {
                name = FL.Player.Playlist.Selected.Title;
            }
            Title = $"{name} - {App.Name}";
            TaskBarProgressValue = 0;
            TaskBarProgressState = TaskbarItemProgressState.Normal;
        };

        if (App.CmdUrl != null)
        {
            FL.Player.OpenAsync(App.CmdUrl);
        }
    });

    public DelegateCommand CmdOnClosing => field ??= new(() =>
    {
        FL.Player.Dispose();
    });

    // ReSharper restore NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
}
