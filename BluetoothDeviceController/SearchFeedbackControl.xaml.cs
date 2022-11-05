using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController
{
    public sealed partial class SearchFeedbackControl : UserControl, IDoSearchFeedback
    {
        public IDoSearch Search { get; set; }
        public SearchFeedbackControl()
        {
            this.InitializeComponent();
        }



        int nFound = 0;
        int nFoundAll = 0;

        /// <summary>
        /// Tells the main app to start a search; the main windows will eventually tell this control what's going on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSearch(object sender, RoutedEventArgs e)
        {
            if (Search == null) return;
            if (Search.GetSearchActive())
            {
                Search.CancelSearch();
            }
            else
            {
                // Is most likely MainPage.Xaml.cs about line 875: public void StartSearchWithUserPreferences()
                Search.StartSearchWithUserPreferences();
            }
        }

        public void StartSearchFeedback()
        {
            //System.Diagnostics.Debug.WriteLine($"SearchFeedback: started");
            uiProgress.ShowPaused = false;
            SetPauseVisibility(true);
            nFound = 0;
            nFoundAll = 0;
            UpdateUI();
        }



        public void StopSearchFeedback()
        {
            Utilities.UIThreadHelper.CallOnUIThread( () => {
                System.Diagnostics.Debug.WriteLine($"SearchFeedback: stopped");
                SetPauseVisibility(false);
                uiProgress.ShowPaused = true;
            });
        }

        public void FoundDevice(FoundDeviceInfo deviceInfo)
        {
            nFoundAll++;
            if (deviceInfo == FoundDeviceInfo.IsNew)
            {
                nFound++;
            }
            UpdateUI();

        }

        private void UpdateUI()
        {
            Utilities.UIThreadHelper.CallOnUIThread(() => {
                uiFound.Text = nFound.ToString();
                uiFoundAll.Text = (nFoundAll - nFound).ToString();
            });
        }

        private void OnFilterClick(object sender, PointerRoutedEventArgs e)
        {
            Search?.ListFilteredOut();
        }

        /// <summary>
        /// Returns TRUE iff the pause checkbox is checked AND the control is visible. It should never be the case that it's invisible and checked
        /// and the checked state matters.
        /// </summary>
        public bool GetIsPaused() 
        { 
            var retval = uiPause.IsChecked.GetValueOrDefault(false) && uiPause.Visibility == Visibility.Visible;
            return retval;
        }
        public void SetPauseVisibility(bool visible)
        {
            uiPause.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OnPauseChecked(object sender, RoutedEventArgs e)
        {
            Search.PauseCheckUpdated(true);
        }

        private void OnPauseUnchecked(object sender, RoutedEventArgs e)
        {
            Search.PauseCheckUpdated(false);
        }

        public void SetSearchFeedbackType(SearchFeedbackType feedbackType)
        {
            switch (feedbackType)
            {
                default:
                case SearchFeedbackType.Normal:
                    uiFilteredOutText.Text = "Filtered out=";
                    break;
                case SearchFeedbackType.Advertisement:
                    uiFilteredOutText.Text = "Updated=";
                    break;
            }
        }

        private void OnClear(object sender, RoutedEventArgs e)
        {
            Search?.ClearDisplay();
        }
    }
}
