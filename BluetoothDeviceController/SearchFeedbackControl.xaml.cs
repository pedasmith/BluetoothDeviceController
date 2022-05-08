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

        /// <summary>
        /// Returns TRUE iff the current thread is the UI thread
        /// </summary>
        /// <returns></returns>
        public static bool IsOnUIThread()
        {
            var dispather = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            var retval = dispather.HasThreadAccess;
            return retval;
        }

        /// <summary>
        /// Calls the given function either directly or on the UI thread the TryRunAsync(). The resulting task is just thrown away.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="priority"></param>
        public static void CallOnUIThread(Action f, Windows.UI.Core.CoreDispatcherPriority priority = Windows.UI.Core.CoreDispatcherPriority.Low)
        {
            if (IsOnUIThread())
            {
                f();
            }
            else
            {
                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                var task = dispatcher.TryRunAsync(priority, () => { f(); });
            }
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
                Search.StartSearchDefault();
            }
        }

        public void StartSearchFeedback()
        {
            //System.Diagnostics.Debug.WriteLine($"SearchFeedback: started");
            uiProgress.ShowPaused = false;
            nFound = 0;
            nFoundAll = 0;
            UpdateUI();
        }



        public void StopSearchFeedback()
        {
            CallOnUIThread( () => {
                System.Diagnostics.Debug.WriteLine($"SearchFeedback: stopped");
                uiProgress.ShowPaused = true;
            });

#if NEVER_EVER_DEFINED
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
#endif
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
            CallOnUIThread(() => {
                uiFound.Text = nFound.ToString();
                uiFoundAll.Text = (nFoundAll - nFound).ToString();
            });
        }
    }
}
