using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
    interface HasId
    {
        /// <summary>
        /// For example, BBC micro:bit
        /// </summary>
        /// <returns></returns>
        string GetDeviceNameUser();
        string GetId();
        string GetPicturePath();
    }

    public sealed partial class PageDockAdapterControl : UserControl
    {
        private PageDockAdapterControl()
        {
            this.InitializeComponent();
        }

        const double OpacityWhenSelected = 1.0;
        const double OpacityWhenNotSelected = 0.4;

        Brush ColorWhenSelected = new SolidColorBrush(Colors.AntiqueWhite);
        Brush ColorWhenNotSelected = new SolidColorBrush(Colors.LightGray);

        bool IsAdded = false;

        public Page GetPage()
        {
            if (uiPageContainer.Children.Count == 0) return null;
            return uiPageContainer.Children[0] as Page;
        }

        /// <summary>
        /// Remove in this context means, "remove the living page from the dock because we're about to display it for realies"
        /// </summary>
        /// <param name="page"></param>
        public void RemovePage(Page page)
        {
            if (uiPageContainer.Children.Count == 0) return;
            uiPageContainer.Children.RemoveAt(0);

            uiUndockedText.Text = "";
            uiUndockedText.Visibility = Visibility.Visible;

            uiDevicePicture.Opacity = OpacityWhenSelected; // make it brighter so which know which one is which.
            uiBorder.Background = ColorWhenSelected;

            // Reenable the page for use!
            page.IsTapEnabled = true;
            page.IsEnabled = true;
        }
        public void SetPage(Page page)
        {
            if (uiPageContainer.Children.Count == 0)
            {
                uiPageContainer.Children.Add (page);
            }
            else
            {
                uiPageContainer.Children[0] = page;
            }
            if (!IsAdded)
            {
                var hasId = page as HasId;
                if (hasId != null)
                {
                    var source = hasId.GetPicturePath();
                    uiDevicePicture.Source = source;
                    uiDeviceId.Text = hasId.GetDeviceNameUser();
                }
            }
            IsAdded = true;
            uiUndockedText.Visibility = Visibility.Collapsed;
            uiDevicePicture.Opacity = OpacityWhenNotSelected; // make it dimmer so which know which one is which.
            uiBorder.Background = ColorWhenNotSelected;

            page.IsTapEnabled = false;
            page.IsEnabled = false;
        }

        IDeletePage DeletePage = null;
        private void OnDelete(object sender, RoutedEventArgs e)
        {
            DeletePage?.DeletePage(this);
        }

        public static PageDockAdapterControl MakeAdapter(Page page, IDeletePage dp)
        {
            if (!(page is HasId)) return null;

            // Gotta dock it for the first time
            var vb = new PageDockAdapterControl();
            vb.SetPage(page);
            vb.IsTapEnabled = true;
            vb.Tag = (page as HasId)?.GetId();
            vb.DeletePage = dp;

            // Set up the page to be untappable
            page.IsTapEnabled = false;
            page.IsEnabled = false;
            return vb;
        }


    }
}
