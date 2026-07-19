using BluetoothProtocols;
using BluetoothProtocolsDevicesCore;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using BluetoothWinUI3.BTDeviceUnitConverters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.

using Utilities;
using Windows.Devices.Bluetooth;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif


#region Change these to match your device
using DeviceSpecificType = BTStandard_Demo; // Change: pick your device, not BTStandard_Demo
using DeviceSpecificSensorData = BTStandard_Demo.Battery_Data; // Change: 
using DeviceSpecificSensorSecondaryData = BTStandard_Demo.Common_Configuration_Data; // Change: pick secondary sensor if needed
using DeviceSpecificBatteryData = BTStandard_Demo.Battery_Data; // Change: many device support battery
#endregion

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTStandard_DemoControl : UserControl, IDeviceControlBasic, IDeviceControlDevice // Change: change the name from BTStandard_DemoControl
{
    #region Change these settings that must be updated for a new device
    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "BTStandard_Demo"; // Change: change the BTStandard_Demo string to match your device. The exact name does not matter.
    #endregion

    #region Change these advanced settings only when needed (most devices won't change these)
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

    public BTStandard_DemoControl() // CHANGE: change the name to match the changed class name
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += Control_DataContextChanged;
    }

    #region Instance value for a device (not changed)
    DeviceSpecificType Device;
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;
    ulong OriginalBTAddr = 0xFFFFFFFF_FFFFFFFF;

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the collection.
    /// </summary>
    public DataCollection<DeviceSpecificSensorData> HistoricalDataUnits { get; } = new();
    public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return HistoricalDataUnits.Data; }

    // CHANGE: some devices (like the heart rate) also have fine grained data.
    public void ClearAccumulatedFineGrainedData() 
    {
        ;  // do nothing
    }
    public IBTCommonMetaData GetDataMostRecent()
    {
        return HistoricalDataUnits.GetDataMostRecent();
    }


    // This control show two kinds of data. 
    // 1. Battery data is the "sensor data" which is the data to be graphed
    // and displayed in a table. 
    //
    // 2. Configuration data which is just displayed to the user
    //

    /// <summary>
    /// Current sensor data from the Device. For the demo, it's battery level.
    /// </summary>
    DeviceSpecificSensorData CurrSensor_Data = null;
    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    DeviceSpecificSensorData CurrSensor_DataUnits = null;

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

    /// <summary>
    /// Data directly from the device. It's always in the original units from the device
    /// and isn't converted into the user's preferred units.
    /// </summary>
    DeviceSpecificSensorSecondaryData CurrSensorSecondary_Data = null;

    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    DeviceSpecificSensorSecondaryData CurrSensorSecondary_DataUnits = null;
    #endregion

    #region Instance values for the UX (not changed)
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
    };
    #endregion

    private void Control_Loaded(object sender, RoutedEventArgs e)
    {
        InitializeUX();
    }


    bool InitializeUXCalled = false;
    /// <summary>
    /// Code to initialize the UX. Will be called both from Control_Loaded and from
    /// DataContextChanged
    /// </summary>
    private void InitializeUX()
    {
        // Loaded gets called both when it's first loaded and also each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        // We only want to do work the first time.

        if (InitializeUXCalled) return;
        InitializeUXCalled = true;

        #region Change to set up the sparkles and graph

        // Change: set the right sparkles.
        // The string is the INPC name from the device, and the Run is the corresponding Sparkle text.
        ControlsWithSparkles = new List<(string, Microsoft.UI.Xaml.Documents.Run)>()
        {
            ( DeviceSpecificType.BatteryLevelPropertyChangedName, uiSensorChange),
            ( DeviceSpecificType.Connection_ParameterPropertyChangedName, uiConnection_ParametersChange),
        };

        // Change: set up the graph by making an OxyPlotModel
        OxyPlotModel = OxyPlotUtilities.MakeOxyPlotModelSimple("Sensor Data", 10, 30, "Battery", "BatteryLevel");
        // "Sensor Data" is for the main graph title  and is human-readable
        // "Battery" for the axis title and for the color settings in the menus and should be concise and human-readable
        // "BatteryLevel" is the underlying sensor property name and must exactly match the C# name.
        #endregion


        // This oxyplot and table code is always the same and doesn't need to be changed.
        OxyPlotUtilities.InitializeOxyPlotData(uiOxyPlot, OxyPlotModel, HistoricalDataUnits.Data);
        OxyPlotUtilities.InitializeLineNamesFromOxyPlotModel(LineNames, OxyPlotModel);

        //
        // Set up the uiTableView
        // https://w-ahmad.dev/WinUI.TableView/index.html
        // https://github.com/w-ahmad/WinUI.TableView
        //
        uiTableView.AutoGeneratingColumn += CurrTableCustomization.TableView_AutoGeneratingColumn_UseCustomization;
        uiTableView.ItemsSource = HistoricalDataUnits.Data;
    }

    // Allows the control to provide feedback to Windows about updates to the device capabilties.
    // For example, the device might not have a sensor, and so the user shouldn't be able 
    // see the table or graph.
    IHandleNotifyDeviceControlChanges NotifyDeviceControlChangesWindows = null;

    /// <summary>
    /// Called by MainWindow so this control knows who to contact based on device changes.
    /// Often there are no changes
    /// </summary>
    public void SetNotifyDeviceControlChanges(IHandleNotifyDeviceControlChanges mainWindow)
    {
        NotifyDeviceControlChangesWindows = mainWindow;
    }

    // If you have to update these dynamically, be sure to call 
    // NotifyDeviceControlChangesWindows.OnGetUXCapabilitiesChanged
    // so the main window menus get updated.

    // The LineNames is set up in the Loaded from the call to OxyPlotUtilities.InitializeLineNamesFromOxyPlotModel
    List<string> _LineNames = new () { };
    /// <summary>
    /// List of line names in the plot. This is set up directly from the OxyPlotModel. The line names
    /// are needed so the MainWindow can set up the list of changeable line colors in the plot.
    /// </summary>
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
    /// </summary>
    // H.OxyPlot
    private PlotModel OxyPlotModel { get; set; } = null;



    /// <summary>
    /// Loop through the LineSeries for where a matching DataFieldY. This is used by the MainWindow
    /// when setting some stuff up.
    /// </summary>
    public uint GetGraphColor(string axisTitle)
    {
        return UtilitiesWinUI3.UtilitiesWinUI3.GetGraphColor(OxyPlotModel, axisTitle);
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
        InitializeUX(); // ensure we're initialized.

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
        uiAddress.Text = DataContextAsKnownDevice.Advertisement.AddressAsString;
        CurrSaveData = AllSaveData.FindWithAdvertisementAddress(DataContextAsKnownDevice.Advertisement.Addr); // Has already been saved, so will exist.

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
        CurrSaveData = AllSaveData.SwitchToDeviceIdCurrSaveData(CurrSaveData, DataContextAsKnownDevice);

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        UtilitiesWinUI3.UtilitiesWinUI3.InitializeKeyLineColorsFromDefaultOxyPlot(OxyPlotModel, rootPanel);
        UpdateUX(CurrSaveData); // Can be null when the user hasn't made any changes
        KnownDeviceName = DataContextAsKnownDevice.Advertisement?.BestName ?? KnownDeviceName;
        uiKnownDeviceName.Text = KnownDeviceName;

        Device.PropertyChanged += Device_PropertyChanged;

        #region Change so the device starts sending notifications for changed properties (data)

        // Change: tell the device to start sending sensor data back.
        // The demo code uses the battery level as the sensor.
        await Device.NotifyBatteryLevelAsync(); // CHANGE: set up the right notifications for your device.
        var sensordata = await Device.ReadBatteryLevel(DefaultCacheMode);

        if (sensordata == null)
        {
            // Happens in the Demo code when the device doesn't report a battery level (e.g., JBL Pro 4 Speakers,
            // but lots of others). Usually sensordata is always present.
            HasSensorData = false;
            RemoveSensorDataUx();
        }


        // Verify that your device has a battery characteristic. If your device does not,
        // just SetBatteryVisibility(Visibility.Collapsed); without further notice.
        var batterydata = await Device.ReadBatteryLevel(DefaultCacheMode);
        if (batterydata == null)
        {
            uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
        }

        // Some UX needs additional information
        await Device.ReadDevice_Name(DefaultCacheMode);
        // How this works: when you call the Read call, in addition to returning data it will
        // also call the Device.PropertyChanged (INPC) callback. In my code, it's handy to just
        // have all the UX update code for handling changes in the same place, so I just
        // ignore the return value here.
        await Device.ReadConnection_Parameter(DefaultCacheMode);
        #endregion

        // The system tracks device changes
        // Can't do this earlier; merely calling FromBluetoothAddressAsync doesn't actually 
        // connect. Once we do the notify and reads the device will be connected or not.
        CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, Device.ble.ConnectionStatus);
    }


    /// <summary>
    /// Called from DataContextChanged when a device does not, in fact, have a sensor. This 
    /// removes the grpah and table from the display (no sensor means no data) and tells
    /// the MainWindow to update its UX accordingly.
    /// </summary>
    private void RemoveSensorDataUx()
    {
        uiDeviceDataList.Items.Remove(uiDeviceDataSensor);
        LineNames.Clear();
        uiOxyPlot.Visibility = Visibility.Collapsed;
        uiTableView.Visibility = Visibility.Visible;

        // Notify MainWindow that the UX capabilities have changed. This might change
        // the UX (e.g., device> show graph/table might be removed)
        // Will also trigger redoing the graph line names via LineNames, which
        // technically isn't quite in accordance with the name.
        NotifyDeviceControlChangesWindows?.OnGetUXCapabilitiesChanged(this, GetUXCapabilities());
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
    public void SetDataGridVisibility(IDeviceControlBasic.Visibility visibility)
    {
        UtilitiesWinUI3.UtilitiesWinUI3.SetDataGridVisibility(uiOxyPlot, uiDataGridPanel, visibility);
    }


    /// <summary>
    /// Updates the OxyPlot line with a given name (e.g., "Temperature" or "Heart Rate"). Is called from MainWindow when the
    /// user picks a new color.
    /// </summary>
    public void UpdateGraphColor(string axisTitle, uint color)
    {
        UtilitiesWinUI3.UtilitiesWinUI3.UpdateGraphColor(OxyPlotModel, rootPanel, axisTitle, color);
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
        foreach (var (axisTitle, color) in colors.GraphColors)
        {
            UpdateGraphColor(axisTitle, color);
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
            #region Change to update the data based on user preferred units (e.g, C versus F)
            // For the BTStandard_Demo, there are no units to change
            if (oldPrefs != null && newPrefs.Distance != oldPrefs.Distance)
            {
                // Change: based on your knowledge of the sensor data, change the distance readings.
                // data.Distance = BluetoothWatcher.Units.Distance.Convert(data.Distance, oldPrefs.Distance, CurrUserPrefs.Distance);
            }
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
            #endregion
        }

        UpdateDeviceDataUX(""); // all of them.
    }

    /// <summary>
    /// Standard: the normal way to resize the control. 
    /// </summary>
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

    /// <summary>
    /// Updates the sparkles based on the changed property. Called from UpdateDeviceDataUX which is
    /// called by Device_PropertyChanged when a device property changes.
    /// </summary>
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


    #region Change to update the UX when the device says there's new data
    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated. Most often called from Device_PropertyChanged
    /// </summary>
    private void UpdateDeviceDataUX(string name)
    {
        if (Device == null) return;
        UpdateSparkles(name); // name is from e.PropertyName when the Device does a PropertyChanged.


        // Change: Always update these even though in practice they are only set once.
        CurrSensor_Data = Device.CurrBattery_Data; // Change: select the right data
        CurrSensorSecondary_Data = Device.CurrCommon_Configuration_Data; // Change: pick secondary data as appropriate
        CurrBattery_Data = Device.CurrBattery_Data; // Change: if your device doesn't have a battery, remove battery stuff!


        // Update data from the device to match the current preferred units. Will create the values as needed.
        CurrSensor_DataUnits = DeviceSpecificSensorData.CopyToWithConvertAndCreate(CurrSensor_Data, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
        CurrSensorSecondary_DataUnits = DeviceSpecificSensorSecondaryData.CopyToWithConvertAndCreate(CurrSensorSecondary_Data, CurrSensorSecondary_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
        CurrBattery_DataUnits = DeviceSpecificBatteryData.CopyToWithConvertAndCreate(CurrBattery_Data, CurrBattery_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

        // Change all this code to match your device and UX.
        switch (name)
        {
            case DeviceSpecificType.Connection_ParameterPropertyChangedName: // Change: update the UX as appropriate
                uiInterval_Min.Text = CurrSensorSecondary_DataUnits.Interval_Min.ToString("F2");
                uiInterval_Max.Text = CurrSensorSecondary_DataUnits.Interval_Max.ToString("F2");
                uiLatency.Text = CurrSensorSecondary_DataUnits.Latency.ToString("F2");
                uiTimeout.Text = CurrSensorSecondary_DataUnits.Timeout.ToString("F2");
                break;


            // For the Demo, the "sensor" is just the battery level. In your code, hook up the 
            // right sensors to the right XAML
            case "*": // never used, but here so it matches the environment code.
            case DeviceSpecificType.BatteryLevelPropertyChangedName:
                uiSensor.Text = CurrSensor_DataUnits.BatteryLevel.ToString("F2"); // Change: update the UX as appropriate

                // Only the sensor data gets plotted as historical data. In the demo,
                // other values are also read (e.g., the Interval_Min), but they aren't
                // part of the sensor data that's plotted.
                // The historical data is updated from the CurrSensor_DataUnits
                UpdateHistoricalDataAndGraph(CurrSensor_DataUnits);
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
    #endregion


    /// <summary>
    /// Helper code to update historical data. The sensor might send a lot of data; the history only
    /// saves a portion of the data. Technicaly, every time there's new data we either update
    /// the most recent entry OR we add a new entry.
    /// </summary>
    private void UpdateHistoricalDataAndGraph(DeviceSpecificSensorData currSensor_DataUnits)
    {
        var deltaInSeconds = currSensor_DataUnits.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
        var verb = (deltaInSeconds > HistoricalDataUpdateRateInSeconds)
            ? DataCollection<DeviceSpecificSensorData>.Verb.Add : DataCollection<DeviceSpecificSensorData>.Verb.ReplaceMostRecent;
        HistoricalDataUnits.Update(currSensor_DataUnits, verb); // Will add or replace the data and will copy as needed.

        //
        // Update the OxyPlot because it doesn't track the INotifyCollectionChanged
        //
        if (verb == DataCollection<DeviceSpecificSensorData>.Verb.Add && HistoricalDataUnits.Count == 2)
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

    #region Exporters don't need to be changed

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

    /// <summary>
    /// A small number of controls have this as a specialty value. For example, the 
    /// BTServicesAndCharacteristics control uses it to "dump" all of the seen 
    /// advertisements or the discovered services + characteristics to the clipboard.
    /// </summary>
    public string GetDetails(IDeviceControlBasic.DetailsType detailsType)
    {
        return "Internal error: no details are available";
    }
    #endregion

} // end of class BTStandard_DemoControl // CHANGE: update the comment to match the class name
