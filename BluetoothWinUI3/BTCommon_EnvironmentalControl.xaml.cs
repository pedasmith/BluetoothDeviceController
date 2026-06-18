using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.

using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Storage.Streams;
using WinUI.TableView;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif



[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTCommon_EnvironmentalControl : UserControl, IDeviceControlBasic, IDeviceControlDevice, IHandleMyBTAdvertisements
{

    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>

    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400

    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "EnvironmentSensor";
    string KnownDeviceName = "device";

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the colleciton.
    /// </summary>
    public SensorDataCollection HistoricalDataUnits { get; } = new SensorDataCollection();
    public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return HistoricalDataUnits.Data ; }
    public IBTCommonMetaData GetDataMostRecent()
    { return HistoricalDataUnits.Count == 0 ? null : HistoricalDataUnits.Data[HistoricalDataUnits.Count - 1]; }
    /// <summary>
    /// The current environment data directly from the sensor (it's the original data, not a copy). The data is 
    /// always in the 'native' units (e.g., always celcius for temperature).
    /// </summary>
    CopyableSensorDataRecord CurrSensor = null;
    enum SensorFamily {  Govee, SensorPro, ThermPro};
    SensorFamily CurrSensorFamily = SensorFamily.Govee;

    /// <summary>
    /// Similar to CurrGoveeData , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalSensorDataUnits collection.
    /// </summary>
    CopyableSensorDataRecord CurrSensorUnits = null;


    /// <summary>
    /// There are multiple sensors that this one control can handle. They are all initialized to 'NotThisSensorFamily'
    /// </summary>
    Govee.SensorType GoveeSensorType = Govee.SensorType.NotThisSensorFamily; 
    SensorPro.SensorType SensorProSensorType = SensorPro.SensorType.NotThisSensorFamily; 
    ThermPro.SensorType ThermProSensorType = ThermPro.SensorType.NotThisSensorFamily; 
    List<string> TableColumns = new();

    public BTCommon_EnvironmentalControl()
    {
        InitializeComponent();
        this.Loaded += BTCommon_EnvironmentalControl_Loaded;
        this.DataContextChanged += BTCommon_EnvironmentalControl_DataContextChanged;



        //
        // Set up the uiTableView
        // https://w-ahmad.dev/WinUI.TableView/index.html
        // https://github.com/w-ahmad/WinUI.TableView
        //
        uiTableView.AutoGeneratingColumn += (s, e) =>
        {
            switch (e.PropertyName)
            {
                case "TimestampMostRecent":
                    var col = e.Column as TableViewDateColumn;
                    if (col == null)
                    {
                        Log($"Error: TimestampMostRecent is not a TableViewDateColumn.");
                        return;
                    }
                    col.IsReadOnly = true;
                    // DateFormat is from the DateTimeFormatter which is completely different from the 
                    // normal format from DateTimeOffset.ToString().
                    // https://learn.microsoft.com/en-us/uwp/api/windows.globalization.datetimeformatting.datetimeformatter?view=winrt-28000
                    col.DateFormat = "{hour.integer}:{minute.integer(2)}:{second.integer(2)}";
                    col.Header = "Time";
                    break;
                case "TimestampMostRecentDT":
                    e.Cancel = true; // Don't generate a column for this property because it's not user friendly. 
                    break;
                default:
                    if (!TableColumns.Contains(e.PropertyName))
                    {
                        // The sensor has a bunch of fields (e.g., "TagType") which should not be part of the data grid.
                        e.Cancel = true;
                    }
                    break;
            }
        };

    }

    bool FirstLoad = true;
    private void BTCommon_EnvironmentalControl_Loaded(object sender, RoutedEventArgs e)
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
    }


    // TODO: should these be discoverable? Maybe from the Model which already has the user friendly names?
    public List<string> LineNames { get { return ["Temperature", "Humidity", "PM25",]; }  }

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
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                if (lineSeries.DataFieldY == name)
                {
                    return UtilitiesOxyColor.OxyColorToUint(lineSeries.Color);
                }
            }
        }
        return UtilitiesOxyColor.OxyColorToUint(OxyColors.Undefined);
    }



    /// <summary>
    /// This is a two-way street. Setting the DataContest to the KnownDevice will update some UX and will
    /// trigger looking up the SaveData and change more things. And it will actually connect to the device.
    /// AND this will update the KnownDevice with, e.g., the DeviceId and the BluetoothLEDevice which will be
    /// used by other bits of the system.
    /// </summary>
    private async void BTCommon_EnvironmentalControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // FYI: by the time this method is called, the DataContext is already set

        if (args.NewValue == null) return; // just bogus; ignore.

        // Must have been set as a KnownDevice; otherwise we're in a very weird state.
        if (DataContextAsKnownDevice == null)
        {
            Log($"Impossible Error: {InternalDeviceType}: Data context change, but it's not a KnownDevice. Type is {args.NewValue.GetType()}");
            return;
        }

        uiAddress.Text = DataContextAsKnownDevice.Advertisement.AddressAsString;

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

    public IDeviceControlBasic.Visibility GetDataGridVisibility()
    {
        var retval = (uiDataGridPanel.Visibility == Visibility.Visible)
            ? IDeviceControlBasic.Visibility.Visible : IDeviceControlBasic.Visibility.Collapsed;
        return retval;
    }

    public void SetDataGridVisibility(IDeviceControlBasic.Visibility visibility)
    {
        switch (visibility)
        {
            case IDeviceControlBasic.Visibility.Collapsed:
                uiOxyPlot.Visibility = Visibility.Visible;
                uiDataGridPanel.Visibility = Visibility.Collapsed;
                break;
            case IDeviceControlBasic.Visibility.Visible:
            default:
                uiOxyPlot.Visibility = Visibility.Collapsed;
                uiDataGridPanel.Visibility = Visibility.Visible;
                break;
        }
    }


    /// <summary>
    /// Sets the line colors for the keys based on the OxyPlotModel values.
    /// </summary>
    private void InitializeKeyLineColorsFromDefaultOxyPlot()
    {
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                var name = lineSeries.DataFieldY;
                if (!String.IsNullOrEmpty(name))
                {
                    if (lineSeries.Color != OxyColors.Automatic
                        && lineSeries.Color != OxyColors.Undefined)
                    {
                        var color = UtilitiesOxyColor.OxyColorToUint(lineSeries.Color);
                        SetLineKeyColor(name, color);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Updates the OxyPlot line with a given name (e.g., "Temperature"). Is called from MainWindow when the
    /// user picks a new color.
    /// </summary>
    /// <param name="lineName"></param>
    /// <param name="color"></param>

    public void UpdateGraphColor(string lineName, uint color)
    {
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lseries)
            {
                if (lseries.DataFieldY == lineName)
                {
                    lseries.Color = OxyColor.FromUInt32((uint)color);
                    OxyPlotModel.InvalidatePlot(false); //DOC: false is just the axes, true is everything.
                    break;
                }
            }
        }
        SetLineKeyColor(lineName, color);
    }

    /// <summary>
    /// The "line" here are the key lines that are generally placed below the data. For example, there's a block 
    /// with the most recent Temperature data, plus a little title, plus a line that gets colored with the same
    /// color as the graph line.
    /// </summary>
    /// <param name="lineName"></param>
    /// <param name="color"></param>
    private void SetLineKeyColor(string lineName, uint color)
    {
        foreach (Line line in UtilitiesWinUI3.UtilitiesWinUI3.FindVisualChildren<Line>(rootPanel))
        {
            if ((line.Tag as string) == lineName + "Color") // e.g., Tag="TemperatureColor"
            {
                byte a = 0xFF;
                byte r = (byte)((color >> 16) & 0xFF);
                byte g = (byte)((color >> 8) & 0xFF);
                byte b = (byte)((color >> 0) & 0xFF);
                line.Stroke = new SolidColorBrush(Windows.UI.Color.FromArgb(a, r, g, b));
            }
        }
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
            CurrSensorUnits?.Name = KnownDeviceName;
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

    public void UpdateUX(MainWindow.WindowSize windowSize, Windows.Foundation.Size largeActualSize)
    {
        CurrWindowSize = windowSize;
        switch (CurrWindowSize)
        {
            default:
            case MainWindow.WindowSize.Normal:
                rootPanel.Width = 380;
                rootPanel.Height = 380;
                SetAxesVisibility(false);
                break;
            case MainWindow.WindowSize.Large:
                rootPanel.Width = largeActualSize.Width;
                rootPanel.Height = largeActualSize.Height;
                SetAxesVisibility(true);
                break;
        }
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

#if NEVER_EVER_DEFINED
    private void Device_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        UIThreadHelper.CallOnUIThread(() =>
        {
            if (!IsLoaded) return;
            UpdateDeviceDataUX(e.PropertyName);
        });
    }
#endif

    Dictionary<string, int> NPropertyChanges { get; } = [];
    //List<string> Sparkles = new List<string>() { "\u00A0", "*" }; // ✨", "💫", "🌟", "⚡", "🔥", "💥" };
    readonly List<string> Sparkles = ["╺", "╼", "╾", "╸", "╾", "╼"];

    private void UpdateSparkles(string name)
    {
        if (name == "") return;
        NPropertyChanges[name] = NPropertyChanges.GetValueOrDefault(name, 0) + 1;

        if (name == SensorDataRecord.TemperaturePropertyChangedName || name == "*")
        {
            uiTemperatureChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }
        if (name == SensorDataRecord.PM25PropertyChangedName || name == "*")
        {
            uiPM25Change.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }
        if (name == SensorDataRecord.HumidityPropertyChangedName || name == "*")
        {
            uiHumidityChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
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
        if (CurrSensor == null) return;

        // name is from e.PropertyName when the Device does a PropertyChanged.

        UpdateSparkles(name);

        // Update to match the current preferred units. Will create a new CurrSensorUnits the first time
        // it's called
        CurrSensorUnits = CurrSensor.CopyToAndUpdateUnits(CurrSensorUnits, CurrUserPrefs, KnownDeviceName);

        // Track the historical data
        switch (name)
        {
            case "*": // All the data changed. This is what always happens with the sensor.
            case SensorDataRecord.TemperaturePropertyChangedName:
            case SensorDataRecord.PM25PropertyChangedName:
            case SensorDataRecord.HumidityPropertyChangedName:
                // Unlike the Nordic_Thingy52 where the different values come in at different
                // time, the sensor data comes in all at once.
                var deltaInSeconds = CurrSensor.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
                var verb = (deltaInSeconds > 5) ? SensorDataCollection.Verb.Add : SensorDataCollection.Verb.ReplaceMostRecent;
                HistoricalDataUnits.Update(CurrSensorUnits, verb); // Will add or replace the data and will copy as needed.

                //
                // Update the OxyPlot because it doesn't tracked the INotifyCollectionChanged
                //
                if (verb == SensorDataCollection.Verb.Add && HistoricalDataUnits.Count == 2)
                {
                    // DOC: Can't have the axes start off invisible because then they can't be switched back on
                    if (CurrWindowSize == MainWindow.WindowSize.Normal)
                    {
                        // Just in case the user quick set to large.
                        SetAxesVisibility(false);
                    }
                }
                uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines
                break;
        }



        if (name == SensorDataRecord.TemperaturePropertyChangedName || name == "" || name == "*")
        {
            uiTemperature.Text = BluetoothWatcher.Units.Temperature.AsString(CurrSensorUnits.Temperature, CurrUserPrefs.Temperature);
        }
        if (name == SensorDataRecord.PM25PropertyChangedName || name == "" || name == "*")
        {
            uiPM25.Text = CurrSensor.PM25.ToString("0.0");
        }
        if (name == SensorDataRecord.HumidityPropertyChangedName || name == "" || name == "*")
        {
            uiHumidity.Text = CurrSensor.Humidity.ToString("0.0") + "%";
        }
        if (name == SensorDataRecord.BatteryPropertyChangedName || name == "" || name == "*") 
        {
            uiBTConnectionControl.SetBatteryLevel(CurrSensor.BatteryInPercent);
        }
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
        try
        {
            var exporter = new Exporters.ExportControlAsPng();
            var outputStream = await exporter.ExportAsync(uiOxyPlot, rootPanel.Background);
            var dataPackage = new DataPackage()
            {
                RequestedOperation = DataPackageOperation.Copy
            };
            outputStream.Seek(0);
            var streamref = RandomAccessStreamReference.CreateFromStream(outputStream);
            dataPackage.SetBitmap(streamref);
            Clipboard.SetContent(dataPackage);
            Clipboard.Flush();
        }
        catch (Exception ex)
        {
            Log($"Error: unable to make PNG data for the clipboard; {ex.Message}");
        }
    }


    /* Code to write to a file. Note that AFAICT the BinaryReader, when it goes out of scope, takes the
     * outputStream with it, so this code is the last code that can use the outputStream.
     * This code works fine, but it's not needed for my app. That's why it's commented out.
     */
#if TURN_ON_GRAPH_TO_FILE_TEST_CODE
        try
        {
            outputStream.Seek(0);
            using (var reader = new BinaryReader(outputStream.AsStreamForRead()))
            {
                var bytes = reader.ReadBytes((int)outputStream.Size);
                System.IO.File.WriteAllBytes("c:\\temp\\2026\\junkgraph.png", bytes); 
            }
        }
        catch (Exception ex)
        {
            Log($"Error: 20: unable to make PNG file; {ex.Message}");
        }
#endif
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
                    CurrSensor = Govee.Parse(GoveeSensorType, data, CurrSensor as Govee);
                    break;
                case SensorFamily.SensorPro:
                    CurrSensor = SensorPro.Parse(SensorProSensorType, data, CurrSensor as SensorPro);
                    break;
                case SensorFamily.ThermPro:
                    CurrSensor = ThermPro.Parse(ThermProSensorType, data, CurrSensor as ThermPro);
                    break;
            }
            if (CurrSensor == null)
            {
                // Lots of reasons it might be invalid. For example, we get an advert that includes a 
                // name (and creates this control), but the advert doesn't include the data because
                // we haven't gotten the BT advertisement response yet.
                Log($"ERROR: unable to parse sensor data for sensor type {CurrSensorFamily}");
                return;
            }
            CurrSensor.Name = name;
            CurrSensor.TimestampMostRecent = data.MostRecentAdvertisement.Timestamp;
            if (CurrSensor.IsValid)
            {
                if (FirstCallWithIsValid)
                {
                    SetupOnFirstValidData();
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
        if (!CurrSensor.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Battery))
        {
            uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
        }

        // Note: you have to remove the sensor from the uiDeviceDataList entirely. You can't just
        // set it to invisible because the items will still show up

        if (CurrSensor.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Temperature))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Left, "Temperature", "Temperature", OxyColors.DarkBlue);
            TableColumns.Add("Temperature");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataTemperature);
        }

        if (CurrSensor.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Humidity))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Left, "Humidity", "Humidity", OxyColors.Violet);
            TableColumns.Add("Humidity");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataHumidity);
        }

        if (CurrSensor.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.PM25))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Right, "PM25", "PM25", OxyColors.Gray);
            TableColumns.Add("PM25");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataPM25);
        }

        if (CurrSensor.IsSensorPresent.HasFlag(SensorDataRecord.SensorPresent.Pressure))
        {
            AddPlotAxisAndSeries(OxyPlotModel, AxisPosition.Right, "Pressure", "Pressure", OxyColors.LightBlue);
            TableColumns.Add("Pressure");
        }
        else
        {
            uiDeviceDataList.Items.Remove(uiDeviceDataPressure);
        }


        //
        uiOxyPlot.Model = OxyPlotModel;

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        InitializeKeyLineColorsFromDefaultOxyPlot();

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
        var visibility = (CurrSensor.IsSensorPresent.HasFlag(flag)) ? Visibility.Visible : Visibility.Collapsed;
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
