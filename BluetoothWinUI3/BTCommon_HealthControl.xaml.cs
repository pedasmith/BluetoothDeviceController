using BluetoothConversions;
using BluetoothProtocols;
using BluetoothProtocolsDevicesCore;
using BluetoothProtocolsDevicesCoreExtensions;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using BluetoothWinUI3.BTDeviceUnitConverters;
using BluetoothWinUI3.Units;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using OxyPlot;
using OxyPlot.Axes;
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


#region Change these to match your device
using DeviceSpecificBatteryData_Choice_MMed = ChoiceMMed_PulseOximeter.Battery_Data; // Change: many device support battery
using DeviceSpecificSensorData = HealthDataRecord; // Change: 

#endregion

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTCommon_HealthControl : UserControl, IDeviceControlBasic, IDeviceControlDevice, IHandleMyBTAdvertisements // Change: change the name from BTStandard_DemoControl
{
    #region Change these settings that must be updated for a new device
    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "HealthSensor"; // Change: change the BTStandard_Demo string to match your device. The exact name does not matter.

    /// <summary>
    /// Tags for the device. This is used to categorize the different devices.
    /// Common tags: environment exersise health cooking agriculture light
    /// </summary>
    public string Tags { get { return "#health"; } }
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

    public BTCommon_HealthControl() // CHANGE: change the name to match the changed class name
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += Control_DataContextChanged;
    }

    #region Instance value for a device (not changed)
    Viatom_PC_60F Device_Viatom = null;
    ChoiceMMed_PulseOximeter Device_MMedPulseOximeter = null;
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
    enum SensorFamily { Unknown, ChoiceMMed_PulseOximeter, Viatom };
    SensorFamily CurrSensorFamily = SensorFamily.Unknown;

    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    DeviceSpecificSensorData CurrSensor_DataUnits = null;
    /// <summary>
    /// Making a battery value that's seperate from the Sensor. This lets the programmer
    /// copy-paste data, pick a new sensor, and the battery stuff will still work.
    /// </summary>
    DeviceSpecificBatteryData_Choice_MMed CurrBattery_Data_Choice_MMed = null;
    /// <summary>
    /// Just like CurrBattery_Data but in user-preferred units. For battery, it
    /// doesn't actually change anything :-)
    /// </summary>
    DeviceSpecificBatteryData_Choice_MMed CurrBattery_DataUnits_Choice_MMed = null;

    enum Vitam_SensorType {  }

    /// <summary>
    /// There are multiple sensors that this one control can handle. They are all initialized to 'NotThisSensorFamily'
    /// </summary>
   
    Viatom.SensorType ViatomSensorType = Viatom.SensorType.NotThisSensorFamily;
    ChoiceMMed_PulseOximeter_Extension.SensorType ChoiceMMedPulseOximeterSensorType = ChoiceMMed_PulseOximeter_Extension.SensorType.NotThisSensorFamily;
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
        // InitializeUX(); // For advertisement-based data, initialize the UX when we get the first data
    }

    private void UpdateForSensor(HealthDataRecord.SensorPresent sensor, int step, int range, string title, string propertyName, StackPanel panel, AxisPosition axisPosition = AxisPosition.Left, string axisKey = null, string axisTitle = null)
    {
        if (CurrSensor_Data.IsSensorPresent.HasFlag(sensor))
        {
            OxyPlotUtilities.AddLine(OxyPlotModel, step, range, title, propertyName, axisPosition: axisPosition, axisKey: axisKey, axisTitle: axisTitle);
            CurrTableCustomization.TableColumns.Add(propertyName);
        }
        else
        {
            uiDeviceDataList.Items.Remove(panel);
        }

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
            ( DeviceSpecificSensorData.PulseRatePropertyChangedName, uiPulseRateChange),
        };

        // Change: set up the graph by making an OxyPlotModel
        OxyPlotModel = OxyPlotUtilities.MakeOxyPlotModel("Health Data");

        // Set up the Connect button and Battery visibility
        uiBTConnectionControl.SetConnectVisibility(Visibility.Visible); // ChoiceMMed and Viatom are connected, not advert.

        switch (CurrSensorFamily)
        {
            default: 
            case SensorFamily.Unknown:
            case SensorFamily.Viatom:
                if (!CurrSensor_Data.IsSensorPresent.HasFlag(HealthDataRecord.SensorPresent.Battery))
                {
                    // TODO: or the device might have a battery service
                    uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
                }
                break;
            case SensorFamily.ChoiceMMed_PulseOximeter:
                // Battery is based on detecting the Battery service
                break;
        }
        // Note: you have to remove the sensor from the uiDeviceDataList entirely. You can't just
        // set it to invisible because the items will still show up
        CurrTableCustomization.TableColumns.Add("Name"); // always show the name

        UpdateForSensor(HealthDataRecord.SensorPresent.PulseRate, 5, 30, "Pulse", "PulseRate", uiDeviceDataPulseRate);
        UpdateForSensor(HealthDataRecord.SensorPresent.OxygenSaturationInPercent, 2, 10, "Oxygen", "OxygenSaturationInPercent", uiDeviceDataOxygenSaturationInPercent);
        UpdateForSensor(HealthDataRecord.SensorPresent.PerfusionIndexInPercent, 2, 10, "Perfusion", "PerfusionIndexInPercent", uiDeviceDataPerfusionIndexInPercent, axisPosition:AxisPosition.Right);
        UpdateForSensor(HealthDataRecord.SensorPresent.RespirationRate, 5, 30, "Respiration (RR)", "RespirationRate", uiDeviceDataRespirationRate, axisPosition: AxisPosition.Right);

        //
        uiOxyPlot.Model = OxyPlotModel;

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        UtilitiesWinUI3.UtilitiesWinUI3.InitializeKeyLineColorsFromDefaultOxyPlot(OxyPlotModel, rootPanel);
        // Advertisement-based devices don't really have a device ID.
        // CurrSaveData = AllSaveData.SwitchToDeviceIdCurrSaveData(CurrSaveData, DataContextAsKnownDevice);

        UpdateUX(CurrSaveData); // Can be null when the user hasn't made any changes
        if (CurrSaveData == null)
        {
            KnownDeviceName = DataContextAsKnownDevice.Advertisement?.BestName ?? KnownDeviceName;
            uiKnownDeviceName.Text = KnownDeviceName;
        }

        // "Sensor Data" is for the main graph title  and is human-readable
        // "Battery" for the axis title and for the color settings in the menus and should be concise and human-readable
        // "BatteryLevel" is the underlying sensor property name and must exactly match the C# name.
#endregion


        // This sarkles, oxyplot, and table code is always the same and doesn't need to be changed.
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
    List<string> _LineNames = new() { };
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
    /// If the device had become disconnected, the control uses this (via BTConnectionControl.GotAnotherAdvertisement)
    /// to trigger a reconnect attempt. GotAnotherAdvertisement is smart and will only reconnect as appropriate.
    /// </summary>
    public async Task HandleMyAdvertisementAsync(WatcherData data)
    {
        await uiBTConnectionControl.GotAnotherAdvertisementAsync();
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

        // What's the correct sensor?
        CurrSensor_Data = new HealthDataRecord()
        {
            IsSensorPresent = HealthDataRecord.SensorPresent.PulseRate | HealthDataRecord.SensorPresent.OxygenSaturationInPercent | HealthDataRecord.SensorPresent.PerfusionIndexInPercent,
        };

        // Initialize sensor types Must happen before we InitializeUX() because that code
        // uses the sensor type to decide what to display
        ViatomSensorType = Viatom.AdvertIsSensorFamily(DataContextAsKnownDevice.Advertisement);
        if (ViatomSensorType != Viatom.SensorType.NotThisSensorFamily) CurrSensorFamily = SensorFamily.Viatom;
        ChoiceMMedPulseOximeterSensorType = ChoiceMMed_PulseOximeter_Extension.AdvertIsSensorFamily(DataContextAsKnownDevice.Advertisement);
        if (ChoiceMMedPulseOximeterSensorType != ChoiceMMed_PulseOximeter_Extension.SensorType.NotThisSensorFamily) CurrSensorFamily = SensorFamily.ChoiceMMed_PulseOximeter;
        switch (CurrSensorFamily)
        {
            default: break;
            case SensorFamily.Unknown: break;
            case SensorFamily.ChoiceMMed_PulseOximeter:
                // Sets the IsSensorPresent value
                ChoiceMMed_PulseOximeter_Extension.SetHealthDataRecordIsSensor(CurrSensor_Data, ChoiceMMedPulseOximeterSensorType);
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

    /// <summary>
    /// Called by e.g., the ConnectionControl when the user wants to reconnect to the device (sensor).
    /// The initial connect is handled by the controls in Control_DataContextChanged() when the
    /// control DataContexts is set
    /// 
    /// Also called by Control_DataContextsChanged for the first connect
    /// </summary>
    public async Task ReconnectAsync()
    {
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

        BluetoothLEDevice ble = null;
        switch (CurrSensorFamily)
        {
            case SensorFamily.Unknown:
                Log($"Health: Error: unknown sensor");
                return;
            case SensorFamily.ChoiceMMed_PulseOximeter:
                ChoiceMMed_PulseOximeter_Extension.SetHealthDataRecordIsSensor(CurrSensor_Data, ChoiceMMedPulseOximeterSensorType);
                ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr);
                Device_MMedPulseOximeter = new ChoiceMMed_PulseOximeter() { ble = ble };
                if (Device_MMedPulseOximeter.ble == null)
                {
                    // ConnectError:NoBLE
                    Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
                    CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
                    return;
                }
                break;
            case SensorFamily.Viatom:
                ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr);
                Device_Viatom = new Viatom_PC_60F() { ble = ble };
                if (Device_Viatom.ble == null)
                {
                    // ConnectError:NoBLE
                    Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
                    CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, BluetoothConnectionStatus.Disconnected);
                    return;
                }
                ViatomFactory = new Viatom_PulseOximeter_PC60FW_Factory();
                break;
        }


        // It's critical to set these!
        DataContextAsKnownDevice.Id = DataContextAsKnownDevice.Advertisement.AddressAsString; //  Device.ble.DeviceId ?? ""; // never null :-)
        DataContextAsKnownDevice.BTLEDevice = ble;
        CurrSaveData = AllSaveData.SwitchToDeviceIdCurrSaveData(CurrSaveData, DataContextAsKnownDevice);

        UtilitiesWinUI3.UtilitiesWinUI3.InitializeKeyLineColorsFromDefaultOxyPlot(OxyPlotModel, rootPanel);
        UpdateUX(CurrSaveData); // Can be null when the user hasn't made any changes
        KnownDeviceName = DataContextAsKnownDevice.Advertisement.BestName;
        uiKnownDeviceName.Text = KnownDeviceName;

        if (Device_MMedPulseOximeter != null)
        {
            Device_MMedPulseOximeter.PropertyChanged += Device_PropertyChanged;
            Device_MMedPulseOximeter.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
            Device_MMedPulseOximeter.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged;
        }
        if (Device_Viatom != null)
        {
            Device_Viatom.PropertyChanged += Device_PropertyChanged;
            Device_Viatom.Status.OnBluetoothStatus += Status_OnBluetoothStatus;
            Device_Viatom.ble.ConnectionStatusChanged += Ble_ConnectionStatusChanged;
        }
        bool connectAllOk = true;
        uiBTConnectionControl.CurrState = BTConnectionControl.ConnectionState.Connecting;
        #region Change so the device starts sending notifications for changed properties (data)

        if (Device_MMedPulseOximeter != null)
        {
            connectAllOk = connectAllOk && await Device_MMedPulseOximeter.NotifyOximeterDataStreamAsync();
            var INDICATE = Windows.Devices.Bluetooth.GenericAttributeProfile.GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            connectAllOk = connectAllOk && await Device_MMedPulseOximeter.NotifyEnablePulseOximeterStreamAsync(INDICATE);

            // Verify that your device has a battery characteristic. If your device does not,
            // just SetBatteryVisibility(Visibility.Collapsed); without further notice.
            var batterydata = connectAllOk ? await Device_MMedPulseOximeter.ReadBattery_Level(DefaultCacheMode) : null;
            if (batterydata == null)
            {
                uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
            }
            connectAllOk = connectAllOk && await Device_MMedPulseOximeter.NotifyBattery_LevelAsync(); 

        }

        if (Device_Viatom != null)
        {
            connectAllOk = connectAllOk && await Device_Viatom.NotifyReceiveAsync();
        }
        #endregion

        // The system tracks device changes
        // Can't do this earlier; merely calling FromBluetoothAddressAsync doesn't actually 
        // connect. Once we do the notify and reads the device will be connected or not.

        var statusMatch = (connectAllOk && ble.ConnectionStatus == BluetoothConnectionStatus.Connected)
            || (!connectAllOk && ble.ConnectionStatus == BluetoothConnectionStatus.Disconnected);
        if (!statusMatch)
        {
            Log($"{KnownDeviceName}: connect is inconsistent: connectAllOk={connectAllOk} but ble={ble.ConnectionStatus}");
        }
        uiBTConnectionControl.SetState(ble.ConnectionStatus);
        CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, ble.ConnectionStatus);

        HandleMyAdvertisement(DataContextAsKnownDevice.Advertisement);
    }


    /// <summary>
    /// Called when the BLE device connection status changes.
    /// </summary>
    private void Ble_ConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        // Choices for ConnectionStatus is just Disconnected and Connected 
        uiBTConnectionControl.SetState(sender.ConnectionStatus);
        UIThreadHelper.CallOnUIThread(() => { Log($"{InternalDeviceType}: Status update: {sender.ConnectionStatus}"); });
        ;
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
    /// Updates the OxyPlit and highlights a given line OR clears the highlight if the
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

    #region Change to update the UX when the device says there's new data
    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated. Most often called from Device_PropertyChanged
    /// </summary>
    private void UpdateDeviceDataUX(string name)
    {
        if (name == Viatom_PC_60F.ReceivePropertyChangedName)
        {
            ViatomFactory.AddNotification(Device_Viatom.CurrTransmit_Data.Receive);
            var next = ViatomFactory.GetNext(CurrSensor_Data);
            if (next == null) return;
            CurrSensor_Data = next;
            name = "*"; // all data is updated
        }
        else if (name == ChoiceMMed_PulseOximeter.OximeterDataStreamPropertyChangedName)
        {
            if (CurrSensor_Data == null)
            {
                ;
            }
            var data = Device_MMedPulseOximeter.CurrTransmitNordic;
            switch (data.Opcode)
            {
                case 0x01: // pulse data
                    uiPleth.Visibility = Visibility.Visible;
                    uiPleth.AddNextPulse(data.PulseData);
                    // Other than updating the uiPleth display, do nothing.
                    // Especially don't update the sparkles; if you do, they just jump around
                    // like a puppy wanting a treat.
                    return;
                case 0x3E: // normal data
                    CurrSensor_Data.OxygenSaturationInPercent = data.OxygenSaturationInPercent;
                    CurrSensor_Data.PerfusionIndexInPercent = data.PerfusionIndexInPercent;
                    CurrSensor_Data.PulseRate = data.PulseRate;
                    CurrSensor_Data.RespirationRate = data.RespirationRate;
                    CurrSensor_Data.TimestampMostRecent = data.TimestampMostRecent;
                    break;
            }

            name = "*"; // all data is updated
        }
        else if (name == ChoiceMMed_PulseOximeter.Battery_LevelPropertyChangedName)
        {
            CurrBattery_Data_Choice_MMed = Device_MMedPulseOximeter.CurrBattery_Data;
        }
        else
        {
            return; // no other data???
        }

        //if (CurrSensor_Data == null) return;
        SparklesHelper.UpdateSparkles(ControlsWithSparkles, name);

        // Update data from the device to match the current preferred units. Will create the values as needed.
        CurrSensor_DataUnits = DeviceSpecificSensorData.CopyToWithConvertAndCreate(CurrSensor_Data, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
        if (CurrBattery_Data_Choice_MMed != null)
        {
            CurrBattery_DataUnits_Choice_MMed = DeviceSpecificBatteryData_Choice_MMed.CopyToWithConvertAndCreate(CurrBattery_Data_Choice_MMed, CurrBattery_DataUnits_Choice_MMed, KnownDeviceName, CurrUserPrefs.Convert);
        }

        if (CurrSensor_DataUnits.PulseRate == 0 && CurrSensor_DataUnits.OxygenSaturationInPercent == 0)
        {
            return; // ignore bogus data; it's not helpful
        }

        // Track the historical data
        switch (name)
        {
            case "*": // All the data changed. This is what always happens with the sensor.
                UpdateHistoricalDataAndGraph(CurrSensor_DataUnits);
                break;
        }



        if (name == HealthDataRecord.PulseRatePropertyChangedName || name == "" || name == "*")
        {
            uiPulseRate.Text = CurrSensor_DataUnits.PulseRate.ToString("F0");
        }
        if (name == HealthDataRecord.OxygenSaturationInPercentPropertyChangedName || name == "" || name == "*")
        {
            uiOxygenSaturationInPercent.Text = CurrSensor_DataUnits.OxygenSaturationInPercent.ToString("0.0") + "%";
        }
        if (name == HealthDataRecord.PerfusionIndexInPercentPropertyChangedName || name == "" || name == "*")
        {
            uiPerfusionIndexInPercent.Text = CurrSensor_DataUnits.PerfusionIndexInPercent.ToString("F0") + "%";
        }
        if (name == HealthDataRecord.RespirationRatePropertyChangedName || name == "" || name == "*")
        {
            uiRespirationRate.Text = CurrSensor_DataUnits.RespirationRate.ToString("F0");
        }

        if (name == HealthDataRecord.BatteryPropertyChangedName || name == "" || name == "*")
        {
            if (CurrSensor_DataUnits.BatteryInPercent != 0)
            {
                uiBTConnectionControl.SetBatteryLevel(CurrSensor_DataUnits.BatteryInPercent);
            }
        }

        //
        // Many devices include a battery level. If so, chances are it's called "BatteryLevel"
        // 
        //
        if (CurrBattery_DataUnits_Choice_MMed != null)
        {
            if (name == ChoiceMMed_PulseOximeter.Battery_LevelPropertyChangedName || name == "")
            {
                uiBTConnectionControl.SetBatteryLevel(CurrBattery_DataUnits_Choice_MMed.BatteryLevel);
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

        // If we're very far behind, skip updating the graph
        var updateAgeInMinutes = DateTimeOffset.Now.Subtract(currSensor_DataUnits.TimestampMostRecent).TotalMinutes;
        if (updateAgeInMinutes < 10)
        {
            uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines 
        }
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

    Viatom_PulseOximeter_PC60FW_Factory ViatomFactory = null;
    private void HandleMyAdvertisementOnUIThread(WatcherData data)
    {
        // It's critical that this be a seperate function, not part of the HandleMyAdvertisement .. UIThreadHelper.CallOnUIThread
        // lambda. If it's part of the lambda, Visual Studio (as of 2026-07-31) has the most truly
        // attrocious gray gridded background that offends me to my very soul.

        if (!IsLoaded) return; // Won't be loaded when we exit the app!

        switch (CurrSensorFamily)
        {
            case SensorFamily.ChoiceMMed_PulseOximeter:
            case SensorFamily.Viatom:
                return; // ChoiceMMed and viatom PC60FW doesn't use adverts for data.
        }
        ;

#if NEVER_EVER_DEFINED
        if (CurrSensor_Data == null)
        {
            // Lots of reasons it might be invalid. For example, we get an advert that includes a 
            // name (and creates this control), but the advert doesn't include the data because
            // we haven't gotten the BT advertisement response yet.
            Log($"ERROR: unable to parse sensor data for sensor type {CurrSensorFamily}");
            return;
        }
        var copyable = CurrSensor_Data as HealthDataRecordCopyable;
        if (copyable != null && !copyable.IsValid)
        {
            // Lots of reasons it might be invalid. For example, we get an advert that includes a 
            // name (and creates this control), but the advert doesn't include the data because
            // we haven't gotten the BT advertisement response yet.
            if (!copyable.IsIgnored)
            {
                // The Ruuvi Air sends an enormous number of unusable advertisements to
                // support backwards compatibility.
                Log($"ERROR: unable to parse IsValid sensor data for sensor type {CurrSensorFamily}");
            }
            return;
        }

        InitializeUX(); // Will initialize the UX as appropriate
        CurrSaveData.History.UpdateAdvertisementHistory(data.MostRecentAdvertisement.Timestamp);
        CurrSaveData.History.UpdateDataHistory(data.MostRecentAdvertisement.Timestamp);
        if (!string.IsNullOrEmpty(data.BestName))
        {
            // RuuviTag Eddystone don't include a Name in their advertisement.
            CurrSensor_Data.Name = data.BestName;
        }
        CurrSensor_Data.TimestampMostRecent = data.MostRecentAdvertisement.Timestamp;
        //UpdateDeviceDataUX("*"); // Update all the data!

        // here!here: tell the window about line names
        if (!CalledOnGetUXCapabilities)
        {
            CalledOnGetUXCapabilities = true;
            NotifyDeviceControlChangesWindows.OnGetUXCapabilitiesChanged(this, this.GetUXCapabilities());
        }
#endif
    }


    /// <summary>
    /// Called by MainWindow / Advertisement Watcher when a new advertisement from the specific (known)
    /// device is seen.
    /// </summary>
    /// <param name="data"></param>
    public void HandleMyAdvertisement(WatcherData data)
    {
        UIThreadHelper.CallOnUIThread(() => HandleMyAdvertisementOnUIThread(data));
    }

    // bool CalledOnGetUXCapabilities = false;




    public string GetDetails(IDeviceControlBasic.DetailsType detailsType)
    {
        return "Internal error: no details are available";
    }
#endregion
} // end of class BTCommon_HealthControl // CHANGE: update the comment to match the class name
