#region Usings stay the same
using BluetoothProtocols;
using BluetoothProtocolsDevicesCore;
using BluetoothProtocolsDevicesCoreExtensions;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.Beeps;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using BluetoothWinUI3.BTDeviceUnitConverters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.
using System.Threading.Tasks;
using Utilities;
using UtilitiesWinUI3;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif

#endregion
using static BluetoothProtocols.BDM_Aneng_Multimeter;
using DeviceSpecificSensorData = BDM_Aneng_Multimeter.Multimeter_Data; // Modify: 
using DeviceSpecificSensorDataPokitPro = PokitProMeter.Multimeter_Data;

using DeviceSpecificSensorDataUnits = BluetoothProtocolsDevicesCoreExtensions.BdmMultimeterDisplayFacade; // Modify: Often the same as the DeviceSpecificSensorData

using DeviceSpecificType = BDM_Aneng_Multimeter; // Modify: pick your device, not BTSimple_Demo
using DeviceSpecificTypePokitPro = BluetoothProtocols.PokitProMeter;
using DeviceSpecificSensorSecondaryDataPokit = PokitProMeter.Service_Status_Data; // Modify: pick secondary sensor if needed


// using DeviceSpecificBatteryData = TI_SensorTag_1352.Battery_Data; // Modify: many device support battery
// The DeviceSpecificDataUnits should be set either to the DeviceSpecificSensorData or to a data facade.
// Use the DeviceSpecificSensorData if it has the data you want to the user to see
// Use the facade when you need to merge multiple sensor data values together
// or when the original data is "messy"
// About half of all devices require a facade
// Not using a facade? Then the line below is what you want (but modify it!)
//using DeviceSpecificSensorDataUnits = TI_SensorTag_1352.Temperature_Data; // Modify: Often the same as the DeviceSpecificSensorData

// Modify these to match your device


[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTBdm_Aneng_Multimeter : UserControl, IDeviceControlBasic, IDeviceControlDevice // Modify: rename to match your device
{
    // Modify these settings to match your device
    IMultimeterProtocolParser Parser = null; //  new BdmMultimeterProtocol();

    enum MultimeterType {  Bdm, FortuneFS9952, PokitPro };
    MultimeterType CurrMultimeterType = MultimeterType.Bdm;
    private bool SetParser(byte[] data)
    {
        if (Parser != null) return true;
        if (FortuneFS9952MultimeterProtocol.Matches(data))
        {
            CurrMultimeterType = MultimeterType.FortuneFS9952;
            Parser = new FortuneFS9952MultimeterProtocol();
            return true;
        }
        if (BdmMultimeterProtocol.Matches(data))
        {
            CurrMultimeterType = MultimeterType.Bdm;
            Parser = new BdmMultimeterProtocol();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "Multimeter"; // Modify: edit the string to match your device. The exact name does not matter.

    /// <summary>
    /// Tags for the device. This is used to categorize the different devices.
    /// Common tags: environment exersise health cooking agriculture light testequipment report utility other
    /// Other tags: cycling heartrate pulseoximeter security entertainment productivity multimeter oscilloscope
    /// Full list in tagToResourceKeyMap AppDictionaryFromTag.cs
    /// </summary>
    public string Tags { get { return "#multimeter #testequipment"; } }

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
    const double HistoricalDataUpdateRateInSeconds = 0.2; // Update really quickly for the multimeter

    public BTBdm_Aneng_Multimeter() // Modify: edit the name to match the class name
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

    DeviceSpecificType DeviceBdmFamily;
    DeviceSpecificTypePokitPro DevicePokitPro;
        
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;
    ulong OriginalBTAddr = 0xFFFFFFFF_FFFFFFFF;

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the collection.
    /// </summary>
    public DataCollection<DeviceSpecificSensorDataUnits> HistoricalDataUnits { get; } = new();
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
    DeviceSpecificSensorData CurrSensor_Data = null;
    DeviceSpecificSensorDataPokitPro CurrSensor_DataPokitPro = null;
    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    DeviceSpecificSensorDataUnits CurrSensor_DataUnits = null;

    /// <summary>
    /// Making a battery value that's seperate from the Sensor. This lets the programmer
    /// copy-paste data, pick a new sensor, and the battery stuff will still work.
    /// </summary>
    //DeviceSpecificType.Battery_Data CurrBattery_Data = null;
    /// <summary>
    /// Just like CurrBattery_Data but in user-preferred units. For battery, the units
    /// are actually OK as is
    /// </summary>
    //DeviceSpecificType.Battery_Data CurrBattery_DataUnits = null;

    /// <summary>
    /// Data directly from the device. It's always in the original units from the device
    /// and isn't converted into the user's preferred units.
    /// </summary>
    DeviceSpecificSensorSecondaryDataPokit CurrSensorSecondary_DataPokit = null;

    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    // DeviceSpecificSensorSecondaryData CurrSensorSecondary_DataUnits = null;
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
        InitializeUX();
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
            ( DeviceSpecificType.MultimeterDataFFF1PropertyChangedName, uiValueChange),
            ( DeviceSpecificType.MultimeterDataFFF4PropertyChangedName, uiValueChange),
            ( DeviceSpecificTypePokitPro.MM_DataPropertyChangedName, uiValueChange),
        };

        // Modify: set up the graph by making an OxyPlotModel and adding lines to it.
        // The line data must exist in the HistoricalData
        OxyPlotModel = OxyPlotUtilities.MakeOxyPlotModel("Multimeter Data")
            .AddLineNoDefault("Value", "MultimeterValue")
            ;
        OxyPlotModel.Title = null;
        HistoricalDataUnits.MaxDataCount = 100; // turns the display into classic scrolling display

        // "Sensor Data" is for the main graph title  and is human-readable
        // "Ambient Temperature" for the axis title and for the color settings in the menus and should be concise and human-readable
        // "Temperature" is the underlying sensor property name and must exactly match the C# name.

        // Your sensor might include properties that aren't interesting to see in the table view.
        // Note that this table is the visible table; the exported data is set differently.
        // Examples:
        // In this demo, the Facade only includes columns to show
        CurrTableCustomization.TableColumnsToExclude.Add("HasPressure");
        CurrTableCustomization.TableColumnsToExclude.Add("Pressure");

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

        if (DataContextAsKnownDevice.Advertisement.BestName.Contains("PokitPro"))
        {
            CurrMultimeterType = MultimeterType.PokitPro;
            DevicePokitPro = new DeviceSpecificTypePokitPro()
            {
                ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
            };
            if (DevicePokitPro.ble == null)
            {
                // ConnectError:NoBLE
                Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
                CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
                return;
            }
            // It's critical to set these!
            DataContextAsKnownDevice.Id = DevicePokitPro.ble.DeviceId ?? ""; // never null :-)
            DataContextAsKnownDevice.BTLEDevice = DevicePokitPro.ble;
        }
        else
        {
            DeviceBdmFamily = new DeviceSpecificType()
            {
                ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
            };
            if (DeviceBdmFamily.ble == null)
            {
                // ConnectError:NoBLE
                Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
                CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
                return;
            }
            // It's critical to set these!
            DataContextAsKnownDevice.Id = DeviceBdmFamily.ble.DeviceId ?? ""; // never null :-)
            DataContextAsKnownDevice.BTLEDevice = DeviceBdmFamily.ble;
        }

        CurrSaveData = AllSaveData.SwitchToDeviceIdCurrSaveData(CurrSaveData, DataContextAsKnownDevice);

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        UtilitiesWinUI3.UtilitiesWinUI3.InitializeKeyLineColorsFromDefaultOxyPlot(OxyPlotModel, rootPanel);
        UpdateUX(CurrSaveData); // Can be null when the user hasn't made any changes
        KnownDeviceName = DataContextAsKnownDevice.Advertisement?.BestName ?? KnownDeviceName;
        uiKnownDeviceName.Text = KnownDeviceName;

        switch (CurrMultimeterType)
        {
            case MultimeterType.Bdm:
            case MultimeterType.FortuneFS9952:
                DeviceBdmFamily.PropertyChanged += Device_PropertyChanged;
                DeviceBdmFamily.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
                DeviceBdmFamily.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged;
                break;
            case MultimeterType.PokitPro:
                DevicePokitPro.PropertyChanged += Device_PropertyChanged;
                DevicePokitPro.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
                DevicePokitPro.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged;
                break;
        }

        bool connectAllOk = true;
        uiBTConnectionControl.CurrState = BTConnectionControl.ConnectionState.Connecting;
        #endregion

        // Modify so the device starts sending notifications for changed properties (data)

        // Modify: tell the device to start sending sensor and battery data back.
        // Only one of these will actually work
        switch (CurrMultimeterType)
        {
            case MultimeterType.Bdm:
            case MultimeterType.FortuneFS9952:
                await DeviceBdmFamily.NotifyMultimeterDataFFF1Async(); // Bdm style
                await DeviceBdmFamily.NotifyMultimeterDataFFF4Async(); // Fortune FS9952 style
                break;
            case MultimeterType.PokitPro:
                await DevicePokitPro.NotifyMM_DataAsync();
                await DevicePokitPro.NotifyStatus_StatusAsync();
                var ppsdata = await DevicePokitPro.ReadStatus_Status();
                Log($"Pokit: Starting: mode={ppsdata.DeviceStatus} voltage={ppsdata.BatteryVoltage:F2}");
                DevicePokitPro.CurrMultimeter_Data.ModeSetting = (double)PokitPro_Meter_Multimeter_Extensions.MM_ModeType.Idle; // always start with Idle
                DevicePokitPro.CurrMultimeter_Data.RangeSetting = 255; // always auto-range!
                DevicePokitPro.CurrMultimeter_Data.IntervalSetting = 200;
                var ppbytes = DevicePokitPro.CurrMultimeter_Data.GetMM_Setting_Bytes();
                var mmsetting = await DevicePokitPro.WriteMM_Settings(ppbytes); // 5=ohms 0=rabge 200 0 0 0=200 mseconds
                SetVisible(uidIdle, true); // allow override because we really want to be in idle mode
                IsIdle = true;
                break;
        }


#if YOUR_CODE_MIGHT_NEED_TO_QUERY_DEVICE
        var sensordata = await Device.Read__SENSOR_DATA__(DefaultCacheMode);
        if (sensordata == null)
        {
            // Happens in the Demo code when the device doesn't report a battery level (e.g., JBL Pro 4 Speakers,
            // but lots of others). Usually sensordata is always present.
            RemoveSensorDataUx();
        }
#endif


        // TODO: no battery at all?
        // The MEET MS-BDTP1A with device name MS-BO47Q1767 says it has a battery
        // but it just pops out a protocol error???
        uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);

        // Some UX needs additional information
        if (DeviceBdmFamily != null)
        {
            connectAllOk = connectAllOk && (await DeviceBdmFamily.ReadDevice_Name(DefaultCacheMode)) != null;
            CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, DeviceBdmFamily.ble.ConnectionStatus);
        }
        if (DevicePokitPro != null)
        {
            connectAllOk = connectAllOk && (await DevicePokitPro.ReadDevice_Name(DefaultCacheMode)) != null;
            CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, DevicePokitPro.ble.ConnectionStatus);
        }

        var beepLevel = UserPreferences.SoundLevelAdjusted(CurrUserPrefs.BeepLevel, CurrSaveData.Preferences.BeepLevel);
        Beeper.Beep(beepLevel, Beeper.Connect);


        // How this works: when you call the Read call, in addition to returning data it will
        // also call the Device.PropertyChanged (INPC) callback. In my code, it's handy to just
        // have all the UX update code for handling changes in the same place, so I just
        // ignore the return value here.

        // The system tracks device changes
        // Can't do this earlier; merely calling FromBluetoothAddressAsync doesn't actually 
        // connect. Once we do the notify and reads the device will be connected or not.
        // Now above: CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, Device.ble.ConnectionStatus);
    }


#if YOUR_CODE_MIGHT_NEED_THIS
    /// <summary>
    /// Called from DataContextChanged when a device does not, in fact, have a sensor. This 
    /// removes the grpah and table from the display (no sensor means no data) and tells
    /// the MainWindow to update its UX accordingly.
    /// </summary>
    private void RemoveSensorDataUx()
    {
        uiDeviceDataList.Items.Remove(ui__Item_To_Remove__);
        LineNames.Clear();
        uiOxyPlot.Visibility = Visibility.Collapsed;
        uiTableView.Visibility = Visibility.Visible;

        // Notify MainWindow that the UX capabilities have changed. This might change
        // the UX (e.g., device> show graph/table might be removed)
        // Will also trigger redoing the graph line names via LineNames, which
        // technically isn't quite in accordance with the name.
        NotifyDeviceControlChangesWindows?.OnGetUXCapabilitiesChanged(this, GetUXCapabilities());
    }
#endif

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

        // Pick the right Engineering Blue
        const uint EngineeringBlueLight = 0xFF0066CC;
        const uint EngineeringBlueDark = 0xFF5599FF;
        uint engineeringBlue = Application.Current.RequestedTheme == ApplicationTheme.Dark ? EngineeringBlueDark : EngineeringBlueLight;
        var colors = saveData.GetDeviceColors(Application.Current.RequestedTheme);
        var brushes = new DeviceColorBrushes(colors, saveData.Preferences, engineeringBlue); 
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
    /// Set the initial user preferences. This can be called before the UX is set up (and is needed when
    /// we start to connect to set the right beep level)
    /// </summary>
    public void SetInitialUserPreferences(UserPreferences newPrefs, UserPreferences oldPrefs)
    {
        CurrUserPrefs = newPrefs;
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
                // data.Temperature = BluetoothWatcher.Units.Temperature.Convert(data.Temperature, oldPrefs.Temperature, CurrUserPrefs.Temperature);
            }
            if (oldPrefs != null && newPrefs.Pressure != oldPrefs.Pressure)
            {
                // Modify: based on your knowledge of the sensor data, edit the pressure readings.
                // data.Pressure = BluetoothWatcher.Units.Pressure.Convert(data.Pressure, oldPrefs.Pressure, CurrUserPrefs.Pressure);
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
        UIThreadHelper.CallOnUIThread(async () =>
        {
            if (!IsLoaded) return;
            CurrSaveData?.History.UpdateDataHistory(DateTimeOffset.Now);
            UpdateDeviceDataUX(e.PropertyName);


            // If it's more than 10 minutes since the last mode check, do a mode reading
            // Only applies to the Pokit Pro device
            var now = DateTimeOffset.Now;
            if (DevicePokitPro != null && now.Subtract(TimeOfLastModeCheck).TotalMinutes >= 10.0)
            {
                TimeOfLastModeCheck = now;
                await DevicePokitPro.ReadStatus_Status();
            }
        });
    }
    #endregion

    bool IsPaused = false;
    private void OnPause(object sender, RoutedEventArgs e)
    {
        IsPaused = (sender as ToggleButton).IsChecked ?? false;
    }

    /// <summary>
    /// The user has two ways to pause the display: via the DMM Blue "HOLD" button and via
    /// the bottom Pause toggle. The Hold button always determines what gets sent, so we 
    /// should follow it when it changes. When the user click 'pause' on the app, we do the 
    /// same thing as a 'hold' 
    /// </summary>
    private void SetPauseToMatchHold(bool hasHold)
    {
        if (HistoricalDataUnits.Count == 0)
        {
            // Very first item; let's get it all synced. The DMM will win.
            uiPauseButton.IsChecked = hasHold; // will set the IsPaused...
        }
        else if (hasHold != OldHold)
        {
            // DMM changed; sync our pause button to that but only when it had
            // been checked. I don't want the pause button to always exactly
            // reflect the Hold button.
            if (uiPauseButton.IsChecked ?? false)
            {
                uiPauseButton.IsChecked = hasHold;
            }
            // IsPaused = hasHold; // shouldn't be needed -- set by OnPause()
        }
        else if (hasHold)
        {
            ; // do nothing?
        }

        OldHold = hasHold;
    }

    bool OldHold = false;
    bool IsIdle = false;
    DateTimeOffset TimeOfLastModeCheck = DateTimeOffset.MinValue;
    DateTimeOffset TimeOfLastContinuityBeep = DateTimeOffset.MinValue;
    const double CONTINUITY_BEEP_WAIT_IN_SECONDS = 1.0;
    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated. Most often called from Device_PropertyChanged
    /// </summary>
    private void UpdateDeviceDataUX(string name)
    {
        bool haveDevice = DeviceBdmFamily != null || DevicePokitPro != null;
        if (!haveDevice) return;


        switch (CurrMultimeterType)
        {
            case MultimeterType.Bdm:
            case MultimeterType.FortuneFS9952:
                // Modify: Always update these even though in practice they are only set once.
                CurrSensor_Data = DeviceBdmFamily.CurrMultimeter_Data; // Modify: select the right data
                                                                       // CurrSensorSecondary_Data = Device.CurrHumidity_Data; // Modify: pick secondary data as appropriate
                                                                       // CurrBattery_Data = Device.CurrBattery_Data; // Modify: if your device doesn't have a battery, remove battery stuff!
                var data = CurrSensor_Data.DataFFF1;
                if (data == null) data = CurrSensor_Data.DataFFF4;
                if (data == null) return; // not real data

                SetParser(data); // Parser is passed into CopyToWithConvertAndCreate
                if (Parser == null) return; // Could not get a match
                break;

            case MultimeterType.PokitPro:
                CurrSensor_DataPokitPro = DevicePokitPro.CurrMultimeter_Data;
                CurrSensorSecondary_DataPokit = DevicePokitPro.CurrService_Status_Data;

                // Special case: the PokitPro will return a micro-ohm value when it should report zero
                switch ((PokitPro_Meter_Multimeter_Extensions.MM_ModeType)CurrSensor_DataPokitPro.OperationMode)
                {
                    case PokitPro_Meter_Multimeter_Extensions.MM_ModeType.Continuity:
                        if (CurrSensor_DataPokitPro.Status == 0)
                        {
                            CurrSensor_DataPokitPro.Data = 0; // No Continuity
                        }
                        else
                        {
                            CurrSensor_DataPokitPro.Data = 1.0; //  Continuity
                        }
                        break;
                    case PokitPro_Meter_Multimeter_Extensions.MM_ModeType.Resistance:
                        if (CurrSensor_DataPokitPro.Data < 0.1)
                        {
                            // TODO: double check actual documentation
                            // It can't actually read value this low
                            CurrSensor_DataPokitPro.Data = 0;
                        }
                        break;
                }
                break;
        }


        SparklesHelper.UpdateSparkles(ControlsWithSparkles, name); // name is from e.PropertyName when the Device does a PropertyChanged.

        // Update data from the device to match the current preferred units. Will create the values as needed.
        // Modify: the CopyToWithConvertAndCreate as made by the autogen system will handle only a
        // single data type. If you make a Facade class to combine multiple sensors into one, you will
        // need to create a version of CopyToWithConvertAndCreate that takes in both types.
        // CurrSensorSecondary_DataUnits = DeviceSpecificSensorSecondaryData.CopyToWithConvertAndCreate(CurrSensorSecondary_Data, CurrSensorSecondary_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
        // CurrBattery_DataUnits = DeviceSpecificBatteryData.CopyToWithConvertAndCreate(CurrBattery_Data, CurrBattery_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

        // Modify: Edit all this code to match your device and UX.
        bool updateDisplay = false;
        string other ="";
        bool hasHold = false;
        switch (name)
        {
            // In your code, hook up the right sensors to the right XAML
            case DeviceSpecificType.MultimeterDataFFF1PropertyChangedName:
            case DeviceSpecificType.MultimeterDataFFF4PropertyChangedName: // UX is more complex than just matching the parsed state
                CurrSensor_DataUnits = DeviceSpecificSensorDataUnits.CopyToWithConvertAndCreate(CurrSensor_Data, Parser, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
                other = CurrSensor_DataUnits.DisplayOther;
                var hasAuto = other.Contains(BdmMultimeterDisplayFacade.AutoStr); // "AUTO "
                hasHold = other.Contains(BdmMultimeterDisplayFacade.HoldStr); // "Hold "
                other = other.Replace(BdmMultimeterDisplayFacade.AutoStr, ""); // "AUTO ", ""); // Auto isn't useful
                updateDisplay = true;
                break;

            case DeviceSpecificTypePokitPro.MM_DataPropertyChangedName:
                CurrSensor_DataUnits = DeviceSpecificSensorDataUnits.CopyToWithConvertAndCreate(CurrSensor_DataPokitPro, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
                other = CurrSensor_DataUnits.DisplayOther;
                updateDisplay = true;

                // If we had one of the 'waiting' messages shown, take them down
                if (IsIdle)
                {
                    return;
                }
                else
                {
                    // Note that we never actually get an 'idle' message
                    uidIdle.Visibility = Visibility.Collapsed;
                    uidSwitch.Visibility = Visibility.Collapsed;
                }
                break;

            case DeviceSpecificTypePokitPro.Status_StatusPropertyChangedName: // mode and battery voltage
                Log($"Pokit:Status_Status:time,{DateTime.Now.ToString("HH:mm")},mode,{CurrSensorSecondary_DataPokit.DeviceStatus},voltage,{CurrSensorSecondary_DataPokit.BatteryVoltage:F2}");
                if (CurrSensorSecondary_DataPokit.DeviceStatus == (double)PokitPro_Meter_Multimeter_Extensions.MM_ModeType.Idle)
                {
                    // The device has decided to switch to idle mode. That's OK, but update the screen accordingly
                    // Do not override the Error
                    SetVisible(uidIdle, false);
                }
                break;

            default:
                Log($"MM: Other change {name}");
                break;


                // Secondary sensor is shown in the UX but isn't in the history data.
        }
        if (updateDisplay)
        {
            if (!IsPaused)
            {
                uiValue.Text = CurrSensor_DataUnits.DisplayText; // which to use? Answer: what the DMM says! .DisplayNumber.ToString("F3");
                uiUnits.Text = CurrSensor_DataUnits.DisplayUnits;
                uiOther.Text = other;
                uiMeasurementType.Text = CurrSensor_DataUnits.ModeStr;
                OxyPlotModel.Axes[1].Title = $"{CurrSensor_DataUnits.ModeStr}";

                // Beep or speak as appropriate
                if (CurrSensor_DataUnits.Mode == DeviceSpecificSensorDataUnits.DmmMode.Continuity)
                {
                    if (CurrSensor_DataUnits.MultimeterValue == 1.0) // 0=no continuity 1=continuity
                    {
                        var now = DateTimeOffset.Now;
                        if (now.Subtract(TimeOfLastContinuityBeep).TotalSeconds >= CONTINUITY_BEEP_WAIT_IN_SECONDS)
                        {
                            TimeOfLastContinuityBeep = now;
                            var beepLevel = UserPreferences.SoundLevelAdjusted(CurrUserPrefs.BeepLevel, CurrSaveData.Preferences.BeepLevel);
                            Beeper.BeepIfNotBeeping(beepLevel, Beeper.Continuity);
                        }
                    }
                    else
                    {
                        TimeOfLastContinuityBeep = DateTimeOffset.MinValue; // after losing continuity, the next get should beep
                    }
                }
                else
                {
                    var voiceLevel = UserPreferences.SoundLevelAdjusted(CurrUserPrefs.VoiceLevel, CurrSaveData.Preferences.VoiceLevel);
                    GlobalSpeaker.Speak(voiceLevel, CurrSensor_DataUnits.DisplayText.ToString());
                }
            }

            if (IsPaused && !hasHold)
            {
                // Set up the other to show the pause
                other = other + "Pause ";
            }
            SetPauseToMatchHold(hasHold);

            // Only the sensor data gets plotted as historical data. In the demo,
            // other values are also read (e.g., the Interval_Min), but they aren't
            // part of the sensor data that's plotted.
            // The historical data is updated from the CurrSensor_DataUnits
            if (!hasHold && !IsPaused)
            {
                // On hold, freeze the display
                UpdateHistoricalDataAndGraph(CurrSensor_DataUnits);
            }


        }

        //
        // Many devices include a battery level. If so, chances are it's called "BatteryLevel" or "Battery_Data"
        // 
        //
        //TODO:         if (CurrBattery_DataUnits != null)
        //        {
        //            if (name == DeviceSpecificType.Battery_DataPropertyChangedName || name == "")
        //            {
        //                uiBTConnectionControl.SetBatteryLevel(CurrBattery_DataUnits.BatteryLevel);
        //            }
        //        }
    }

    // End of UX changes

    #region Historical Data and export code stays the same

    /// <summary>
    /// Helper code to update historical data. The sensor might send a lot of data; the history only
    /// saves a portion of the data. Technicaly, every time there's new data we either update
    /// the most recent entry OR we add a new entry.
    /// </summary>
    private void UpdateHistoricalDataAndGraph(DeviceSpecificSensorDataUnits currSensor_DataUnits)
    {
        var deltaInSeconds = currSensor_DataUnits.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
        var verb = (deltaInSeconds > HistoricalDataUpdateRateInSeconds)
            ? DataCollection<DeviceSpecificSensorDataUnits>.Verb.Add : DataCollection<DeviceSpecificSensorDataUnits>.Verb.ReplaceMostRecent;
        HistoricalDataUnits.Update(currSensor_DataUnits, verb); // Will add or replace the data and will copy as needed.

        // Remove old data. Max time is 5 seconds.

        const long TIME_CUTOFF_IN_SECONDS = 5;
        HistoricalDataUnits.RemoveOlderThanSeconds(TIME_CUTOFF_IN_SECONDS);

        //
        // Update the OxyPlot because it doesn't track the INotifyCollectionChanged
        //
        if (verb == DataCollection<DeviceSpecificSensorDataUnits>.Verb.Add && HistoricalDataUnits.Count == 2)
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
            | IDeviceControlBasic.UXCapabilities.CanBeepAndVoice
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

    private async void OnRangeMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (DevicePokitPro == null) return;
        var rangestr = (sender as FrameworkElement)?.Tag as string;
        if (string.IsNullOrEmpty(rangestr)) return; // will never happen
        byte range = 0;
        var byteparse = Byte.TryParse(rangestr, out range);
        if (!byteparse) return;
        byte[] mmsetting = [5, (byte)range, 200, 0, 0, 0]; // 5=ohms 0=range 200 0 0 0=200 mseconds
        var result = await DevicePokitPro.WriteMM_Settings(mmsetting);
        ;
    }

    private async void OnSetupPokitPro(object sender, RoutedEventArgs e)
    {
        if (DevicePokitPro == null) return;

        // Set the mode
        var mode = DevicePokitPro.CurrMultimeter_Data.ModeSetting;
        var modestr = mode.ToString();
        foreach (var item in uidMMSetupPokitPro_Mode.Children)
        {
            var rb = item as RadioButton;
            if (rb == null) continue;
            var tag = rb.Tag as string;
            if (string.IsNullOrEmpty (tag)) continue;

            var match = tag == modestr;
            rb.IsChecked = match ? true : false;
        }

        //
        // Show the dialog box
        //
        var result = await uidMMSetupPokitPro.ShowAsync();

        // Update the mode!
        foreach (var item in uidMMSetupPokitPro_Mode.Children)
        {
            var rb = item as RadioButton;
            if (rb == null) continue;

            if (rb.IsChecked.Value) modestr = rb.Tag as string;
        }

        byte newPokitProMode=0;
        var parseOk = byte.TryParse(modestr, out newPokitProMode);
        if (!parseOk) return;

        var oldMode = DevicePokitPro.CurrMultimeter_Data.ModeSetting;
        var oldRange = DevicePokitPro.CurrMultimeter_Data.RangeSetting;

        DevicePokitPro.CurrMultimeter_Data.ModeSetting = newPokitProMode;
        var settingsBytes = DevicePokitPro.CurrMultimeter_Data.GetMM_Setting_Bytes();
        var setOk = await DevicePokitPro.WriteMM_Settings(settingsBytes, (GattCommunicationStatus)999);
        if (setOk == (GattCommunicationStatus)999)
        {
            // Threw an exception. Technically this could be for a lot of reasons. But the #1 reason
            // by a mile is that the switch isn't set correctly. If so, reset to the old mode.
            Log("Pokit: Invalid request to change mode; set to Idle instead.");
            uidSwitchError.Visibility = Visibility.Visible;
            DevicePokitPro.CurrMultimeter_Data.ModeSetting = (double)PokitPro_Meter_Multimeter_Extensions.MM_ModeType.Idle; // and not oldMode
            DevicePokitPro.CurrMultimeter_Data.RangeSetting = oldRange;
            SetVisible(uidSwitchError);

            settingsBytes = DevicePokitPro.CurrMultimeter_Data.GetMM_Setting_Bytes();
            var resetOk = await DevicePokitPro.WriteMM_Settings(settingsBytes, (GattCommunicationStatus)999);
        }
        else if (newPokitProMode == 0) // and not (byte)BdmMultimeterDisplayFacade.DmmMode.Idle)
        {
            SetVisible(uidIdle, true); // user explicitly requested the idle mode, so show it
            IsIdle = true;
        }
        else
        {
            SetVisible(uidSwitch);
            IsIdle = false;
        }

        var ppsdata = await DevicePokitPro.ReadStatus_Status();
        Log($"Pokit: Set Mode result: mode={ppsdata.DeviceStatus} voltage={ppsdata.BatteryVoltage:F2}");

    }

#if NEVER_EVER_DEFINED
/// It turns out that the Pokit Pro doesn't expose the range settings in the app. It's always on auto-range!
    private void OnMMPokitSetupClick(object sender, RoutedEventArgs e)
    {
        var tag = (sender as RadioButton)?.Tag as string;
        if (tag == null) return; // can never happen
        int mode;
        if (!int.TryParse(tag, out mode)) return; // can never happen
        var combo = uidMMSetupPokitRange;
        combo.Items.Clear();
        int currRange = (int)CurrSensor_DataPokitPro.OperationMode;
        switch (mode)
        {
            case 1:
            case 2: // volts
                combo.IsEnabled = true;
                combo.Items.Add(new ComboBoxItem() { Content = "Autorange", Tag = "255", IsSelected=(currRange==255) });
                break;

            case 3:
            case 4: // amps
                combo.IsEnabled = true;
                combo.Items.Add(new ComboBoxItem() { Content = "Autorange", Tag = "255", IsSelected = (currRange == 255) });
                break;

            case 5: // ohms
                combo.IsEnabled = true;
                combo.Items.Add(new ComboBoxItem() { Content = "Autorange", Tag = "255", IsSelected = (currRange == 255) });
                combo.Items.Add(new ComboBoxItem() { Content = "  0..160 Ω", Tag = "0", IsSelected = (currRange == 0) });
                combo.Items.Add(new ComboBoxItem() { Content = "160..330 Ω", Tag = "1", IsSelected = (currRange == 1) });
                combo.Items.Add(new ComboBoxItem() { Content = "330..890 Ω", Tag = "2", IsSelected = (currRange == 2) });
                combo.Items.Add(new ComboBoxItem() { Content = "890..1.5 ㏀", Tag = "3", IsSelected = (currRange == 3) });
                combo.Items.Add(new ComboBoxItem() { Content = "1.5..10 ㏀", Tag = "4", IsSelected = (currRange == 4) });
                combo.Items.Add(new ComboBoxItem() { Content = " 10..100 ㏀", Tag = "5", IsSelected = (currRange == 5) });
                combo.Items.Add(new ComboBoxItem() { Content = "100..470 ㏀", Tag = "6", IsSelected = (currRange == 6) });
                combo.Items.Add(new ComboBoxItem() { Content = "470..1000 ㏀", Tag = "7", IsSelected = (currRange == 7) });
                break;


            default:
                combo.IsEnabled = false;
                break;
        }

    }
#endif

    private void SetVisible(FrameworkElement item, bool letIdleOverrideSwitchError = false)
    {
        if (letIdleOverrideSwitchError == false && uidSwitchError.Visibility == Visibility.Visible && item == uidIdle)
        {
            // The user is already looking at the switch error and we have detected Idle mode.
            // Don't switch from the error to the idle screen.
            return;
        }
        uidIdle.Visibility = (item == uidIdle) ? Visibility.Visible : Visibility.Collapsed;
        uidSwitch.Visibility = (item == uidSwitch) ? Visibility.Visible : Visibility.Collapsed;
        uidSwitchError.Visibility = (item == uidSwitchError) ? Visibility.Visible : Visibility.Collapsed;
    }

    private void OnBeep(object sender, RoutedEventArgs e)
    {
        var beepLevel = UserPreferences.SoundLevelAdjusted(CurrUserPrefs.BeepLevel, CurrSaveData.Preferences.BeepLevel);
        Beeper.Beep(beepLevel, Beeper.Connect);
    }
} // end of class BTSimple_DemoControl // Modify: update the comment to match the class name
