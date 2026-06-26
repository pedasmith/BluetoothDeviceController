using BluetoothProtocols;
using BluetoothProtocolsDevicesCore;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using BluetoothWinUI3.BTDeviceUnitConverters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.

using Utilities;
using Windows.Devices.Bluetooth;
using WinUI.TableView;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif

#region Set these to match your device
using DeviceSpecificSensorData = SensorDataRecord; // Change: 
#endregion


[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTCommon_EnvironmentalControl : UserControl, IDeviceControlBasic, IDeviceControlDevice, IHandleMyBTAdvertisements
{
    #region Settings that must be updated for a new device
    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "EnvironmentSensor";
    #endregion

    #region Advanced settings and values
    /// <summary>
    /// Every use case might have a different point of view about how frequently to update
    /// the historical data (the data displayed in the graph + shown in the table view
    /// + exported). A good default is 5 seconds.
    /// </summary>
    const double HistoricalDataUpdateRateInSeconds = 5.0;
    #endregion

    public BTCommon_EnvironmentalControl()
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += Control_DataContextChanged;


        // Setting up the OxyPlotModel is moved to SetupOnFirstValidData(). 
        // That's because the model can only be set up when we have the first
        // data, and sometimes that's delayed (the adverisement that triggers
        // making this control might not include the environment data)

        //
        // Set up the uiTableView
        // https://w-ahmad.dev/WinUI.TableView/index.html
        // https://github.com/w-ahmad/WinUI.TableView
        //
        uiTableView.AutoGeneratingColumn += CurrTableCustomization.TableView_AutoGeneratingColumn_UseCustomization;
    }

    #region Instance value for a device
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the colleciton.
    /// </summary>
    public DataCollection<DeviceSpecificSensorData> HistoricalDataUnits { get; } = new();
    public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return HistoricalDataUnits.Data; }
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
    TableViewColumnCustomization CurrTableCustomization = new TableViewColumnCustomization();
    #endregion

    bool FirstLoad = true;
    private void Control_Loaded(object sender, RoutedEventArgs e)
    {
        // Loaded gets called first when it's first loaded an then each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        if (!FirstLoad) return;
        FirstLoad = false;
        switch (CurrSensorFamily)
        {
            case SensorFamily.Govee: uiDeviceName.Text = "Sensor " + GoveeSensorType.ToString(); break;
            case SensorFamily.SensorPro: uiDeviceName.Text = "Sensor " + SensorProSensorType.ToString(); break;
            case SensorFamily.ThermPro: uiDeviceName.Text = "Sensor " + ThermProSensorType.ToString(); break;
        }

        // Change: set the right sparkles
        ControlsWithSparkles = new List<(string, Microsoft.UI.Xaml.Documents.Run)>()
        {
            ( SensorDataRecord.TemperaturePropertyChangedName, uiTemperatureChange),
            ( SensorDataRecord.PM25PropertyChangedName, uiPM25Change),
            ( SensorDataRecord.HumidityPropertyChangedName, uiHumidityChange),
        };
    }


    // TODO: should these be discoverable? Maybe from the Model which already has the user friendly names?
    List<string> _LineNames = new List<string>() { "Temperature", "Humidity", "PM25", }; // CHANGE:
    public List<string> LineNames { get { return _LineNames; }  }

    /// <summary>
    /// The DataContext is a WinUI3 (and the rest of XAML) thing, and is just an object. And it can be
    /// set by anyone, at any time, to any value. The Bluetooth controls generally require that the 
    /// DataContext be a KnownDevice (which is turn is a bunch of data: the SupportedDevice, the
    /// WatcherData / Bluetooth advertisement that triggered this control being created, etc.)
    /// 
    /// DataContextAsKnownDevice is either a real KnownDevice or it's null.
    /// </summary>
    public KnownDevice DataContextAsKnownDevice { get { return DataContext as KnownDevice; } }

    int NLeftAxis = 0;
    int NRightAxis = 0;

    private void AddPlotAxisAndSeries(PlotModel oxyPlotModel, AxisPosition position, string title, string key, OxyColor color)
    {
        var tier = position == AxisPosition.Left ? NLeftAxis++ : NRightAxis++;
        var axis = new LinearAxis()
        {
            Position = position,
            PositionTier = tier,
            Title = title,
            Key = key,
        };

        var series = new LineSeries
        {
            Title = title,
            Color = color,
            StrokeThickness = 0.75,
            MarkerType = MarkerType.None,
            DataFieldX = "TimestampMostRecentDT",
            DataFieldY = key,
            YAxisKey = key,
            ItemsSource = HistoricalDataUnits.Data,
        };

        oxyPlotModel.Axes.Add(axis);
        oxyPlotModel.Series.Add(series);
    }

    // H.OxyPlot
    private PlotModel OxyPlotModel { get; set; } = new PlotModel
    {
        Title = "Environment Data",
        PlotAreaBorderColor = OxyColors.Transparent,
        TextColor = OxyColors.Black,
        Axes =
            {
                new DateTimeAxis { Position = AxisPosition.Bottom },
            },
    };
    /// <summary>
    /// Set the axes to either visible or invisible. 
    /// </summary>
    public void SetAxesVisibility(bool isVisible)
    {
        foreach (var axis in OxyPlotModel.Axes)
        {
            axis.IsAxisVisible = isVisible;
        }
        uiOxyPlot.InvalidatePlot(false); // false means just update for the axis
    }

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

        uiAddress.Text = DataContextAsKnownDevice.Advertisement.AddressAsString;
        CurrSaveData = AllSaveData.FindWithAdvertisementAddress(DataContextAsKnownDevice.Advertisement.Addr); // might return null for the first connection
        // Specific to the Common_EnvironmentalControl: since this control works via advertisements, we're 
        // guaranteed to always get a CurrSaveData at this point.

        // In the Nordic_Thingy, setting the DataContext to a KnownDevice is the trigger
        // for connecting via BluetoothLE to a device. But this Sensor display is driven
        // entirely by advertisements, and the device is not needed.
        //Device = new Nordic_Thingy()
        //{
        //    ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
        //};
        //if (Device.ble == null)
        //{
        //    Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString//(DataContextAsKnownDevice.Advertisement.Addr)}");
        //    return;
        //}

        // It's critical to set these!
        DataContextAsKnownDevice.Id = DataContextAsKnownDevice.Advertisement.AddressAsString; //  Device.ble.DeviceId ?? ""; // never null :-)
        //DataContextAsKnownDevice.BTLEDevice = Device.ble;

        KnownDeviceName = DataContextAsKnownDevice.Advertisement.BestName;


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
            uiDeviceName.Text = KnownDeviceName;
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

        // Update the saved data in the HistoricalSensorDataUnits to match the new user preferences.
        foreach (var data in HistoricalDataUnits.Data)
        {
            if (oldPrefs != null && newPrefs.Temperature != oldPrefs.Temperature)
            {
                data.Temperature = BluetoothWatcher.Units.Temperature.Convert(data.Temperature, oldPrefs.Temperature, CurrUserPrefs.Temperature);
            }
            if (oldPrefs != null && newPrefs.Pressure != oldPrefs.Pressure)
            {
                data.Pressure = BluetoothWatcher.Units.Pressure.Convert(data.Pressure, oldPrefs.Pressure, CurrUserPrefs.Pressure);
            }
        }

        UpdateDeviceDataUX(""); // the graph is changed, but not the data
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
    /// needs to be updated. For the Govee and ThermPro, there's never just one piece of data; we either get it all 
    /// or just the one.
    /// </summary>
    /// <param name="name"></param>
    private void UpdateDeviceDataUX(string name)
    {
        if (CurrSensor_Data == null) return;

        // name is from e.PropertyName when the Device does a PropertyChanged.
        UpdateSparkles(name);

        // Update data from the device to match the current preferred units. Will create the values as needed.
        CurrSensor_DataUnits = DeviceSpecificSensorData.CopyToOrClone(CurrSensor_Data, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

        // Track the historical data
        switch (name)
        {
            case "*": // All the data changed. This is what always happens with the sensor.
            case SensorDataRecord.TemperaturePropertyChangedName:
            case SensorDataRecord.PM25PropertyChangedName:
            case SensorDataRecord.HumidityPropertyChangedName:
                UpdateHistoricalDataAndGraph();
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


    /// <summary>
    /// Helper code to update historical data. The sensor might send a lot of data; the history only
    /// saves a portion of the data. Technicaly, every time there's new data we either update
    /// the most recent entry OR we add a new entry.
    /// </summary>
    private void UpdateHistoricalDataAndGraph()
    {
        var deltaInSeconds = CurrSensor_DataUnits.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
        var verb = (deltaInSeconds > HistoricalDataUpdateRateInSeconds)
            ? DataCollection<DeviceSpecificSensorData>.Verb.Add : DataCollection<DeviceSpecificSensorData>.Verb.ReplaceMostRecent;
        HistoricalDataUnits.Update(CurrSensor_DataUnits, verb); // Will add or replace the data and will copy as needed.

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

    #region Exporters

    /// <summary>
    /// Called from MainWindow when the user asks for, e.g., exported data or graphs.
    /// </summary>
    /// <returns></returns>
    public IDeviceControlBasic.UXCapabilities GetUXCapabilities()
    {
        var retval =
            IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng
            | IDeviceControlBasic.UXCapabilities.CanGetData
            | IDeviceControlBasic.UXCapabilities.CanRename
            | IDeviceControlBasic.UXCapabilities.CanShowTable
            ;
        return retval;
    }

    public async void ExportGraphAsPng()
    {
        await UtilitiesWinUI3.UtilitiesWinUI3.ExportGraphAsPngAsync(uiOxyPlot, rootPanel.Background, Log);
    }


    string name = "EnvironmentSensor";
    bool FirstCallWithIsValid = true;


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

            name = data.BestName;
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
            CurrSaveData.History.UpdateAdvertisementHistory(data.MostRecentAdvertisement.Timestamp);
            CurrSaveData.History.UpdateDataHistory(data.MostRecentAdvertisement.Timestamp);
            CurrSensor_Data.Name = name;
            CurrSensor_Data.TimestampMostRecent = data.MostRecentAdvertisement.Timestamp;
            if (true) // was: CurrSensor_Data.IsValid
            {
                if (FirstCallWithIsValid)
                {
                    SetupOnFirstValidData();
                    // Advertisements aren't really "connected".
                    CurrSaveData.History.UpdateConnectionHistory(data.MostRecentAdvertisement.Timestamp, BluetoothConnectionStatus.Connected);

                    FirstCallWithIsValid = false;
                }
                UpdateDeviceDataUX("*"); // Update all the data!
            }
        });

    }

    private void SetupOnFirstValidData()
    {
        // Set up the Connect button and Battery visibility
        uiBTConnectionControl.SetConnectVisibility(Visibility.Collapsed);
        if (!CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Battery))
        {
            uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
        }

        // Note: you have to remove the sensor from the uiDeviceDataList entirely. You can't just
        // set it to invisible because the items will still show up

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Temperature))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Left, "Temperature", "Temperature", OxyColors.DarkBlue);
            CurrTableCustomization.TableColumns.Add("Temperature");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataTemperature);
        }

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Humidity))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Left, "Humidity", "Humidity", OxyColors.Violet);
            CurrTableCustomization.TableColumns.Add("Humidity");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataHumidity);
        }

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.PM25))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Right, "PM25", "PM25", OxyColors.Gray);
            CurrTableCustomization.TableColumns.Add("PM25");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataPM25);
        }

        if (CurrSensor_Data.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Pressure))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Right, "Pressure", "Pressure", OxyColors.LightBlue);
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

        var saveData = AllSaveData.FindWithId(DataContextAsKnownDevice.Id);
        if (saveData != null)
        {
            UpdateUX(saveData);
        }

        //
        // Initialize the table
        //
        uiTableView.ItemsSource = HistoricalDataUnits.Data;
    }

    /// <summary>
    /// Update the visibility for each of the data block (e.g., the block that says "Temperature")
    /// </summary>

    private void AdjustSensorVisibility(Panel panel, SensorDataRecord.SensorPresent flag)
    {
        var visibility = (CurrSensor_Data.IsSensorPresent.HasFlag(flag)) ? Visibility.Visible : Visibility.Collapsed;
        // e.g., SensorDataRecord.SensorPresent.Temperature
        panel.Visibility = visibility;
    }

    public string GetDetails(IDeviceControlBasic.DetailsType detailsType)
    {
        return "Internal error: no details are available";
    }

    IHandleNotifyDeviceControlChanges NotifyDeviceControlChangesWindows = null;
    public void SetNotifyDeviceControlChanges(IHandleNotifyDeviceControlChanges mainWindow)
    {
        NotifyDeviceControlChangesWindows = mainWindow;
    }

#endregion

} // end of class BTCommon_EnvironmentalControl
