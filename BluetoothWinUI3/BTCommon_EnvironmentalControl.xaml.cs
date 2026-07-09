using BluetoothProtocols;
using BluetoothProtocolsDevicesCore;
using BluetoothWatcher.AdvertismentWatcher;
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
using DeviceSpecificSensorData = SensorDataRecord; // Change: 
#endregion

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTCommon_EnvironmentalControl : UserControl, IDeviceControlBasic, IDeviceControlDevice, IHandleMyBTAdvertisements // Change: change the name from BTStandard_DemoControl
{
    #region Change these settings that must be updated for a new device
    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "EnvironmentSensor"; // Change: change the BTStandard_Demo string to match your device. The exact name does not matter.
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
    /// Every use case might have a different point of view about how frequently to update
    /// the historical data (the data displayed in the graph + shown in the table view
    /// + exported). A good default is 5 seconds.
    /// </summary>
    const double HistoricalDataUpdateRateInSeconds = 5.0;
    #endregion

    public BTCommon_EnvironmentalControl() // CHANGE: change the name to match the changed class name
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += Control_DataContextChanged;
    }

    #region Instance value for a device (not changed)
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;

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
    public IBTCommonMetaData GetDataMostRecent() // TODO: add this to the data collections!
    {
        return HistoricalDataUnits.Count == 0 ? null : HistoricalDataUnits.Data[HistoricalDataUnits.Count - 1];
    }

    /// <summary>
    /// Current sensor data from the Device. For the demo, it's battery level.
    /// </summary>
    DeviceSpecificSensorData CurrSensor_Data = null;
    enum SensorFamily { Govee, SensorPro, ThermPro };
    SensorFamily CurrSensorFamily = SensorFamily.Govee;

    /// <summary>
    /// Similar to Curr...Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    DeviceSpecificSensorData CurrSensor_DataUnits = null;


    /// <summary>
    /// There are multiple sensors that this one control can handle. They are all initialized to 'NotThisSensorFamily'
    /// </summary>
    Govee.SensorType GoveeSensorType = Govee.SensorType.NotThisSensorFamily;
    SensorPro.SensorType SensorProSensorType = SensorPro.SensorType.NotThisSensorFamily;
    ThermPro.SensorType ThermProSensorType = ThermPro.SensorType.NotThisSensorFamily;
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
            ( SensorDataRecord.TemperaturePropertyChangedName, uiTemperatureChange),
            ( SensorDataRecord.PM25PropertyChangedName, uiPM25Change),
            ( SensorDataRecord.HumidityPropertyChangedName, uiHumidityChange),
        };

        // Change: set up the graph by making an OxyPlotModel
        OxyPlotModel = OxyPlotUtilities.MakeOxyPlotModel("Sensor Data");

        // Set up the Connect button and Battery visibility
        uiBTConnectionControl.SetConnectVisibility(Visibility.Collapsed);
        if (!CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Battery))
        {
            uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
        }

        // Note: you have to remove the sensor from the uiDeviceDataList entirely. You can't just
        // set it to invisible because the items will still show up
        CurrTableCustomization.TableColumns.Add("Name"); // always show the nae

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Temperature))
        {
            OxyPlotUtilities.AddLine(OxyPlotModel, 5, 30, "Temperature", "Temperature");
            CurrTableCustomization.TableColumns.Add("Temperature");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataTemperature);
        }

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Humidity))
        {
            OxyPlotUtilities.AddLine(OxyPlotModel, 5, 20, "Humidity", "Humidity");
            CurrTableCustomization.TableColumns.Add("Humidity");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataHumidity);
        }

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.PM25))
        {
            OxyPlotUtilities.AddLine(OxyPlotModel, 5, 5, "PM25", "PM25");
            CurrTableCustomization.TableColumns.Add("PM25");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataPM25);
        }

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Pressure))
        {
            OxyPlotUtilities.AddLine(OxyPlotModel, 5, 10, "Pressure", "Pressure", double.NaN, OxyPlot.Axes.AxisPosition.Right);
            CurrTableCustomization.TableColumns.Add("Pressure");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataPressure);
        }


        //
        uiOxyPlot.Model = OxyPlotModel;

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        UtilitiesWinUI3.UtilitiesWinUI3.InitializeKeyLineColorsFromDefaultOxyPlot(OxyPlotModel, rootPanel);
        // Advertisement-based devices don't really have a device ID. CurrSaveData = AllSaveData.FindWithId(DataContextAsKnownDevice.Id);
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
    /// This is a two-way street. Setting the DataContest to the KnownDevice will update some UX and will
    /// trigger looking up the SaveData and change more things. And it will actually connect to the device.
    /// AND this will update the KnownDevice with, e.g., the DeviceId and the BluetoothLEDevice which will be
    /// used by other bits of the system.
    /// </summary>
    private async void Control_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // FYI: by the time this method is called, the DataContext in the object is already set

        if (args.NewValue == null) return; // just bogus; ignore.
        //         InitializeUX(); // ensure we're initialized.


        // Must have been set as a KnownDevice; otherwise we're in a very weird state.
        // DataContxtAsKnownDevice is just the DataContext cast (with an "as") to KnownDevice.
        if (DataContextAsKnownDevice == null)
        {
            Log($"Impossible Error: {InternalDeviceType}: Data context change, but it's not a KnownDevice. Type is {args.NewValue.GetType()}");
            return;
        }

        uiAddress.Text = DataContextAsKnownDevice.Advertisement.AddressAsString;
        CurrSaveData = AllSaveData.FindWithAdvertisementAddress(DataContextAsKnownDevice.Advertisement.Addr); // might return null for the first connection
        // Specific to the Common_EnvironmentalControl: since this control works via advertisements, we're 
        // guaranteed to always get a CurrSaveData at this point.


        // It's critical to set these!
        DataContextAsKnownDevice.Id = DataContextAsKnownDevice.Advertisement.AddressAsString; //  Device.ble.DeviceId ?? ""; // never null :-)
        //DataContextAsKnownDevice.BTLEDevice = Device.ble;

        KnownDeviceName = DataContextAsKnownDevice.Advertisement.BestName;
        uiKnownDeviceName.Text = KnownDeviceName;


        // Initialize data values. Somewhat ugly code :-(
        GoveeSensorType = Govee.AdvertIsSensorFamily(DataContextAsKnownDevice.Advertisement);
        if (GoveeSensorType != Govee.SensorType.NotThisSensorFamily) CurrSensorFamily = SensorFamily.Govee;

        SensorProSensorType = SensorPro.AdvertIsSensorFamily(DataContextAsKnownDevice.Advertisement);
        if (SensorProSensorType != SensorPro.SensorType.NotThisSensorFamily) CurrSensorFamily = SensorFamily.SensorPro;

        ThermProSensorType = ThermPro.AdvertIsSensorFamily(DataContextAsKnownDevice.Advertisement);
        if (ThermProSensorType != ThermPro.SensorType.NotThisSensorFamily) CurrSensorFamily = SensorFamily.ThermPro;

        HandleMyAdvertisement(DataContextAsKnownDevice.Advertisement);
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
            if (oldPrefs != null && newPrefs.Temperature != oldPrefs.Temperature)
            {
                // Change: based on your knowledge of the sensor data, change the temperature readings.
                data.Temperature = BluetoothWatcher.Units.Temperature.Convert(data.Temperature, oldPrefs.Temperature, CurrUserPrefs.Temperature);
            }
            if (oldPrefs != null && newPrefs.Pressure != oldPrefs.Pressure)
            {
                // Change: based on your knowledge of the sensor data, change the pressure readings.
                data.Pressure = BluetoothWatcher.Units.Pressure.Convert(data.Pressure, oldPrefs.Pressure, CurrUserPrefs.Pressure);
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
        if (CurrSensor_Data == null) return;
        UpdateSparkles(name); // name is from e.PropertyName when the Device does a PropertyChanged.

        // Update data from the device to match the current preferred units. Will create the values as needed.
        CurrSensor_DataUnits = DeviceSpecificSensorData.CopyToOrClone(CurrSensor_Data, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

        // Track the historical data
        switch (name)
        {
            case "*": // All the data changed. This is what always happens with the sensor.
            case SensorDataRecord.TemperaturePropertyChangedName:
            case SensorDataRecord.PM25PropertyChangedName:
            case SensorDataRecord.HumidityPropertyChangedName:
                UpdateHistoricalDataAndGraph(CurrSensor_DataUnits);
                break;
        }



        if (name == SensorDataRecord.TemperaturePropertyChangedName || name == "" || name == "*")
        {
            uiTemperature.Text = BluetoothWatcher.Units.Temperature.AsString(CurrSensor_DataUnits.Temperature, CurrUserPrefs.Temperature);
        }
        if (name == SensorDataRecord.PM25PropertyChangedName || name == "" || name == "*")
        {
            uiPM25.Text = CurrSensor_Data.PM25.ToString("0.0");
        }
        if (name == SensorDataRecord.HumidityPropertyChangedName || name == "" || name == "*")
        {
            uiHumidity.Text = CurrSensor_Data.Humidity.ToString("0.0") + "%";
        }
        if (name == SensorDataRecord.BatteryPropertyChangedName || name == "" || name == "*") 
        {
            uiBTConnectionControl.SetBatteryLevel(CurrSensor_Data.BatteryInPercent);
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
    /// Called by MainWindow / Advertisement Watcher when a new advertisement from the specific (known)
    /// device is seen.
    /// </summary>
    /// <param name="data"></param>
    public void HandleMyAdvertisement(WatcherData data)
    {
        UIThreadHelper.CallOnUIThread(() =>
        {
            if (!IsLoaded) return; // Won't be loaded when we exit the app!

            switch (CurrSensorFamily)
            {
                case SensorFamily.Govee:
                    CurrSensor_Data = Govee.Parse(GoveeSensorType, data, CurrSensor_Data as Govee);
                    break;
                case SensorFamily.SensorPro:
                    CurrSensor_Data = SensorPro.Parse(SensorProSensorType, data, CurrSensor_Data as SensorPro);
                    break;
                case SensorFamily.ThermPro:
                    CurrSensor_Data = ThermPro.Parse(ThermProSensorType, data, CurrSensor_Data as ThermPro);
                    break;
            }
            if (CurrSensor_Data == null)
            {
                // Lots of reasons it might be invalid. For example, we get an advert that includes a 
                // name (and creates this control), but the advert doesn't include the data because
                // we haven't gotten the BT advertisement response yet.
                Log($"ERROR: unable to parse sensor data for sensor type {CurrSensorFamily}");
                return;
            }
            InitializeUX(); // Will initialize the UX as appropriate
            CurrSaveData.History.UpdateAdvertisementHistory(data.MostRecentAdvertisement.Timestamp);
            CurrSaveData.History.UpdateDataHistory(data.MostRecentAdvertisement.Timestamp);
            CurrSensor_Data.Name = data.BestName;
            CurrSensor_Data.TimestampMostRecent = data.MostRecentAdvertisement.Timestamp;
            UpdateDeviceDataUX("*"); // Update all the data!
        });

    }




    public string GetDetails(IDeviceControlBasic.DetailsType detailsType)
    {
        return "Internal error: no details are available";
    }
    #endregion

} // end of class BTCommon_EnvironmentalControl // CHANGE: update the comment to match the class name
