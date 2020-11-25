﻿﻿//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
using System;
using System.Linq;
using Windows.System;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace AppUIBasics
{
    /// <summary>
    /// A page that displays the app's settings.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public string Version
        {
            get
            {
                var version = Windows.ApplicationModel.Package.Current.Id.Version;
                return String.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
            }
        }

        public SettingsPage()
        {
            this.InitializeComponent();
            Loaded += OnSettingsPageLoaded;

#if NETFX_CORE // UNO TODO
            if (ElementSoundPlayer.State == ElementSoundPlayerState.On)
                soundToggle.IsOn = true;
            if (ElementSoundPlayer.SpatialAudioMode == ElementSpatialAudioMode.On)
                spatialSoundBox.IsChecked = true;

            if (NavigationRootPage.Current.NavigationView.PaneDisplayMode == Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto)
            {
                navigationLocation.SelectedIndex = 0;
            }
            else
            {
                navigationLocation.SelectedIndex = 1;
            }

            screenshotModeToggle.IsOn = UIHelper.IsScreenshotMode;
            screenshotFolderLink.Content = UIHelper.ScreenshotStorageFolder.Path;
#endif
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            NavigationRootPage.Current.NavigationView.Header = "Settings";
        }

        private async void OnFeedbackButtonClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("feedback-hub:"));
        }

        private void OnSettingsPageLoaded(object sender, RoutedEventArgs e)
        {
            var currentTheme = App.RootTheme.ToString();
            (ThemePanel.Children.Cast<RadioButton>().FirstOrDefault(c => c?.Tag?.ToString() == currentTheme)).IsChecked = true;
        }

        private void OnThemeRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var selectedTheme = ((RadioButton)sender)?.Tag?.ToString();
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;

            if (selectedTheme != null)
            {
                App.RootTheme = App.GetEnum<ElementTheme>(selectedTheme);
                if (selectedTheme == "Dark")
                {
                    titleBar.ButtonForegroundColor = Colors.White;
                }
                else if (selectedTheme == "Light")
                {
                    titleBar.ButtonForegroundColor = Colors.Black;
                }
                else
                {
                    if (Application.Current.RequestedTheme == ApplicationTheme.Dark)
                    {
                        titleBar.ButtonForegroundColor = Colors.White;
                    }
                    else
                    {
                        titleBar.ButtonForegroundColor = Colors.Black;
                    }
                }
            }
        }

        private void OnThemeRadioButtonKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Up)
            {
                NavigationRootPage.Current.PageHeader.Focus(FocusState.Programmatic);
            }
        }
        private void spatialSoundBox_Checked(object sender, RoutedEventArgs e)
        {
#if NETFX_CORE // UNO TODO
            if(soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.On;
            }
#endif
        }

        private void soundToggle_Toggled(object sender, RoutedEventArgs e)
        {
#if NETFX_CORE // UNO TODO
            if (soundToggle.IsOn == true)
            {
                spatialSoundBox.IsEnabled = true;
                ElementSoundPlayer.State = ElementSoundPlayerState.On;
            }
            else
            {
                spatialSoundBox.IsEnabled = false;
                spatialSoundBox.IsChecked = false;

                ElementSoundPlayer.State = ElementSoundPlayerState.Off;
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
#endif
        }

        private void navigationToggle_Toggled(object sender, RoutedEventArgs e)
        {
            NavigationOrientationHelper.IsLeftMode = navigationLocation.SelectedIndex == 0;
        }

        private void screenshotModeToggle_Toggled(object sender, RoutedEventArgs e)
        {
            UIHelper.IsScreenshotMode = screenshotModeToggle.IsOn;
        }

        private void spatialSoundBox_Unchecked(object sender, RoutedEventArgs e)
        {
#if NETFX_CORE // UNO TODO
            if (soundToggle.IsOn == true)
            {
                ElementSoundPlayer.SpatialAudioMode = ElementSpatialAudioMode.Off;
            }
#endif
        }

        private void navigationLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NavigationOrientationHelper.IsLeftMode = navigationLocation.SelectedIndex == 0;
        }

        private async void FolderButton_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            folderPicker.FileTypeFilter.Add(".png"); // meaningless, but you have to have something
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder != null)
            {
                UIHelper.ScreenshotStorageFolder = folder;
                screenshotFolderLink.Content = UIHelper.ScreenshotStorageFolder.Path;
            }
        }

        private async void screenshotFolderLink_Click(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(UIHelper.ScreenshotStorageFolder);
        }
    }
}
