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
            uiProgress.ShowPaused = false;
            nFound = 0;
            nFoundAll = 0;
        }

        public void StopSearchFeedback()
        {
            var task = this.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
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
            var task = this.Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => {
                uiFound.Text = nFound.ToString();
                uiFoundAll.Text = (nFoundAll-nFound).ToString();
            });

        }
    }
}
