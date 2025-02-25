﻿using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FlyleafLib.MediaPlayer.Translation.Services;
using AIMediaPlayer.Extensions;
using AIMediaPlayer.Services;

namespace AIMediaPlayer.Controls.Settings;

public partial class SettingsSubtitlesAction : UserControl
{
    public SettingsSubtitlesAction()
    {
        InitializeComponent();

        DataContext = ((App)Application.Current).Container.Resolve<SettingsSubtitlesActionVM>();
    }

    private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
        // TODO: L: refactor
        try
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Failed to open link" + ex.Message);
        }
    }
}

public class SettingsSubtitlesActionVM : Bindable
{
    public FlyleafManager FL { get; }

    public SettingsSubtitlesActionVM(FlyleafManager fl)
    {
        FL = fl;

        SelectedTranslateWordServiceType = FL.PlayerConfig.Subtitles.TranslateWordServiceType;

        List<WordClickAction> wordClickActions = Enum.GetValues<WordClickAction>().ToList();
        if (string.IsNullOrEmpty(FL.Config.Subs.PDICPipeExecutablePath))
        {
            // PDIC is enabled only when exe is configured
            wordClickActions.Remove(WordClickAction.PDIC);
        }
        WordClickActions = wordClickActions;

        foreach (IMenuAction menuAction in FL.Config.Subs.WordMenuActions)
        {
            MenuActions.Add((IMenuAction)menuAction.Clone());
        }
    }

    public TranslateServiceType SelectedTranslateWordServiceType
    {
        get;
        set
        {
            if (Set(ref field, value))
            {
                FL.PlayerConfig.Subtitles.TranslateWordServiceType = value;
            }
        }
    }

    public List<WordClickAction> WordClickActions { get; }

    public List<ModifierKeys> ModifierKeys { get; } =
    [
        System.Windows.Input.ModifierKeys.Control,
        System.Windows.Input.ModifierKeys.Shift,
        System.Windows.Input.ModifierKeys.Alt,
        System.Windows.Input.ModifierKeys.None
    ];

    public ObservableCollection<IMenuAction> MenuActions { get; } = new();

    // ReSharper disable NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract

    public DelegateCommand CmdApplyContextMenu => field ??= new(() =>
    {
        ObservableCollection<IMenuAction> newActions = new(MenuActions.Select(a => (IMenuAction)a.Clone()));

        // Apply to config
        FL.Config.Subs.WordMenuActions = newActions;
    });

    public DelegateCommand CmdAddSearchAction => field ??= new(() =>
    {
        MenuActions.Add(new SearchMenuAction
        {
            Title = "New Search",
            Url = "https://example.com/?q=%w"
        });
    });

    public DelegateCommand CmdAddClipboardAction => field ??= new(() =>
    {
        MenuActions.Add(new ClipboardMenuAction());
    });

    public DelegateCommand CmdAddClipboardAllAction => field ??= new(() =>
    {
        MenuActions.Add(new ClipboardAllMenuAction());
    });

    public DelegateCommand<IMenuAction> CmdRemoveAction => field ??= new((action) =>
    {
        if (action != null)
        {
            MenuActions.Remove(action);
        }
    });

    // TODO: L: SaveCommand?

    // ReSharper restore NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
}

class DataGridRowOrderBehaviorMenuAction : DataGridRowOrderBehavior<IMenuAction>;

class MenuActionTemplateSelector : DataTemplateSelector
{
    public DataTemplate SearchTemplate { get; set; }
    public DataTemplate ClipboardTemplate { get; set; }
    public DataTemplate ClipboardAllTemplate { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        return item switch
        {
            SearchMenuAction => SearchTemplate,
            ClipboardMenuAction => ClipboardTemplate,
            ClipboardAllMenuAction => ClipboardAllTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
