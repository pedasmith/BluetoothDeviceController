#region Usings stay the same
using BluetoothProtocols;
using BluetoothProtocolsDevicesCore;
using BluetoothProtocolsDevicesCoreExtensions.TI_SensorTag_Extensions;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using BluetoothWinUI3.BTDeviceUnitConverters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.
using System.Threading.Tasks;
using Utilities;
using UtilitiesWinUI3;
using Windows.Devices.Bluetooth;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif

#endregion
// Modify these to match your device
using DeviceSpecificType_2541 = TI_SensorTag_2541; // Modify: pick your device, not BTSimple_Demo
using DeviceSpecificType_1350 = TI_SensorTag_1350; // Modify: pick your device, not BTSimple_Demo
using DeviceSpecificType_1352 = TI_SensorTag_1352; // Modify: pick your device, not BTSimple_Demo
using DeviceSpecificSensorData_2541 = TI_SensorTag_2541.Humidity_Data; // Modify: 
using DeviceSpecificSensorData_1350 = TI_SensorTag_1350.Barometer_Data; // Modify: 
using DeviceSpecificSensorData_1352 = TI_SensorTag_1352.Temperature_Data; // Modify: 
using DeviceSpecificSensorSecondaryData_1350 = TI_SensorTag_1350.Humidity_Data; // Modify: pick secondary sensor if needed
using DeviceSpecificSensorSecondaryData_1352 = TI_SensorTag_1352.Humidity_Data; // Modify: pick secondary sensor if needed
using DeviceSpecificBatteryData_1350 = TI_SensorTag_1350.Battery_Data; // Modify: many device support battery
using DeviceSpecificBatteryData_1352 = TI_SensorTag_1352.Battery_Data; // Modify: many device support battery

using DeviceSpecificSensorDataFacade = BluetoothProtocolsDevicesCoreExtensions.TI_SensorTag_Extensions.Environment_Data; // Modify: 



[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTTI_SensorTagControl : UserControl, IDeviceControlBasic, IDeviceControlDevice // Modify: rename to match your device
{
    // Modify these settings to match your device
    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "TI_SensorTag"; // Modify: edit the string to match your device. The exact name does not matter.

    /// <summary>
    /// Tags for the device. This is used to categorize the different devices.
    /// Common tags: environment exersise health cooking agriculture light
    /// </summary>
    public string Tags { get { return "#environment"; } }

    // Modify these advanced settings only when needed (most devices won't update these)
    /// <summary>
    /// Most developer never need to switch this from 'true'!
    /// Ususually a device always has their sensor data. But some devices are might not. 
    /// The HasSensorData shows how to handle the case of your device not always having
    /// the sensor.
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

    public BTTI_SensorTagControl() // Modify: edit the name to match the class name
    {
        Initialize(); // Initialization that will stay the same
    }

    #region Instance value for a device stay the same
    private void Initialize()
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += Control_DataContextChanged;
    }

    DeviceSpecificType_2541 Device_2541 = null;
    DeviceSpecificType_1350 Device_1350 = null;
    DeviceSpecificType_1352 Device_1352 = null;
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;
    ulong OriginalBTAddr = 0xFFFFFFFF_FFFFFFFF;

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the collection.
    /// </summary>
    public DataCollection<DeviceSpecificSensorDataFacade> HistoricalDataUnits { get; } = new();
    public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return HistoricalDataUnits.Data; }
    #endregion

    // Modify: some devices (like the heart rate) also have fine grained data.
    // Most devices do not; it's OK to just return data every 5 second or so
    public void ClearAccumulatedFineGrainedData()
    {
        ;  // do nothing
    }

    #region Historical data methods and fields stay the same
    /// <summary>
    /// Called from MainWindow when the user wants to clear their graph
    /// </summary>
    public void ClearData()
    {
        HistoricalDataUnits.Data.Clear();
    }

    public IBTCommonMetaData GetDataMostRecent()
    {
        return HistoricalDataUnits.GetDataMostRecent();
    }


    /// <summary>
    /// Current sensor data from the Device. For the demo, it's battery level.
    /// </summary>
    DeviceSpecificSensorData_2541 CurrSensor_Data_2541 = null;
    DeviceSpecificSensorData_1350 CurrSensor_Data_1350 = null;
    DeviceSpecificSensorData_1352 CurrSensor_Data_1352 = null;
    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    DeviceSpecificSensorDataFacade CurrSensor_DataUnits = null;

    /// <summary>
    /// Making a battery value that's seperate from the Sensor. This lets the programmer
    /// copy-paste data, pick a new sensor, and the battery stuff will still work.
    /// </summary>
    DeviceSpecificType_1350.Battery_Data CurrBattery_Data_1350 = null;
    DeviceSpecificType_1352.Battery_Data CurrBattery_Data_1352 = null;
    /// <summary>
    /// Just like CurrBattery_Data but in user-preferred units. For battery, the units
    /// are actually OK as is
    /// </summary>
    DeviceSpecificType_1350.Battery_Data CurrBattery_DataUnits_1350 = null;
    DeviceSpecificType_1352.Battery_Data CurrBattery_DataUnits_1352 = null;

    /// <summary>
    /// Data directly from the device. It's always in the original units from the device
    /// and isn't converted into the user's preferred units.
    /// </summary>
    DeviceSpecificSensorSecondaryData_1350 CurrSensorSecondary_Data_1350 = null;
    DeviceSpecificSensorSecondaryData_1352 CurrSensorSecondary_Data_1352 = null;

    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    // Not used; is placed into the EnvironmentData class: DeviceSpecificSensorSecondaryData CurrSensorSecondary_DataUnits = null;
    #endregion

    #region Instance values for the UX stay the same
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
        // Actually only update when data context is updated. Only then do we know the device type and HasPressure is set.
        // InitializeUX();
    }

    bool InitializeUXCalled = false;

    /// <summary>
    /// Code to initialize the UX. Will be called both from Control_Loaded and from
    /// DataContextChanged
    /// </summary>
    private void InitializeUX()
    {
        // InitializeUX gets called both when it's first loaded and also each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        // We only want to do work the first time.

        if (InitializeUXCalled) return;
        InitializeUXCalled = true;

        // Modify to set up the sparkles and graph

        // Modify: set the right sparkles.
        // The string is the INPC name from the device, and the Run is the corresponding Sparkle text.
        ControlsWithSparkles = new List<(string, Microsoft.UI.Xaml.Documents.Run)>()
        {
            ( DeviceSpecificType_2541.Humidity_DataPropertyChangedName, uiHumidityChange),
            ( DeviceSpecificType_1350.Barometer_DataPropertyChangedName, uiTemperatureChange),
            ( DeviceSpecificType_1350.Barometer_DataPropertyChangedName, uiPressureChange),
            ( DeviceSpecificType_1352.Temperature_DataPropertyChangedName, uiTemperatureChange),
            ( DeviceSpecificType_1352.Humidity_DataPropertyChangedName, uiHumidityChange),
        };

        // Modify: set up the graph by making an OxyPlotModel and adding lines to it.
        // The line data must exist in the HistoricalData
        OxyPlotModel = OxyPlotUtilities.MakeOxyPlotModel("TI Sensor Tag")
            .AddLine(10, 30, "Temperature", "Temperature")
            .AddLine(10, 5, "Humidity", "Humidity")
            ;
        if (HasPressure)
        {
            OxyPlotModel.AddLine(5, 10, "Pressure", "Pressure");
        }


        // "Sensor Data" is for the main graph title  and is human-readable
        // "Ambient Temperature" for the axis title and for the color settings in the menus and should be concise and human-readable
        // "Temperature" is the underlying sensor property name and must exactly match the C# name.

        // Your sensor might include properties that aren't interesting to see in the table view.
        // Note that this table is the visible table; the exported data is set differently.
        if (!HasPressure)
        {
            CurrTableCustomization.TableColumnsToExclude.Add("Pressure");
        }
        CurrTableCustomization.TableColumnsToExclude.Add("HasPressure");

        // end modifications

        InitializeSparklesOxyplotTables();
    }

    #region Code to initialize some of the UX components after the customization is setup up. Will stay the same
    private void InitializeSparklesOxyplotTables()
    {
        // This sparkles, oxyplot, and table code is always the same and doesn't need to be edited.
        SparklesHelper.InitializeSparkles(ControlsWithSparkles);

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
    /// Called by MainWindow so this control knows who to contact based on device schema updates.
    /// Often there are no updates
    /// </summary>
    public void SetNotifyDeviceControlChanges(IHandleNotifyDeviceControlChanges mainWindow)
    {
        NotifyDeviceControlChangesWindows = mainWindow;
    }

    // If you have to update these dynamically, be sure to call 
    // NotifyDeviceControlChangesWindows.OnGetUXCapabilitiesChanged
    // so the main window menus get updated.

    // The LineNames is set up in the Loaded from the call to OxyPlotUtilities.InitializeLineNamesFromOxyPlotModel
    List<string> _LineNames = new() { };
    /// <summary>
    /// List of line names in the plot. This is set up directly from the OxyPlotModel. The line names
    /// are needed so the MainWindow can set up the list of editable line colors in the plot.
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
    /// If the device had become disconnected, the control uses this (via BTConnectionControl.GotAnotherAdvertisement)
    /// to trigger a reconnect attempt. GotAnotherAdvertisement is smart and will only reconnect as appropriate.
    /// </summary>
    public async Task HandleMyAdvertisementAsync(WatcherData data)
    {
        await uiBTConnectionControl.GotAnotherAdvertisementAsync();
    }

    /// <summary>
    /// This is a two-way street. Setting the DataContest to the KnownDevice will update some UX and will
    /// trigger looking up the SaveData and update more things. And it will actually connect to the device.
    /// AND this will update the KnownDevice with, e.g., the DeviceId and the BluetoothLEDevice which will be
    /// used by other bits of the system.
    /// </summary>
    private async void Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // FYI: by the time this method is called, the DataContext in the object is already set

        if (args.NewValue == null) return; // just bogus; ignore.

        CurrSensorType = TI_SensorTag.AdvertIsSensorFamily(DataContextAsKnownDevice.Advertisement);
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                break;
            case TI_SensorTag.SensorType.TI_1350:
                HasPressure = true;
                break;
            case TI_SensorTag.SensorType.TI_1352:
                break;
        }

        InitializeUX(); // ensure we're initialized.
        uiBTConnectionControl.SetDeviceControl(this);
        if (OriginalBTAddr != 0xFFFFFFFF_FFFFFFFF)
        {
            ; // duplicate call!
            return;
        }
        await ReconnectAsync();
    }
    #endregion
    private TI_SensorTag.SensorType CurrSensorType = TI_SensorTag.SensorType.NotThisSensorFamily;
    private bool HasPressure = false; // Most sensors don't have pressure sensor

    /// <summary>
    /// Called by e.g., the ConnectionControl when the user wants to reconnect to the device (sensor).
    /// The initial connect is handled by the controls in Control_DataContextChanged() when the
    /// control DataContexts is set
    /// 
    /// Also called by Control_DataContextsChanged for the first connect
    /// </summary>
    public async Task ReconnectAsync()
    {
        #region Normal device setup stays the same
        // Must have been set as a KnownDevice; otherwise we're in a very weird state.
        // DataContxtAsKnownDevice is just the DataContext cast (with an "as") to KnownDevice.
        if (DataContextAsKnownDevice == null)
        {
            Log($"Impossible Error: {InternalDeviceType}: Data context change, but it's not a KnownDevice. Type is {DataContext.GetType()}");
            return;
        }

        OriginalBTAddr = DataContextAsKnownDevice.Advertisement.Addr;
        uiAddress.Text = DataContextAsKnownDevice.Advertisement.AddressAsString;
        CurrSaveData = AllSaveData.FindWithAdvertisementAddress(DataContextAsKnownDevice.Advertisement.Addr); // Has already been saved, so will exist.

        CurrSensorType = TI_SensorTag.AdvertIsSensorFamily(DataContextAsKnownDevice.Advertisement);
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                Device_2541 = new DeviceSpecificType_2541()
                {
                    ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
                };
                if (Device_2541.ble == null)
                {
                    // ConnectError:NoBLE
                    Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
                    CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
                    return;
                }
                DataContextAsKnownDevice.Id = Device_2541.ble.DeviceId ?? ""; // never null :-)
                DataContextAsKnownDevice.BTLEDevice = Device_2541.ble;
                break;
            case TI_SensorTag.SensorType.TI_1350:
                Device_1350 = new DeviceSpecificType_1350()
                {
                    ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
                };
                if (Device_1350.ble == null)
                {
                    // ConnectError:NoBLE
                    Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
                    CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
                    return;
                }
                HasPressure = true;
                DataContextAsKnownDevice.Id = Device_1350.ble.DeviceId ?? ""; // never null :-)
                DataContextAsKnownDevice.BTLEDevice = Device_1350.ble;
                break;
            case TI_SensorTag.SensorType.TI_1352:
                Device_1352 = new DeviceSpecificType_1352()
                {
                    ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
                };
                if (Device_1352.ble == null)
                {
                    // ConnectError:NoBLE
                    Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
                    CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
                    return;
                }
                DataContextAsKnownDevice.Id = Device_1352.ble.DeviceId ?? ""; // never null :-)
                DataContextAsKnownDevice.BTLEDevice = Device_1352.ble;
                break;
        }

        // It's critical to set these!
        CurrSaveData = AllSaveData.SwitchToDeviceIdCurrSaveData(CurrSaveData, DataContextAsKnownDevice);

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        UtilitiesWinUI3.UtilitiesWinUI3.InitializeKeyLineColorsFromDefaultOxyPlot(OxyPlotModel, rootPanel);
        UpdateUX(CurrSaveData); // Can be null when the user hasn't made any changes
        KnownDeviceName = DataContextAsKnownDevice.Advertisement?.BestName ?? KnownDeviceName;
        uiKnownDeviceName.Text = KnownDeviceName;

        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                Device_2541.PropertyChanged += Device_PropertyChanged;
                Device_2541.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
                Device_2541.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged;
                break;
            case TI_SensorTag.SensorType.TI_1350:
                Device_1350.PropertyChanged += Device_PropertyChanged;
                Device_1350.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
                Device_1350.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged;
                break;
            case TI_SensorTag.SensorType.TI_1352:
                Device_1352.PropertyChanged += Device_PropertyChanged;
                Device_1352.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
                Device_1352.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged;
                break;
        }
        bool connectAllOk = true;
        uiBTConnectionControl.CurrState = BTConnectionControl.ConnectionState.Connecting;
        #endregion

        // Modify so the device starts sending notifications for changed properties (data)

        // Modify: tell the device to start sending sensor and battery data back.
        bool removePressure = true;
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                await Device_2541.NotifyHumidity_DataAsync();
                // No battery on teh 2541: await Device_2541.NotifyBattery_DataAsync(); // Modify: set up the right notifications for your device.

                await Device_2541.WriteHumidity_Configure([1]); // turn it on!
                break;

            case TI_SensorTag.SensorType.TI_1350:
                await Device_1350.NotifyBarometer_DataAsync(); // Modify: set up the right notifications for your device.
                await Device_1350.NotifyHumidity_DataAsync(); // Modify: set up the right notifications for your device.
                await Device_1350.NotifyBatteryLevelAsync(); // Modify: set up the right notifications for your device.

                await Device_1350.WriteBarometer_Config([1]); // turn it on!
                await Device_1350.WriteHumidity_Config([1]); // turn it on!
                removePressure = false; // 1350 is the only tag that does pressure
                break;

            case TI_SensorTag.SensorType.TI_1352:
                await Device_1352.NotifyTemperature_DataAsync(); // Modify: set up the right notifications for your device.
                await Device_1352.NotifyHumidity_DataAsync(); // Modify: set up the right notifications for your device.
                await Device_1352.NotifyBattery_DataAsync(); // Modify: set up the right notifications for your device.

                await Device_1352.WriteTemperature_Conf([1]); // turn it on!
                await Device_1352.WriteHumidity_Conf([1]); // turn it on!
                break;
        }

        if (removePressure)
        {
            uiDeviceDataList.Items.Remove(uiPressurePanel);
        }

        // Verify that your device has a battery characteristic. If your device does not,
        // just SetBatteryVisibility(Visibility.Collapsed); without further notice.
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541: // no battery data on the 2541
                uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
                break;
            case TI_SensorTag.SensorType.TI_1350:
                {
                    var batterydata = connectAllOk ? await Device_1350.ReadBatteryLevel(DefaultCacheMode) : null;
                    if (batterydata == null)
                    {
                        uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
                    }
                }
                break;
            case TI_SensorTag.SensorType.TI_1352:
                {
                    var batterydata = connectAllOk ? await Device_1352.ReadBattery_Data(DefaultCacheMode) : null;
                    if (batterydata == null)
                    {
                        uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
                    }
                }
                break;
        }


        // Some UX needs additional information
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                connectAllOk = connectAllOk && (await Device_2541.ReadDevice_Name(DefaultCacheMode)) != null;
                break;
            case TI_SensorTag.SensorType.TI_1350:
                connectAllOk = connectAllOk && (await Device_1350.ReadDevice_Name(DefaultCacheMode)) != null;
                break;
            case TI_SensorTag.SensorType.TI_1352:
                connectAllOk = connectAllOk && (await Device_1352.ReadDevice_Name(DefaultCacheMode)) != null;
                break;
        }

        // The system tracks device changes
        // Can't do this earlier; merely calling FromBluetoothAddressAsync doesn't actually 
        // connect. Once we do the notify and reads the device will be connected or not.
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, Device_2541.ble.ConnectionStatus);
                break;
            case TI_SensorTag.SensorType.TI_1350:
                CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, Device_1350.ble.ConnectionStatus);
                break;
            case TI_SensorTag.SensorType.TI_1352:
                CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, Device_1352.ble.ConnectionStatus);
                break;
        }

    }


    #region Update glue code stays the same
    /// <summary>
    /// Called when the BLE device connection status changes.
    /// </summary>
    private void Ble_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        // Choices for ConnectionStatus is just Disconnected and Connected 
        uiBTConnectionControl.SetState(sender.ConnectionStatus);
        UIThreadHelper.CallOnUIThread(() => { Log($"{InternalDeviceType}: Status update: {sender.ConnectionStatus}"); });
    }

    /// <summary>
    /// Called from the protocol CS file when a read or notify (etc) happen. Will send a bunch
    /// of OK / OK / OK alongn with an occassional Fail.
    /// </summary>
    private void Status_OnBluetoothStatus(object source, BluetoothCommunicationStatus status)
    {
        uiBTConnectionControl.SetState(status);
        UIThreadHelper.CallOnUIThread(() => { Log($"{InternalDeviceType}: Status update: {status.AsStatusString}"); });
        ;
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
    /// Updates the OxyPlot and highlights a given line OR clears the highlight if the
    /// lineTag is !CLEAR
    /// </summary>
    public void HighlightGraphLine(string lineTag)
    {
        OxyPlotModel.DoHighlightGraphLine(uiOxyPlot, lineTag);
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
    #endregion

    /// <summary>
    /// UserPreferences are for the app as a whole, not for this particular device. For example: the preferred temperature unit.
    /// </summary>
    public void UpdateUX(UserPreferences newPrefs, UserPreferences oldPrefs)
    {
        CurrUserPrefs = newPrefs;

        // Update the saved data in the HistoricalDataUnits to match the new user preferences.
        foreach (var data in HistoricalDataUnits.Data)
        {
            // Modify to update the data based on user preferred units (e.g, C versus F)
            // For the BTSimple_Demo, there is just the temperature
            if (oldPrefs != null && newPrefs.Distance != oldPrefs.Distance)
            {
                // Modify: based on your knowledge of the sensor data, edit the distance readings.
                // data.Distance = BluetoothWatcher.Units.Distance.Convert(data.Distance, oldPrefs.Distance, CurrUserPrefs.Distance);
            }
            if (oldPrefs != null && newPrefs.Temperature != oldPrefs.Temperature)
            {
                // Modify: based on your knowledge of the sensor data, edit the temperature readings.
                data.Temperature = BluetoothWatcher.Units.Temperature.Convert(data.Temperature, oldPrefs.Temperature, CurrUserPrefs.Temperature);
            }
            if (oldPrefs != null && newPrefs.Pressure != oldPrefs.Pressure)
            {
                // Modify: based on your knowledge of the sensor data, edit the pressure readings.
                data.Pressure = BluetoothWatcher.Units.Pressure.Convert(data.Pressure, oldPrefs.Pressure, CurrUserPrefs.Pressure);
            }
        }

        UpdateDeviceDataUX(""); // all of them.
    }

    #region Glue code stays the same
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

    SparklesHelper SparklesHelper = new();

    private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        UIThreadHelper.CallOnUIThread(() =>
        {
            if (!IsLoaded) return;
            CurrSaveData?.History.UpdateDataHistory(DateTimeOffset.Now);
            UpdateDeviceDataUX(e.PropertyName);
        });
    }

    #endregion

    // Modify to update the UX when the device says there's new data
    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated. Most often called from Device_PropertyChanged
    /// </summary>
    private void UpdateDeviceDataUX(string name)
    {
        if (Device_2541 == null && Device_1350 == null && Device_1352 == null) return;
        SparklesHelper.UpdateSparkles(ControlsWithSparkles, name); // name is from e.PropertyName when the Device does a PropertyChanged.


        // Modify: Always update these even though in practice they are only set once.
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                CurrSensor_Data_2541 = Device_2541?.CurrHumidity_Data; // Modify: pick secondary data as appropriate
                CurrSensor_DataUnits = DeviceSpecificSensorDataFacade.CopyToWithConvertAndCreate(CurrSensor_Data_2541, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
                break;
            case TI_SensorTag.SensorType.TI_1350:
                CurrSensor_Data_1350 = Device_1350?.CurrBarometer_Data; // Modify: select the right data
                CurrSensorSecondary_Data_1350 = Device_1350?.CurrHumidity_Data; // Modify: pick secondary data as appropriate
                CurrSensor_DataUnits = DeviceSpecificSensorDataFacade.CopyToWithConvertAndCreate(CurrSensor_Data_1350, CurrSensorSecondary_Data_1350, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

                CurrBattery_Data_1350 = Device_1350?.CurrBattery_Data; // Modify: if your device doesn't have a battery, remove battery stuff!
                CurrBattery_DataUnits_1350 = DeviceSpecificBatteryData_1350.CopyToWithConvertAndCreate(CurrBattery_Data_1350, CurrBattery_DataUnits_1350, KnownDeviceName, CurrUserPrefs.Convert);
                break;
            case TI_SensorTag.SensorType.TI_1352:
                CurrSensor_Data_1352 = Device_1352?.CurrTemperature_Data; // Modify: select the right data
                CurrSensorSecondary_Data_1352 = Device_1352?.CurrHumidity_Data; // Modify: pick secondary data as appropriate
                CurrSensor_DataUnits = DeviceSpecificSensorDataFacade.CopyToWithConvertAndCreate(CurrSensor_Data_1352, CurrSensorSecondary_Data_1352, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

                CurrBattery_Data_1352 = Device_1352?.CurrBattery_Data; // Modify: if your device doesn't have a battery, remove battery stuff!
                CurrBattery_DataUnits_1352 = DeviceSpecificBatteryData_1352.CopyToWithConvertAndCreate(CurrBattery_Data_1352, CurrBattery_DataUnits_1352, KnownDeviceName, CurrUserPrefs.Convert);
                break;
        }

        // Check to make sure the data is OK
        bool incompleteData = false;
        if (CurrSensor_DataUnits.TimestampMostRecent.Year < 2000) incompleteData = true;
        if (HasPressure && CurrSensor_DataUnits.Pressure == 0) incompleteData = true;
        if (incompleteData) return;

        // Modify: Edit all this code to match your device and UX.
        switch (name)
        {
            // In your code, hook up the right sensors to the right XAML
            case DeviceSpecificType_2541.IR_DataPropertyChangedName:
            case DeviceSpecificType_1350.Barometer_DataPropertyChangedName:
            case DeviceSpecificType_1352.Temperature_DataPropertyChangedName:
            case DeviceSpecificType_1352.Humidity_DataPropertyChangedName:
                uiTemperature.Text = CurrSensor_DataUnits.Temperature.ToString("F2"); // Modify: update the UX as appropriate
                uiHumidity.Text = CurrSensor_DataUnits.Humidity.ToString("F2"); // Modify: update the UX as appropriate
                uiPressure.Text = CurrSensor_DataUnits.Pressure.ToString("F2"); // Modify: update the UX as appropriate

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
        switch (CurrSensorType)
        {
            case TI_SensorTag.SensorType.TI_2541:
                // No battery
                break;
            case TI_SensorTag.SensorType.TI_1350:
                if (name == DeviceSpecificType_1350.BatteryLevelPropertyChangedName || name == "")
                {
                    uiBTConnectionControl.SetBatteryLevel(CurrBattery_DataUnits_1350.BatteryLevel);
                }
                break;
            case TI_SensorTag.SensorType.TI_1352:
                if (name == DeviceSpecificType_1352.Battery_DataPropertyChangedName || name == "")
                {
                    uiBTConnectionControl.SetBatteryLevel(CurrBattery_DataUnits_1352.BatteryLevel);
                }
                break;
        }
    }
    // End of UX changes

    #region Historical Data and export code stays the same

    /// <summary>
    /// Helper code to update historical data. The sensor might send a lot of data; the history only
    /// saves a portion of the data. Technicaly, every time there's new data we either update
    /// the most recent entry OR we add a new entry.
    /// </summary>
    private void UpdateHistoricalDataAndGraph(DeviceSpecificSensorDataFacade currSensor_DataUnits)
    {
        var deltaInSeconds = currSensor_DataUnits.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
        var verb = (deltaInSeconds > HistoricalDataUpdateRateInSeconds)
            ? DataCollection<DeviceSpecificSensorDataFacade>.Verb.Add : DataCollection<DeviceSpecificSensorDataFacade>.Verb.ReplaceMostRecent;
        HistoricalDataUnits.Update(currSensor_DataUnits, verb); // Will add or replace the data and will copy as needed.

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


    /// <summary>
    /// Called from MainWindow when the user asks for, e.g., exported data or graphs. Most sensors will 
    /// support all these options.
    /// </summary>
    public IDeviceControlBasic.UXCapabilities GetUXCapabilities()
    {
        var retval = IDeviceControlBasic.UXCapabilities.CanRename;
        if (HasSensorData)
        {
            retval = retval
            | IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng
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

} // end of class BTTI_SensorTagControl // Modify: update the comment to match the class name
