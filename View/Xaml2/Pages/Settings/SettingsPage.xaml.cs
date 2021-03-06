using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Fork.Logic.Model.Settings;
using Fork.View.Xaml2.Pages.Settings;
using Fork.ViewModel;

namespace Fork.View.Xaml2.Pages.Settings
{
    public partial class SettingsPage : Page
    {
        private SettingsViewModel viewModel;

        public SettingsPage(SettingsViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            DataContext = viewModel;
        }

        private void FileSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is ISettingsPage settingsPage)
            {
                UpdateSettingsFrame(settingsPage);
            }
        }

        public void UpdateSettingsFrame(ISettingsPage settingsPage)
        {
            if (settingsPage == null)
            {
                return;
            }
            settingsFrame.Content = settingsPage;
        }
    }
}
