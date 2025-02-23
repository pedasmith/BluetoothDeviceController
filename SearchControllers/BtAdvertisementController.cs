﻿using BluetoothDeviceController;
using BluetoothDeviceController.Beacons;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.LockScreen;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation.Metadata;
using Windows.UI.Core;

namespace SearchControllers
{
    public interface IDeviceDisplay
    {
        void AddOrUpdateDeviceBle(DeviceInformationWrapper wrapper);
    }
    /// <summary>
    /// Handles all advertisement searchs -- start, stop, reporting
    /// BTAdvertisementWatcher replaces MenuBleWatcher in main.xaml.cs
    /// </summary>
    public class BTAdvertisementController
    {
        public IDeviceDisplay DeviceDisplay = null;
        public IDoSearchFeedback SearchFeedback = null;
        public CoreDispatcher Dispatcher = null;

        BluetoothLEAdvertisementWatcher Watcher = null;
        /// <summary>
        /// Handy deubgging point; set this and we can watch or break when specific devices are seen
        /// </summary>
        ulong Debug_Addr = 0;
        /// <summary>
        /// List of advertisements we've already seen. 
        /// </summary>
        Dictionary<ulong, BleAdvertisementWrapper> BleWrappers = new Dictionary<ulong, BleAdvertisementWrapper>();
        Dictionary<ulong, BluetoothLEAdvertisementReceivedEventArgs> BleOriginalAdverts = new Dictionary<ulong, BluetoothLEAdvertisementReceivedEventArgs>();

        public bool IsActive {  get { return Watcher != null; } }
        /// <summary>
        /// Scanning means getting more information from each advertisement. It's not supported on all version of Windows.
        /// </summary>
        public bool CanScan
        {
            get
            {
                var retval = ApiInformation.IsPropertyPresent("Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher", "ScanningMode");
                return retval;
            }
        }
        public bool CanExtendedAdvertisement
        {
            get
            {
                var retval = ApiInformation.IsPropertyPresent("Windows.Devices.Bluetooth.Advertisement.BluetoothLEAdvertisementWatcher", "AllowExtendedAdvertisements");
                return retval;
            }
        }

        /// <summary>
        /// Call to start scanning for advertisements.
        /// </summary>
        public void Start()
        {
            Watcher = new BluetoothLEAdvertisementWatcher();
            if (CanExtendedAdvertisement) Watcher.AllowExtendedAdvertisements = true;
            if (CanScan) Watcher.ScanningMode = BluetoothLEScanningMode.Active;

            Watcher.Received += Watcher_Received;
            Watcher.Stopped += Watcher_Stopped;
            Watcher.Start();
        }


        public void Stop()
        {
            Watcher?.Stop();
            Watcher = null;
        }

        ConcurrentQueue<DeviceInformationWrapper> DIItemsToAdd = new ConcurrentQueue<DeviceInformationWrapper>();
        bool InsideUxUpdateLoop = false;
        private void Watcher_Received(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementReceivedEventArgs args)
        {
            var name = args.Advertisement.LocalName;
            if (string.IsNullOrEmpty (name))
            {
                name = BluetoothAddress.AsString(args.BluetoothAddress);
            }
            System.Diagnostics.Debug.WriteLine($"DeviceBleWatcher: Device {name} seen");
            if (name.Contains("Wescale") || name.StartsWith("LC"))
            {
                ; // Handy hook for debugger.
            }
            if (Debug_Addr != 0 && Debug_Addr == args.BluetoothAddress)
            {
                ; // Breakpoint
            }
            else if (name.StartsWith("Govee") && Debug_Addr == 0)
            {
                Debug_Addr = args.BluetoothAddress;
            }

            // Get the appropriate ble advert wrapper (or make a new one)
            BleAdvertisementWrapper advertisementWrapper = null;
            BleWrappers.TryGetValue(args.BluetoothAddress, out advertisementWrapper);
            if (advertisementWrapper == null)
            {
                advertisementWrapper = new BleAdvertisementWrapper();
                advertisementWrapper.AdvertisementType = BleAdvertisementWrapper.BleAdvertisementType.BluetoothLE;
                BleWrappers.Add(args.BluetoothAddress, advertisementWrapper);
            }
            advertisementWrapper.BleAdvert = args;
            bool canUpdate = true;
            if (args.IsScanResponse)
            {
                BluetoothLEAdvertisementReceivedEventArgs original = null;
                BleOriginalAdverts.TryGetValue(args.BluetoothAddress, out original);
                advertisementWrapper.BleOriginalAdvert = original;
                if (original == null)
                {
                    canUpdate = false; 
                }
            }
            else if (args.IsScannable)
            {
                BleOriginalAdverts[args.BluetoothAddress] = args;
            }

            var diwrapper = new DeviceInformationWrapper(advertisementWrapper);
            DIItemsToAdd.Enqueue(diwrapper);

            if (Dispatcher != null && DeviceDisplay != null && canUpdate)
            {
                // TODO: this is completely incorrect multi-threaded code. What needs to happen is that
                // I should do this UI work when the while is > 0. That means I should track if the 
                // UX async task is still running in some way.
                // Luckily, most of the time there will be another advertisement along soon.
                lock (this)
                {
                    if (!InsideUxUpdateLoop)
                    {
                        // Just a free-running task
                        var task = Dispatcher.TryRunAsync(Windows.UI.Core.CoreDispatcherPriority.Low,
                            () =>
                            {
                                lock (this)
                                {
                                    InsideUxUpdateLoop = true;
                                }
                                while (InsideUxUpdateLoop)
                                {
                                    DeviceInformationWrapper di = null;
                                    var status = DIItemsToAdd.TryDequeue(out di);
                                    if (status)
                                    {
                                        DeviceDisplay.AddOrUpdateDeviceBle(di);
                                    }
                                    lock (this)
                                    {
                                        if (DIItemsToAdd.IsEmpty)
                                        {
                                            InsideUxUpdateLoop = false;
                                        }
                                    }
                                }
                            }
                        );
                    }
                } // end of lock
                // Will be the MainPage.Xaml.cs method call at about line 782
            }
        }

        private void Watcher_Stopped(BluetoothLEAdvertisementWatcher sender, BluetoothLEAdvertisementWatcherStoppedEventArgs args)
        {
            // Original comment: Might well be a duplicate; the timer (Timeout_BluetoothLEAdvertisementWatcher_Tick) will already have called CancelSearch()
            SearchFeedback?.StopSearchFeedback();
        }
    }
}
