using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using UtilitiesWinUI3;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;

#if NET8_0_OR_GREATER
#nullable disable
#endif

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3
{
    public sealed partial class HeartRate_RRInterval_Control : UserControl
    {
        const int MAX_IMAGES = 205; // Always an odd number! DBG: super small for debugging!
        const double WIDTH_R_IN_PIXELS = 54; // 314; // Measured the image in Assets. Note: make this dynamic?
        const double W1000MS_IN_PIXELS = 120.0;
        public HeartRate_RRInterval_Control()
        {
            InitializeComponent();
        }

        WriteableBitmap WriteableR;
        WriteableBitmap WriteableCenter;

        public void AddRRIntervalText(string value)
        {
            uiRRText.Text = value;
        }

        public void UpdateGraphColor(string _, uint color)
        {
            Color xamlColor = UtilitiesWinUI3.UtilitiesWinUI3.ConvertIgnoreA(color);
            WriteableCenter.Recolor(xamlColor);
            WriteableR.Recolor(xamlColor);
        }

        private void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
            Console.WriteLine(str);
        }

        public void AddRRInterval(double rrInMilliseconds)
        {
            if (uiAllImages.Children.Count == 0)
            {
                Log($"Internal error: RRInterval: images are not initialized");
                return; // Not initialized??
            }

            double imageRInMilliseconds = WIDTH_R_IN_PIXELS / W1000MS_IN_PIXELS * 1000.0;
            var scale = ((rrInMilliseconds - imageRInMilliseconds) / 1000.0) * W1000MS_IN_PIXELS;
            if (scale < 1) scale = 1.0;

            // Don't let us accumulate too many images
            lock (this)
            {
                var n = uiAllImages.Children.Count;
                if (n >= MAX_IMAGES)
                {
                    // images are always R Center R Center R Center R .. .. Center R
                    // move the center (index 1) to the end, then the R (old index 2)
                    // note that the old index 2 becomes index 1 after we more the center from index 1

                    uiAllImages.Children.Move(1, (uint)(n - 1)); // move the center to the end
                    uiAllImages.Children.Move(1, (uint)(n - 1)); // move the next R to the end
                    (uiAllImages.Children[n - 2] as Image).Width = scale;
                }
                else
                {
                    var newCenter = new Image()
                    {
                        Source = WriteableCenter,
                        Width = scale,
                        Stretch = Stretch.Fill,
                        Tag = "Center",
                    };
                    var newR = new Image()
                    {
                        Source = WriteableR,
                        Tag = "R"
                    };

                    uiAllImages.Children.Add(newCenter);
                    uiAllImages.Children.Add(newR);
                    uiScroll.ChangeView(uiScroll.ScrollableWidth, null, null);
                }
            }

        }

        private async Task<WriteableBitmap> ReadBitmapAsync(Uri uri)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(uri);

            // Decode into a WriteableBitmap
            using var stream = await file.OpenReadAsync();
            var bmp = new WriteableBitmap(1, 1); // temporary size; replaced by SetSourceAsync
            await bmp.SetSourceAsync(stream);
            return bmp;
        }


        /// <summary>
        /// Add in the leftmost 'R' item
        /// </summary>
        public async Task InitializeAsync()
        {
            if (uiAllImages.Children.Count > 0)
            {
                return; // Already initialized
            }

            bool inFirstInitialize = true;
            lock(InitializeLock)
            {
                if (InitializeStarted)
                {
                    inFirstInitialize = false;
                }
                InitializeStarted = true;
            }
            if (inFirstInitialize)
            {
                // Load the asset (PNG, JPG, etc.) from the app package
                var uriR = new Uri("ms-appx:///Assets/GraphicalHelps/RR_Interval_HeartRate_R.png");
                var uriCenter = new Uri("ms-appx:///Assets/GraphicalHelps/RR_Interval_HeartRate_Center.png");
                WriteableR = await ReadBitmapAsync(uriR);
                WriteableCenter = await ReadBitmapAsync(uriCenter);
                var newR = new Image()
                {
                    Source = WriteableR,
                };
                uiAllImages.Children.Add(newR);
            }
            else
            {
                while (uiAllImages.Children.Count == 0)
                {
                    await Task.Delay(2); // should be super fast!
                }
            }
        }

        bool InitializeStarted = false;
        Lock InitializeLock = new Lock();
    }
}
