using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;


namespace BluetoothWinUI3
{
    /// <summary>
    /// Implemented by MainWindow to handle device control changes that happen
    /// after a DeviceControl device get a DataContextChanged notification.
    /// 
    /// This happens in e.g., BTStandard_Demo when the device doesn't have a 
    /// battery and the main UX needs to update (the MainWindow Device> menu has
    /// to be updated to not allow showing the graph versus table)
    /// </summary>
    public interface IHandleNotifyDeviceControlChanges
    {
        /// <summary>
        /// Says the the UXCapabilities have changed. Can also means that the LineNames have changed,
        /// which is a little over-broad.
        /// </summary>
        void OnGetUXCapabilitiesChanged(UserControl deviceControl, IDeviceControlBasic.UXCapabilities newCapabilities);
    }

    /// <summary>
    /// Many (but not all) of the UserControls that get displayed will also implement
    /// IDeviceControlBasic for the most common methods that need to be handled.
    /// </summary>
    public interface IDeviceControlBasic
    {
        /// <summary>
        /// updates the device control user interface base on the user preferences in SaveData.
        /// </summary>
        /// <param name="saveData"></param>
        void UpdateUX(SaveData saveData);

        void UpdateUX(UserPreferences newPrefs, UserPreferences oldPrefs);

        void UpdateUX(MainWindow.WindowSize windowSize, Windows.Foundation.Size largeActualSize);

        enum Visibility { Visible, Collapsed, }
        Visibility GetDataGridVisibility();
        void SetDataGridVisibility(Visibility visibility);

        [Flags] enum UXCapabilities 
        { 
            None = 0x00, 
            CommonOptionsV01 = CanGetGraphAsPng + CanGetData + CanRename + CanShowTable,
            CanGetGraphAsPng = 0x01, 
            CanGetData = 0x02,
            CanRename = 0x04,
            CanShowTable = 0x08,
            CanGetDetails = 0x10,
        }
        UXCapabilities GetUXCapabilities();

        // TODO: make the Png exporter like the IExportData? So that all that awful code is in one place?
        void ExportGraphAsPng();
        // Gone! ExportData is now in ExportDeviceData() and uses the GetData() for export! string ExportData(IExportData exporter);
        IReadOnlyList<IBTCommonMetaData> GetDataAll();
        /// <summary>
        /// Most controls do not have any fine grained data
        /// Clears the accumulated data. Is called by the SmartExportRunner about every 5 seconds.
        /// This is useful when data comes in quicker than the normal smart export frequency 
        /// and we want all that data. For example, the heart rate control keeps the most recent
        /// pulse data (which is fine) but also there's a much finer grained RRInterval data. We want
        /// to save all the RRInterval data with no duplicates on the smart export.
        /// </summary>
        void ClearAccumulatedFineGrainedData();


        /// <summary>
        /// Returns NULL if there's no data yet
        /// </summary>
        /// <returns></returns>
        IBTCommonMetaData GetDataMostRecent();

        enum DetailsType {  Normal, All, }
        string GetDetails(DetailsType detailsType);

        void SetNotifyDeviceControlChanges(IHandleNotifyDeviceControlChanges mainWindow);
    }


    public interface IDeviceControlDevice : IDeviceControlBasic
    {
        /// <summary>
        /// Returns the KnownDevice associated with the device
        /// </summary>
        /// <returns></returns>
        KnownDevice DataContextAsKnownDevice { get;  }

        /// <summary>
        /// Get the list of graph name (e.g., "Temperature", "Pressure", "Humidity". This will be used by
        /// e.g., the MainWindow to update the device menu display
        /// </summary>
        List<string> LineNames { get; }
        uint GetGraphColor(string lineName); // OxyColor is a set of 4 bytes ARGB

        /// <summary>
        /// Update the color of a single line
        /// </summary>
        void UpdateGraphColor(string lineName, uint color);
    }

    /// <summary>
    /// The DeviceControl wants to get all of the advertisements that are seen. 
    /// Used by e.g., BTServicesCharacteristicsDisplay
    /// Is a speciality interface; most device controls will not implement this.
    /// </summary>
    public interface IHandleBTAdvertisements
    {
        void HandleAdvertisement(WatcherData data);
    }


    /// <summary>
    /// The DeviceControl wants all of the advertisements for the specific (known) device based on 
    /// BT address. Used by a bunch of sensor like the BTCommon_EnvironmentalControl because the 
    /// Govee, SensorPro, ThermPro etc. data is packed into the adverts.
    /// Many device control do not implement this.
    /// </summary>
    public interface IHandleMyBTAdvertisements
    {
        void HandleMyAdvertisement(WatcherData data);
    }

}
