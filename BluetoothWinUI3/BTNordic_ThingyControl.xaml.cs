using BluetoothProtocols;
using BluetoothProtocols.NS_Nordic_Thingy;
using BluetoothProtocolsDevicesCore;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using BluetoothWinUI3.BTDeviceUnitConverters;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
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

using DeviceSpecificSensorData = Nordic_Thingy.Environment_Data; // Change: 
using DeviceSpecificSensorSecondaryData = Nordic_Thingy.EnvironmentColor_Data; // Change: pick secondary sensor if needed
using DeviceSpecificType = Nordic_Thingy; // Change: pick your device, not BTStandard_Demo


public static class Environment_DataUtilities
{
    /// <summary>
    /// Returns true when the data has valid pressure, humidity data. Can't detect when the temperature is invalid
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static bool IsValidPH(this DeviceSpecificSensorData data)
    {
        // 0.0 is a valid temperature :-(
        var retval = (data.Pressure != 0.0 && data.Humidity!= 0.0);
        return retval;
    }
}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTNordic_ThingyControl : UserControl, IDeviceControlBasic, IDeviceControlDevice
{

    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>

    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400


    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "Nordic_Thingy";
    Nordic_Thingy Device;
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;
    ulong OriginalBTAddr = 0xFFFFFFFF_FFFFFFFF;

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the colleciton.
    /// </summary>
    public DataCollection<DeviceSpecificSensorData> HistoricalDataUnits { get;  } = new DataCollection<DeviceSpecificSensorData>();
    public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return HistoricalDataUnits.Data; }
    public IBTCommonMetaData GetDataMostRecent() 
        { return HistoricalDataUnits.Count == 0 ? null : HistoricalDataUnits.Data[HistoricalDataUnits.Count-1]; }
    /// <summary>
    /// The current environment data directly from the sensor (it's the original data, not a copy). The data is 
    /// always in the 'native' units (e.g., always celcius for temperature).
    /// </summary>
    DeviceSpecificSensorData CurrSensor_Data = null;

    /// <summary>
    /// Similar to CurrEnvironment_Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalEnvironment_DataUnits collection.
    /// </summary>
    DeviceSpecificSensorData CurrSensor_DataUnits = null;
    DeviceSpecificType.Battery_Data CurrBattery_Data = null;
    DeviceSpecificSensorSecondaryData CurrSensorSecondary_Data = null;




    public BTNordic_ThingyControl()
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += BTNordic_ThingyControl_DataContextChanged;

        //
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
        //uiTableView.CornerButtonMode = TableViewCornerButtonMode.None;
        //uiTableView.SelectionMode = ListViewSelectionMode.None;
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
            }
        };

    }

    // List of controls that have the 'data updated' sparkles
    List<(string, Microsoft.UI.Xaml.Documents.Run)> ControlsWithSparkles = null;


    private void Control_Loaded(object sender, RoutedEventArgs e)
    {
        // Loaded gets called first when it's first loaded an then each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        if (uiTableView.ItemsSource != null) return;
        uiTableView.ItemsSource = HistoricalDataUnits.Data;

        // Change: set the right sparkles
        ControlsWithSparkles = new List<(string, Microsoft.UI.Xaml.Documents.Run)>()
        {
            ( DeviceSpecificType.Temperature_cPropertyChangedName, uiTemperature_cChange),
            ( DeviceSpecificType.Pressure_hpaPropertyChangedName, uiPressure_hpaChange),
            ( DeviceSpecificType.HumidityPropertyChangedName, uiHumidityChange),
            ( DeviceSpecificType.Air_Quality_eCOS_TVOCPropertyChangedName, uieCOSChange),
            ( DeviceSpecificType.Air_Quality_eCOS_TVOCPropertyChangedName, uiTVOCChange),
            ( DeviceSpecificType.Color_RGB_ClearPropertyChangedName, uiColorChange),
        };
    }



    // TODO: should these be discoverable? Maybe from the Model which already has the user friendly names?
    public List<string> LineNames { get { return [ "Temperature", "Pressure", "Humidity", "eCOS", "TVOC" ]; } }

    public KnownDevice DataContextAsKnownDevice { get { return DataContext as KnownDevice; } }


    // H.OxyPlot
    private PlotModel OxyPlotModel { get; set; } = new PlotModel
    {
        Title = "Environment Data",
        PlotAreaBorderColor = OxyColors.Transparent,
        TextColor = OxyColors.Black, 
        Axes =
            {
                new DateTimeAxis { Position = AxisPosition.Bottom },
                new LinearAxis
                {
                    Position = AxisPosition.Left,
                    PositionTier = 0, // PositionTier=0 is the innermost tier. //DOC:
                    MajorGridlineColor = OxyColors.Black,
                    MajorGridlineStyle = LineStyle.Solid,
                    MajorGridlineThickness = 1,
                    MajorStep = 10, // 1 hpa
                    MinimumRange= 30,
                    Title="Pressure",
                    Key="Pressure"
                },
                new LinearAxis
                {
                    Position = AxisPosition.Left,
                    PositionTier = 1,
                    Title="Temperature",
                    Key="Temperature"
                },
                new LinearAxis
                {
                    Position = AxisPosition.Left,
                    PositionTier = 2,
                    Title="Humidity",
                    Key="Humidity"
                },
                new LinearAxis
                {
                    Position = AxisPosition.Right,
                    PositionTier = 0,
                    Minimum = 380, // Initial eCOS is zero, which isn't a realistic value. An actual sensor reading is always 400 or more?
                    Title="eCOS",
                    Key="eCOS"
                },
                new LinearAxis
                {
                    Position = AxisPosition.Right,
                    PositionTier = 1,
                    Title="TVOC",
                    Key="TVOC"
                },
            },
        Series =
            {
                new LineSeries
                {
                    Title = "Temperature",
                    Color = OxyColors.DarkBlue,
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "Temperature",
                    YAxisKey= "Temperature",
                },
                new LineSeries
                {
                    Title = "Pressure",
                    Color = OxyColors.LightBlue,
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "Pressure",
                    YAxisKey= "Pressure",
                },
                new LineSeries
                {
                    Title = "Humidity",
                    Color = OxyColors.Violet,
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "Humidity",
                    YAxisKey= "Humidity",
                },
                new LineSeries
                {
                    Title = "eCOS",
                    Color = OxyColors.Black,
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "eCOS",
                    YAxisKey= "eCOS",
                },
                new LineSeries
                {
                    Title = "TVOC",
                    Color = OxyColors.Gray,
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "TVOC",
                    YAxisKey= "TVOC",
                },
            }
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
    private async void BTNordic_ThingyControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // FYI: by the time this method is called, the DataContext is already set

        if (args.NewValue == null) return; // just bogus; ignore.

        // Must have been set as a KnownDevice; otherwise we're in a very weird state.
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

        Device = new Nordic_Thingy()
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
        CurrSaveData?.UpdateWithDevice(DataContextAsKnownDevice);
        CurrSaveData = AllSaveData.FindWithId(DataContextAsKnownDevice.Id); // now it's "guaranteed" to exist. Use the stable form of the device id.

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        InitializeKeyLineColorsFromDefaultOxyPlot();

        UpdateUX(CurrSaveData); // Shouldn't be null, but also handles null gracefully.

        Device.PropertyChanged += Device_PropertyChanged;
        await Device.NotifyBatteryLevelAsync();
        await Device.NotifyTemperature_cAsync();
        await Device.NotifyPressure_hpaAsync();
        await Device.NotifyHumidityAsync();
        await Device.NotifyAir_Quality_eCOS_TVOCAsync(); // both TVOC and eCOS
        await Device.NotifyColor_RGB_ClearAsync();
        await Device.ReadBatteryLevel(BluetoothCacheMode.Cached); // I'm happy getting unchanged data? TODO: think about this more. 

        // Can't do this earlier; merely calling FromBluetoothAddressAsync doesn't actually 
        // connect. Once we do the notify and reads the device will be connected or not.
        CurrSaveData?.History.UpdateConnectionHistory(DateTimeOffset.Now, Device.ble.ConnectionStatus);
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

        // Update the saved data in the HistoricalEnvironment_DataUnits to match the new user preferences.
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
                run.Text = Sparkles[NPropertyChanges[potentialMatchName] % Sparkles.Count];
            }
        }
    }

#if NEVER_EVER_DEFINED
    Dictionary<string, int> NPropertyChanges { get; } = [];
    //List<string> Sparkles = new List<string>() { "\u00A0", "*" }; // ✨", "💫", "🌟", "⚡", "🔥", "💥" };
    readonly List<string> Sparkles = [ "╺", "╼", "╾", "╸", "╾", "╼" ]; 

    private void UpdateSparkles(string name)
    {
        // In practice, name is never "*". The code is set up this way to match the Govee code.
        if (name == "") return;
        NPropertyChanges[name] = NPropertyChanges.GetValueOrDefault(name, 0) + 1;


        if (name == DeviceSpecificType.Temperature_cPropertyChangedName ||  name == "*")
        {
            uiTemperature_cChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        } 
        if (name == DeviceSpecificType.Pressure_hpaPropertyChangedName ||  name == "*")
        {
            uiPressure_hpaChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }
        if (name == DeviceSpecificType.HumidityPropertyChangedName ||  name == "*")
        {
            uiHumidityChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }
        if (name == DeviceSpecificType.Air_Quality_eCOS_TVOCPropertyChangedName ||  name == "*")
        {
            uieCOSChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
            uiTVOCChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }
        if (name == DeviceSpecificType.Color_RGB_ClearPropertyChangedName ||  name == "*")
        {
            uiColorChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }

    }
#endif

    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated.
    /// </summary>
    /// <param name="name"></param>
    private void UpdateDeviceDataUX(string name)
    {
        if (Device == null) return;
        CurrBattery_Data = Device.CurrBattery_Data;
        CurrSensor_Data = Device.CurrEnvironment_Data;
        CurrSensorSecondary_Data = Device.CurrEnvironmentColor_Data;

        // name is from e.PropertyName when the Device does a PropertyChanged.

        UpdateSparkles(name);

        // Update to match the current preferred units. Will create a new CurrEnvironment_DataUnits the first time
        // it's called
        CurrSensor_DataUnits = DeviceSpecificSensorData.CopyToOrClone(CurrSensor_Data, CurrSensor_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

        // Update this historical data; this will automatically update the table and graph.
        //
        // There's two kinds of sensors: ones like the Nordic_Thingy that send each bit of data separately,
        // and ones that send all the data at once (like the Govee). 
        //
        // Track the historical data
        switch (name)
        {
            case "*": // never used, but here so it matches the Govee code.
            case DeviceSpecificType.Temperature_cPropertyChangedName:
            case DeviceSpecificType.Pressure_hpaPropertyChangedName:
            case DeviceSpecificType.HumidityPropertyChangedName:
            case DeviceSpecificType.Air_Quality_eCOS_TVOCPropertyChangedName:
                //
                // Don't add to the CurrEnvironment until we P+T+H data (technically, we don't check T)
                // That's because otherwise the graph tries to include 0  pressure on the pressure line
                // which looks really weird.
                // It would be OK to add for the table, but I'm willing to give that up in order to 
                // make the graph better.
                if (!CurrSensor_Data.IsValidPH())
                {
                    break;
                }

                var deltaInSeconds = CurrSensor_Data.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
                var verb = (deltaInSeconds > 5) ? DataCollection<DeviceSpecificSensorData>.Verb.Add : DataCollection<DeviceSpecificSensorData>.Verb.ReplaceMostRecent;
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
                        SetAxesVisibility(false);
                    }
                }
                uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines
                break;
        }

        //
        // Update the text values on the screen.
        //

        if (CurrBattery_Data != null)
        {
            if (name == "BatteryLevel" || name == "")
            {
                uiBTConnectionControl.SetBatteryLevel(CurrBattery_Data.BatteryLevel);
            }
        }

        if (CurrSensor_Data != null)
        {
            if (name == DeviceSpecificType.Temperature_cPropertyChangedName || name == "")
            {
                uiTemperature_c.Text = BluetoothWatcher.Units.Temperature.AsString(CurrSensor_DataUnits.Temperature, CurrUserPrefs.Temperature);
            }
            if (name == DeviceSpecificType.Pressure_hpaPropertyChangedName || name == "")
            {
                uiPressure_hpa.Text = BluetoothWatcher.Units.Pressure.AsString(CurrSensor_DataUnits.Pressure, CurrUserPrefs.Pressure);
            }
            if (name == DeviceSpecificType.HumidityPropertyChangedName || name == "")
            {
                uiHumidity.Text = CurrSensor_Data.Humidity.ToString("0.0") + "%";
            }
            if (name == DeviceSpecificType.Air_Quality_eCOS_TVOCPropertyChangedName || name == "")
            {
                uieCOS.Text = CurrSensor_Data.eCOS.ToString("0.0");
                uiTVOC.Text = CurrSensor_Data.TVOC.ToString("0.0");
            }
            if (name == DeviceSpecificType.Color_RGB_ClearPropertyChangedName || name == "")
            {
                var RGB = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)CurrSensorSecondary_Data.Red, (byte)CurrSensorSecondary_Data.Green, (byte)CurrSensorSecondary_Data.Blue));
                uiColor.Background = RGB; //TODO: and the 'clear' value?
            }
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

} // end of class BTNordic_ThingyControl
