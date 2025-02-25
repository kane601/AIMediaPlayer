﻿using System.Collections;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AIMediaPlayer.ViewModels;

namespace AIMediaPlayer.Controls.Settings;

public partial class SettingsPlugins : UserControl
{
    public SettingsPlugins()
    {
        InitializeComponent();
    }

    private void PluginValueChanged(object sender, RoutedEventArgs e)
    {
        string curPlugin = ((TextBlock)((Panel)((FrameworkElement)sender).Parent).Children[0]).Text;

        if (DataContext is SettingsDialogVM vm)
        {
            vm.FL.PlayerConfig.Plugins[cmbPlugins.Text][curPlugin] = ((TextBox)sender).Text;
        }
    }
}

public class GetDictionaryItemConverter : IMultiValueConverter
{
    public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return null;
        if (value[0] == null || value[0] == DependencyProperty.UnsetValue)
            return null;
        if (value[1] == null || value[1] == DependencyProperty.UnsetValue)
            return null;

        return ((IDictionary)value[0])[value[1]];
    }
    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture) { throw new NotImplementedException(); }
}
