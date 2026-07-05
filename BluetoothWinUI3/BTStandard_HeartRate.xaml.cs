using BluetoothProtocols;
using BluetoothProtocolsDevicesCore;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using BluetoothWinUI3.BTDeviceUnitConverters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.
using System.Text;
using Utilities;
using Windows.Devices.Bluetooth;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif

// TODO: set the name
// TODO: what should I do with the RR data?

#region Set these to match your device
using DeviceSpecificType = BTStandard_HeartRate; // Change: pick your device, not BTStandard_Demo
using DeviceSpecificSensorData = BTStandard_HeartRate.Heart_Rate_Data; // Change: 
using DeviceSpecificBatteryData = BTStandard_HeartRate.Battery_Data; // Change: many device support battery
using DeviceSpecificSensorDataFacade = Heart_Rate_Data_Facade; // Change: 
#endregion

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTStandard_HeartRateControl : UserControl, IDeviceControlBasic, IDeviceControlDevice
{
    #region Settings that must be updated for a new device
    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "BTStandard_HeartRate"; // Change: BTStandard_Demo
    #endregion

    #region Advanced settings and values
    /// <summary>
    /// Most developer never need to change this from 'true'!
    /// Ususually a device always has their sensor data. But some devices are might not. 
    /// For the BTStandard_DemoControl, the "sensor" is just the battery level. That was
    /// picked because so many devices include a battery level. But in case it doesn't,
    /// there's a way to tell the MainWindow that the device doesn't have a sensor.
    /// </summary>
    bool HasSensorData = true;

    /// <summary>
    /// Normally we can just read cached data and that's good enough. Some advanced cases
    /// might require reading non-cached data.
    /// </summary>
    BluetoothCacheMode DefaultCacheMode = BluetoothCacheMode.Cached;

    /// <summary>
    /// Every use case might have a different point of view about how frequently to update
    /// the historical data (the data displayed in the graph + shown in the table view
    /// + exported). A good default is 5 seconds.
    /// </summary>
    const double HistoricalDataUpdateRateInSeconds = 5.0;
    #endregion

    public BTStandard_HeartRateControl()
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += Control_DataContextChanged;

        // TODO: make this a utility and move the call to the loaded?
        // Set up the OxyModel Series. Reminder that each series is, e.g., "Temperature" or "Pressure"
        // This can't be done at initialization time because of C#: it won't let me use a regular
        // field when doing an initialization.
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                lineSeries.ItemsSource = HistoricalDataUnits.Data; //DOC:
            }
        }
        uiOxyPlot.Model = OxyPlotModel;

        //
        // Set up the uiTableView
        // https://w-ahmad.dev/WinUI.TableView/index.html
        // https://github.com/w-ahmad/WinUI.TableView
        //
        uiTableView.AutoGeneratingColumn += CurrTableCustomization.TableView_AutoGeneratingColumn_UseCustomization;
    }

    #region Instance value for a device
    DeviceSpecificType Device;
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;
    ulong OriginalBTAddr = 0xFFFFFFFF_FFFFFFFF;

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the collection.
    /// </summary>
    public DataCollection<DeviceSpecificSensorDataFacade> HistoricalDataUnits { get; } = new();
    public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return HistoricalDataUnits.Data; }
    public void ClearAccumulatedFineGrainedData()
    {
        // Only the RRInterval data is fine grained.
        CurrSensor_DataUnits?.CurrRRRecent?.DoClearAccumulatedFineGrainedData();
    }

    public IBTCommonMetaData GetDataMostRecent() // TODO: add this to the data collections!
    {
        return HistoricalDataUnits.Count == 0 ? null : HistoricalDataUnits.Data[HistoricalDataUnits.Count - 1];
    }

    // This control show two kinds of data. 
    // 1. Battery data is the "sensor data" which is the data to be graphed
    // and displayed in a table. 
    //
    // 2. Configuration data which is just displayed to the user
    //

    /// <summary>
    /// Current sensor data from the Device. 
    /// </summary>
    DeviceSpecificSensorData CurrSensor_Data = null;
    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    DeviceSpecificSensorDataFacade CurrSensor_DataUnits = null;

    /// <summary>
    /// Making a battery value that's seperate from the Sensor. This lets the programmer
    /// copy-paste data, pick a new sensor, and the battery stuff will still work.
    /// </summary>
    DeviceSpecificType.Battery_Data CurrBattery_Data = null;
    /// <summary>
    /// Just like CurrBattery_Data but in user-preferred units. For battery, it
    /// doesn't actually change anything :-)
    /// </summary>
    DeviceSpecificType.Battery_Data CurrBattery_DataUnits = null;

    #endregion

    #region Instance values for the UX
    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>
    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400


    /// <summary>
    /// List of the controls that have the little 'data has been updated' sparkles.
    /// Set in the Control_Loaded.
    /// </summary>
    List<(string, Microsoft.UI.Xaml.Documents.Run)> ControlsWithSparkles = null;

    /// <summary>
    /// Customization for the TableView.
    /// </summary>
    TableViewColumnCustomization CurrTableCustomization = new TableViewColumnCustomization()
    {
        TableColumnsToExclude = ["CurrFlagsDecoded", "Flags", "RRInterval", "SensorLocation"],
    };
    #endregion

    private void Control_Loaded(object sender, RoutedEventArgs e)
    {
        // Loaded gets called both when it's first loaded and also each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        // We only want to do work the first time.

        if (uiTableView.ItemsSource != null) return;
        uiTableView.ItemsSource = HistoricalDataUnits.Data;

        // Change: set the right sparkles
        ControlsWithSparkles = new List<(string, Microsoft.UI.Xaml.Documents.Run)>()
        {
            ( DeviceSpecificType.Heart_Rate_MeasurementPropertyChangedName, uiBpmChange),
        };
    }

    // Allows the control to provide feedback to Windows about updates to the device capabilties.
    // For example, the device might not have a sensor, and so the user shouldn't be able 
    // see the table or graph.
    IHandleNotifyDeviceControlChanges NotifyDeviceControlChangesWindows = null;
    public void SetNotifyDeviceControlChanges(IHandleNotifyDeviceControlChanges mainWindow)
    {
        NotifyDeviceControlChangesWindows = mainWindow;
    }

    // If you have to update these dynamically, be sure to call 
    // NotifyDeviceControlChangesWindows.OnGetUXCapabilitiesChanged
    // so the main window menus get updated.

    // TODO: should these be discoverable? Maybe from the Model which already has the user friendly names?
    List<string> _LineNames = new List<string>() { "HeartRate" }; // CHANGE:
    public List<string> LineNames { get { return _LineNames; } }

    /// <summary>
    /// The DataContext is a WinUI3 (and the rest of XAML) thing, and is just an object. And it can be
    /// set by anyone, at any time, to any value. The Bluetooth controls generally require that the 
    /// DataContext be a KnownDevice (which is turn is a bunch of data: the SupportedDevice, the
    /// WatcherData / Bluetooth advertisement that triggered this control being created, etc.)
    /// 
    /// DataContextAsKnownDevice is either a real KnownDevice or it's null.
    /// </summary>
    public KnownDevice DataContextAsKnownDevice { get { return DataContext as KnownDevice; } }

    /// <summary>
    /// The OxyPlotModel is the graph for the sensor data that we want to plot. It's of
    /// type "H.Oxyplot" which is a WinUI3 port of the original OxyPlot code.
    /// CHANGE: you will want to set the Title and the list of Axes and LineSeries. In
    /// general, each sensor type (e.g, on the Nordic Thingy there's a sensor for temperature,
    /// dumidity, pressure, etc.) has its own Axis and its own LineSeries.
    /// </summary>
    // H.OxyPlot
    private PlotModel OxyPlotModel { get; set; }
        = OxyPlotUtilities.MakeOxyPlotModelSimple("Heart Rate", 10, 50, "Heart Rate", "HeartRate");
        // CHANGE: set up the graph




    /// <summary>
    /// Loop through the LineSeries for where a matching DataFieldY
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public uint GetGraphColor(string name)
    {
        return UtilitiesWinUI3.UtilitiesWinUI3.GetGraphColor(name, OxyPlotModel);
    }


    /// <summary>
    /// This is a two-way street. Setting the DataContest to the KnownDevice will update some UX and will
    /// trigger looking up the SaveData and change more things. And it will actually connect to the device.
    /// AND this will update the KnownDevice with, e.g., the DeviceId and the BluetoothLEDevice which will be
    /// used by other bits of the system.
    /// </summary>
    private async void Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // FYI: by the time this method is called, the DataContext in the object is already set

        if (args.NewValue == null) return; // just bogus; ignore.

        // Must have been set as a KnownDevice; otherwise we're in a very weird state.
        // DataContxtAsKnownDevice is just the DataContext cast (with an "as") to KnownDevice.
        if (DataContextAsKnownDevice == null)
        {
            Log($"Impossible Error: {InternalDeviceType}: Data context change, but it's not a KnownDevice. Type is {args.NewValue.GetType()}");
            return;
        }
        if (OriginalBTAddr != 0xFFFFFFFF_FFFFFFFF)
        {
            ; // duplicate call!
            return;
        }


        OriginalBTAddr = DataContextAsKnownDevice.Advertisement.Addr;
        uiAddress.Text = BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr);
        CurrSaveData = AllSaveData.FindWithAdvertisementAddress(DataContextAsKnownDevice.Advertisement.Addr); // might return null for the first connection

        Device = new DeviceSpecificType()
        {
            ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
        };
        if (Device.ble == null)
        {
            Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
            CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
            return;
        }
        // It's critical to set these!
        DataContextAsKnownDevice.Id = Device.ble.DeviceId ?? ""; // never null :-)
        DataContextAsKnownDevice.BTLEDevice = Device.ble;
        CurrSaveData = AllSaveData.FindWithId(DataContextAsKnownDevice.Id); // Use the stable form of the device id.
        // CurrSaveData won't exist if the user hasn't made any changes

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        UtilitiesWinUI3.UtilitiesWinUI3.InitializeKeyLineColorsFromDefaultOxyPlot(OxyPlotModel, rootPanel);
        UpdateUX(CurrSaveData); // Can be null when the user hasn't made any changes
        if (CurrSaveData == null)
        {
            KnownDeviceName = DataContextAsKnownDevice.Advertisement?.BestName ?? KnownDeviceName;
            uiKnownDeviceName.Text = KnownDeviceName;
        }

        Device.PropertyChanged += Device_PropertyChanged;

        await Device.NotifyBatteryLevelAsync(); // CHANGE: and the next lines
        await Device.ReadBatteryLevel(DefaultCacheMode);
        await Device.ReadBody_Sensor_Location(DefaultCacheMode);

        // Tons of GAP stuff to test out more reads
        await Device.ReadManufacturer_Name_String(DefaultCacheMode);
        await Device.ReadModel_Number_String(DefaultCacheMode);
        await Device.ReadHardware_Revision_String(DefaultCacheMode);
        await Device.ReadFirmware_Revision_String(DefaultCacheMode);
        await Device.ReadSoftware_Revision_String(DefaultCacheMode);
        await Device.ReadSystem_ID(DefaultCacheMode);
        await Device.ReadDevice_Name(DefaultCacheMode);

        await Device.NotifyHeart_Rate_MeasurementAsync();

        // Can't do this earlier; merely calling FromBluetoothAddressAsync doesn't actually 
        // connect. Once we do the notify and reads the device will be connected or not.
        CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, Device.ble.ConnectionStatus);
    }


    /// <summary>
    /// Called from MainWindow to find out whether the display is the graph or the table.
    /// </summary>
    public IDeviceControlBasic.Visibility GetDataGridVisibility()
    {
        var retval = (uiDataGridPanel.Visibility == Visibility.Visible)
            ? IDeviceControlBasic.Visibility.Visible : IDeviceControlBasic.Visibility.Collapsed;
        return retval;
    }


    /// <summary>
    /// When visibility is Visible, display the table of data. When collapsed, display
    /// the grid. Is called from MainWindow based on user selection.
    /// </summary>
    /// <param name="visibility"></param>
    public void SetDataGridVisibility(IDeviceControlBasic.Visibility visibility)
    {
        UtilitiesWinUI3.UtilitiesWinUI3.SetDataGridVisibility(uiOxyPlot, uiDataGridPanel, visibility);
    }


    /// <summary>
    /// Updates the OxyPlot line with a given name (e.g., "Temperature"). Is called from MainWindow when the
    /// user picks a new color.
    /// </summary>
    /// <param name="lineName"></param>
    /// <param name="color"></param>

    public void UpdateGraphColor(string lineName, uint color)
    {
        UtilitiesWinUI3.UtilitiesWinUI3.UpdateGraphColor(OxyPlotModel, rootPanel, lineName, color);
    }




    /// <summary>
    /// SaveData is per-device and includes the display name (e.g., a "Thingy" might have a preferred name of "Living Room")
    /// and also a bunch of color information.
    /// </summary>
    public void UpdateUX(SaveData saveData)
    {
        if (saveData == null) return;

        var name = saveData.GetUserName();
        if (name != KnownDeviceName)
        {
            KnownDeviceName = name;
            uiKnownDeviceName.Text = KnownDeviceName;
            CurrSensor_DataUnits?.Name = KnownDeviceName;
            foreach (var item in HistoricalDataUnits.Data)
            {
                item.Name = KnownDeviceName;
            }
        }

        var colors = saveData.GetDeviceColors(Application.Current.RequestedTheme);
        var brushes = new DeviceColorBrushes(colors);
        DeviceColorBrushes.SetUxColors(this.rootPanel, brushes);

        // Set the graph text colors
        var oxyColorText = UtilitiesOxyColor.WinUI3ColorToOxyColor(colors.TextColor);
        OxyPlotModel.TextColor = oxyColorText;

        // Also set the graph line colors.
        foreach (var (lineName, color) in colors.GraphColors)
        {
            UpdateGraphColor(lineName, color);
        }
    }

    /// <summary>
    /// UserPreferences are for the app as a whole, not for this particular device. For example: the preferred temperature unit.
    /// </summary>
    public void UpdateUX(UserPreferences newPrefs, UserPreferences oldPrefs)
    {
        CurrUserPrefs = newPrefs;

        // Update the saved data in the HistoricalDataUnits to match the new user preferences.
        foreach (var data in HistoricalDataUnits.Data)
        {
            // For the BTStandard_Demo, there are no units to change
            if (oldPrefs != null && newPrefs.Temperature != oldPrefs.Temperature)
            {
                // Change: based on your knowledge of the sensor data, change the temperature readings.
                // data.Temperature = BluetoothWatcher.Units.Temperature.Convert(data.Temperature, oldPrefs.Temperature, CurrUserPrefs.Temperature);
            }
            if (oldPrefs != null && newPrefs.Pressure != oldPrefs.Pressure)
            {
                // Change: based on your knowledge of the sensor data, change the pressure readings.
                // data.Pressure = BluetoothWatcher.Units.Pressure.Convert(data.Pressure, oldPrefs.Pressure, CurrUserPrefs.Pressure);
            }
        }

        UpdateDeviceDataUX(""); // all of them.
    }

    /// <summary>
    /// Standard: the normal way to resize the control. 
    /// </summary>
    /// <param name="windowSize"></param>
    /// <param name="largeActualSize"></param>

    public void UpdateUX(MainWindow.WindowSize windowSize, Windows.Foundation.Size largeActualSize)
    {
        CurrWindowSize = windowSize;
        UtilitiesWinUI3.UtilitiesWinUI3.UpdateUXWindowSize(windowSize, largeActualSize, rootPanel, OxyPlotModel, uiOxyPlot);
    }



    /// <summary>
    /// User preferences as set by the UpdateUX call
    /// </summary>
    UserPreferences CurrUserPrefs { get; set; } = null;


    private void Log(string str)
    {
        System.Diagnostics.Debug.WriteLine(str);
        Console.WriteLine(str);
    }

    private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        UIThreadHelper.CallOnUIThread(() =>
        {
            if (!IsLoaded) return;
            CurrSaveData?.History.UpdateDataHistory(DateTimeOffset.Now);
            UpdateDeviceDataUX(e.PropertyName);
        });
    }


    Dictionary<string, int> NPropertyChanges { get; } = [];
    readonly List<string> Sparkles = ["╺", "╼", "╾", "╸", "╾", "╼"];

    private void UpdateSparkles(string name)
    {
        // In practice, name is never "*". The code is set up this way to match the Govee code.
        if (name == "") return;
        NPropertyChanges[name] = NPropertyChanges.GetValueOrDefault(name, 0) + 1;
        foreach ((string potentialMatchName, Microsoft.UI.Xaml.Documents.Run run) in ControlsWithSparkles)
        {
            if (potentialMatchName == name || name == "*")
            {
                run.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
            }
        }
    }


    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated.
    /// </summary>
    /// <param name="name"></param>
    private void UpdateDeviceDataUX(string name)
    {
        if (Device == null) return;
        CurrSensor_Data = Device.CurrHeart_Rate_Data;
        CurrBattery_Data = Device.CurrBattery_Data;

        // name is from e.PropertyName when the Device does a PropertyChanged.

        UpdateSparkles(name);

        // Update data from the device to match the current preferred units. Will create the values as needed.
        CurrSensor_DataUnits = DeviceSpecificSensorDataFacade.CopyToOrClone(CurrSensor_Data, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
        CurrBattery_DataUnits = DeviceSpecificBatteryData.CopyToOrClone(CurrBattery_Data, CurrBattery_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

        // Update this historical data; this will automatically update the table and graph.
        //
        // There's two kinds of sensors: ones like the Nordic_Thingy that send each bit of data separately,
        // and ones that send all the data at once (like the Govee). 
        //
        // Track the historical data
        switch (name)
        {
            //            case BTStandard_HeartRate.Device_NamePropertyChangedName:
            //                uiName.Text = CurrCommon_Configuration_Data.Device_Name;
            //                break;

            case "*": // never used, but here so it matches the Govee code.
            case DeviceSpecificType.Heart_Rate_MeasurementPropertyChangedName:
                uiBpm.Text = CurrSensor_DataUnits.HeartRate.ToString();
                uiFlags.Text = CurrSensor_DataUnits.CurrFlagsDecoded.ToString();
                if (CurrSensor_DataUnits.RRInterval == null)
                {
                    if (uiRRInterval.Text == "")
                    {
                        uiRRInterval.Text = "[]";
                    }
                }
                else if (CurrSensor_DataUnits.RRInterval.Count == 0)
                {
                    ; // do nothing
                }
                else
                {
                    CurrSensor_DataUnits.CurrRRRecent.AddRRData(CurrSensor_DataUnits.TimestampMostRecent, CurrSensor_DataUnits.RRInterval);

                    var rr = new StringBuilder();
                    foreach (var value in CurrSensor_DataUnits.RRInterval)
                    {
                        if (rr.Length != 0)
                        {
                            rr.Append(", ");
                        }
                        rr.Append(value.ToString());
                    }
                    uiRRInterval.Text = "[" + rr.ToString() + "]";
                }
                if (CurrSensor_DataUnits.CurrFlagsDecoded.HasFlag(DeviceSpecificSensorDataFacade.FlagsDecoded.SensorContactSupported))
                {
                    var icon = CurrSensor_DataUnits.CurrFlagsDecoded.HasFlag(DeviceSpecificSensorDataFacade.FlagsDecoded.SensorContactDetected)
                        ? "✓" : "✗"; // 
                    uiSensorConnection.Text = icon;
                }
                else
                {
                    uiSensorConnection.Text = "--"; // device doesn't know if it's connected TODO: correct icon?
                }
                UpdateHistoricalDataAndGraph();
                break;

            case DeviceSpecificType.Body_Sensor_LocationPropertyChangedName:
                // The check box is set by the 
                uiSensorLocation.Text = CurrSensor_DataUnits.Sensor;
                break;


            case DeviceSpecificType.Device_NamePropertyChangedName:
                uiName.Text = Device.CurrGAP_Data.DeviceName;
                Log($"HeartRate: Manufacturer: {Device.CurrDevice_Information_Data.Manufacturer}");
                Log($"HeartRate: Model: {Device.CurrDevice_Information_Data.ModelNumber}");
                Log($"HeartRate: HardwareRevision: {Device.CurrDevice_Information_Data.HardwareRevision}");
                Log($"HeartRate: FirmwareRevision: {Device.CurrDevice_Information_Data.FirmwareRevision}");
                Log($"HeartRate: SoftwareRevision: {Device.CurrDevice_Information_Data.SoftwareRevision}");
                Log($"HeartRate: SystemID: {Device.CurrDevice_Information_Data.SystemID}");
                break;
        }

        //
        // Many devices include a battery level. If so, chances are it's called "BatteryLevel"
        // 
        //
        if (CurrBattery_DataUnits != null)
        {
            if (name == DeviceSpecificType.BatteryLevelPropertyChangedName || name == "")
            {
                uiBTConnectionControl.SetBatteryLevel(CurrBattery_DataUnits.BatteryLevel);
            }
        }
    }


    /// <summary>
    /// Helper code to update historical data. The sensor might send a lot of data; the history only
    /// saves a portion of the data. Technicaly, every time there's new data we either update
    /// the most recent entry OR we add a new entry.
    /// </summary>
    private void UpdateHistoricalDataAndGraph()
    {
        var deltaInSeconds = CurrSensor_DataUnits.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
        var verb = (deltaInSeconds > HistoricalDataUpdateRateInSeconds)
            ? DataCollection<DeviceSpecificSensorDataFacade>.Verb.Add : DataCollection<DeviceSpecificSensorDataFacade>.Verb.ReplaceMostRecent;
        HistoricalDataUnits.Update(CurrSensor_DataUnits, verb); // Will add or replace the data and will copy as needed.

        //
        // Update the OxyPlot because it doesn't track the INotifyCollectionChanged
        //
        if (verb == DataCollection<DeviceSpecificSensorDataFacade>.Verb.Add && HistoricalDataUnits.Count == 2)
        {
            // DOC: Can't have the axes start off invisible because then they can't be switched back on
            if (CurrWindowSize == MainWindow.WindowSize.Normal)
            {
                // Just in case the user quick set to large.
                OxyPlotModel.SetAxesVisibility(uiOxyPlot, false);
            }
        }

        uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines
    }

    #region Exporters

    /// <summary>
    /// Called from MainWindow when the user asks for, e.g., exported data or graphs. Most sensors will 
    /// support all these options.
    /// </summary>
    public IDeviceControlBasic.UXCapabilities GetUXCapabilities()
    {
        var retval = IDeviceControlBasic.UXCapabilities.CanRename;
        if (HasSensorData)
        {
            retval |=
            IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng
            | IDeviceControlBasic.UXCapabilities.CanGetData
            | IDeviceControlBasic.UXCapabilities.CanShowTable
            ;
        }
        return retval;
    }

    public async void ExportGraphAsPng()
    {
        await UtilitiesWinUI3.UtilitiesWinUI3.ExportGraphAsPngAsync(uiOxyPlot, rootPanel.Background, Log);
    }


    public string GetDetails(IDeviceControlBasic.DetailsType detailsType)
    {
        return "Internal error: no details are available";
    }
    #endregion

} // end of class BTStandard_HeartRateControl
