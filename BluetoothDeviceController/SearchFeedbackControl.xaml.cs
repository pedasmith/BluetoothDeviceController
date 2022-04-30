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

        private void OnSearch(object sender, RoutedEventArgs e)
        {
            if (Search == null) return;
            if (Search.GetSearchActive())
            {
                Search.CancelSearch();
            }
            else
            {
                Search.StartSearchDefault();
            }
        }

        public void StartSearchFeedback()
        {
            //System.Diagnostics.Debug.WriteLine($"SearchFeedback: started");
            uiProgress.ShowPaused = false;
            nFound = 0;
            nFoundAll = 0;
        }

        public static bool IsOnUIThread()
        {
            var dispather = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            var retval = dispather.HasThreadAccess;
            return retval;
        }

        public void StopSearchFeedback()
        {
            if (SearchFeedbackControl.IsOnUIThread())
            {
                //System.Diagnostics.Debug.WriteLine($"SearchFeedback: stopped (quickly)");
                uiProgress.ShowPaused = true;
            }
            else
            {
                //System.Diagnostics.Debug.WriteLine($"SearchFeedback: stopping (wait for UI)");
                var task = this.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                    System.Diagnostics.Debug.WriteLine($"SearchFeedback: stopped (for real)");
                    uiProgress.ShowPaused = true;
                });

            }
        }

        public void FoundDevice(FoundDeviceInfo deviceInfo)
        {
            nFoundAll++;
            if (deviceInfo == FoundDeviceInfo.IsNew)
            {
                nFound++;
            }
            var task = this.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                uiFound.Text = nFound.ToString();
                uiFoundAll.Text = (nFoundAll-nFound).ToString();
            });

        }
    }
}
