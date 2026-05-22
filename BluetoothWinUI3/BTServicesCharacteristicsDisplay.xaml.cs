using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3
{
    public sealed partial class BTServicesCharacteristicsDisplay : UserControl, IHandleBTAdvertisements, IDeviceControlBasic
    {
        /// <summary>
        /// Standard: Panel size. Set in UpdateUX from MainWindow.
        /// </summary>
        MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400

        /// <summary>
        /// Used for logging only
        /// </summary>
        private readonly string InternalDeviceType = "BT_Services";

        public BTServicesCharacteristicsDisplay()
        {
            InitializeComponent();
        }

        public ObservableCollection<WatcherData> WatcherDataList { get; internal set; } = new ObservableCollection<WatcherData>();
        private int FindWatcherDataIndex(WatcherData data)
        {
            for (int i = 0; i < WatcherDataList.Count; i++)
            {
                if (WatcherDataList[i].Addr == data.Addr)
                {
                    return i;
                }
            }
            return -1;
        }
        public void HandleAdvertisement(WatcherData data)
        {
            var index = FindWatcherDataIndex(data);
            if (index == -1)
            {
                WatcherDataList.Add(data);
            }
            else
            {
                WatcherDataList[index] = data;
            }
            uiLog.Text = data.ToString();
        }

        private void uiAdvertisementListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ; // TODO: show details of the clicked item.
        }

        public void UpdateUX(SaveData saveData)
        {
        }

        public void UpdateUX(UserPreferences newPrefs, UserPreferences oldPrefs)
        {
        }

        public void UpdateUX(MainWindow.WindowSize windowSize, Windows.Foundation.Size largeActualSize)
        {
            CurrWindowSize = windowSize;
            switch (CurrWindowSize)
            {
                default:
                case MainWindow.WindowSize.Normal:
                    rootPanel.Width = 380;
                    rootPanel.Height = 380;
                    break;
                case MainWindow.WindowSize.Large:
                    rootPanel.Width = largeActualSize.Width;
                    rootPanel.Height = largeActualSize.Height;
                    break;
            }
        }

        public IDeviceControlBasic.Visibility GetDataGridVisibility()
        {
            return IDeviceControlBasic.Visibility.Collapsed;
        }

        public void SetDataGridVisibility(IDeviceControlBasic.Visibility visibility)
        {
        }

        public IDeviceControlBasic.UXCapabilities GetUXCapabilities()
        {
            var retval = IDeviceControlBasic.UXCapabilities.None; //  IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng | IDeviceControlBasic.UXCapabilities.CanGetData;
            return retval;
        }

        public void ExportGraphAsPng()
        {
        }

        public string ExportData(IExportData exporter)
        {
            return string.Empty;
        }
    }
}
