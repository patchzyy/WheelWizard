﻿using CT_MKWII_WPF.Models.Settings;
using CT_MKWII_WPF.Services.Settings;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace CT_MKWII_WPF.Views.Pages.Settings;

public partial class VideoSettings : UserControl
{
    public VideoSettings()
    {
        InitializeComponent();
        UpdateSettingsState();
        PopulateRenderers();
        InitializeRendererDropdown();
    }

    private void PopulateRenderers()
    {
        foreach (var renderer in SettingValues.GFXRenderers.AllRenderers)
        {
            RendererDropdown.Items.Add(renderer);
        }
    }

    private void UpdateSettingsState()
    {
        var enableDolphinSettings = SettingsHelper.PathsSetupCorrectly();
        VideoBorder.IsEnabled = enableDolphinSettings;
        if (!enableDolphinSettings)
            return;

        VSyncButton.IsChecked = (bool)SettingsManager.VSYNC.Get();
        RecommendedButton.IsChecked = (bool)SettingsManager.RECOMMENDED_SETTINGS.Get();
        Button30FPS.IsChecked = (bool)SettingsManager.FORCE_30FPS.Get();
        ShowFPSButton.IsChecked = (bool)SettingsManager.SHOW_FPS.Get();
        var finalResolution = (int)SettingsManager.INTERNAL_RESOLUTION.Get();
        if (finalResolution < 0 || finalResolution > ResolutionStackPanel.Children.Count)
            return;

        foreach (var element in ResolutionStackPanel.Children)
        {
            if (element is not RadioButton radioButton)
                continue;
            radioButton.IsChecked = (radioButton.Tag.ToString() == finalResolution.ToString());
        }

    }

    private void UpdateResolution(object sender, RoutedEventArgs e)
    {
        if (sender is not RadioButton radioButton)
            return;

        SettingsManager.INTERNAL_RESOLUTION.Set(int.Parse(radioButton.Tag.ToString()!));
    }

    private void VSync_OnClick(object sender, RoutedEventArgs e) =>
        SettingsManager.VSYNC.Set(VSyncButton.IsChecked == true);

    private void Recommended_OnClick(object sender, RoutedEventArgs e) =>
        SettingsManager.RECOMMENDED_SETTINGS.Set(RecommendedButton.IsChecked == true);

    private void _30FPS_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsManager.FORCE_30FPS.Set(Button30FPS.IsChecked == true);
    }

    private void ShowFPS_OnClick(object sender, RoutedEventArgs e)
    {
        SettingsManager.SHOW_FPS.Set(ShowFPSButton.IsChecked == true);
    }

    private void RendererDropdown_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selectedDisplayName = RendererDropdown.SelectedItem.ToString();
        if (SettingValues.GFXRenderers.RendererMapping.TryGetValue(selectedDisplayName, out string actualValue))
        {
            SettingsManager.GFX_BACKEND.Set(actualValue);
        }
        else
        {
            Console.WriteLine($"Warning: Unknown renderer selected: {selectedDisplayName}");
        }
    }

    private void InitializeRendererDropdown()
    {
        string currentRenderer = (string)SettingsManager.GFX_BACKEND.Get();
    
        // Find the display name for the current renderer value
        string displayName = SettingValues.GFXRenderers.RendererMapping
            .FirstOrDefault(x => x.Value == currentRenderer).Key;

        if (displayName != null)
        {
            // Set the selected item in the dropdown
            RendererDropdown.SelectedItem = displayName;
        }
    }

}

