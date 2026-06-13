using BluetoothProtocols;
using BluetoothProtocols.NS_TAOPE_CyclingSpeedCadence;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Utilities;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Bluetooth;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using WinUI.TableView;
using static BluetoothProtocols.TAOPE_CyclingSpeedCadence;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

#if NET8_0_OR_GREATER
#nullable disable
#endif

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class TAOPE_CyclingSpeedCadenceControl : UserControl, IDeviceControlBasic, IDeviceControlDevice
{
    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>

    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400


    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "TOAPE_Cycle";
    TAOPE_CyclingSpeedCadence Device;
    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the colleciton.
    /// </summary>
    public Cycling_Speed_and_Cadence_DataCollection HistoricalSpeed_and_Cadence_DataUnits { get; } = new Cycling_Speed_and_Cadence_DataCollection(); // CHANGE:

    /// <summary>
    /// Similar to CurrBattery_Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalBattery_DataUnits collection.
    /// </summary>
    Cycling_Speed_and_Cadence_Data CurrSpeed_and_Cadence_Data = null;
    Cycling_Speed_and_Cadence_Data CurrSpeed_and_Cadence_DataUnits = null;



    public TAOPE_CyclingSpeedCadenceControl()
    {
        InitializeComponent();
        this.Loaded += TAOPE_CyclingSpeedCadenceControl_Loaded;
        this.DataContextChanged += TAOPE_CyclingSpeedCadenceControl_DataContextChanged;

        //
        // Set up the OxyModel Series. Reminder that each series is, e.g., "Temperature" or "Pressure"
        // This can't be done at initialization time because of C#: it won't let me use a regular
        // field when doing an initialization.
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                lineSeries.ItemsSource = HistoricalSpeed_and_Cadence_DataUnits.Data; //DOC:
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


    private void TAOPE_CyclingSpeedCadenceControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Loaded gets called first when it's first loaded an then each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        // We only want to do work the first time.

        if (uiTableView.ItemsSource != null) return;
        uiTableView.ItemsSource = HistoricalSpeed_and_Cadence_DataUnits.Data;
    }

    private Cycling_Speed_and_Cadence_Data CopyAndUpdateUnits(Cycling_Speed_and_Cadence_Data source, Cycling_Speed_and_Cadence_Data dest)
    {
        dest ??= source.Clone();
        // CHANGE: You might be tempted to use the dest.CopyFrom(source) at this point. But that will 
        // copy all the fields without updating the units (and will therefore trigger a bunch of INPC callbacks)
        // An easy way to fill this is in:
        // 1. Copy the dest.CopyFrom() method, changing "this" to dest and "value" to source.
        // 2. Each field that's a unit with user preferences needs to be changed to use
        //    the BluetoothWatcher.Units.(type).Convert
        //    Example (from Nordic_thingy.xaml.cs):
        //        dest.Temperature = BluetoothWatcher.Units.Temperature.Convert(source.Temperature, BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius, CurrUserPrefs.Temperature);
        // The starting units is always the units of the raw protocol. For the Nordic, the temperature
        // is always in Celcius. 
        // For this, the raw battery level is always in percent, and there's no user adjustment.

        dest.TimestampMostRecent = source.TimestampMostRecent;
        dest.Flags = source.Flags;
        dest.RevolutionWheel = source.RevolutionWheel;
        dest.TimeWheel = source.TimeWheel;
        dest.RevolutionCrank = source.RevolutionCrank;
        dest.TimeCrank = source.TimeCrank;
        dest.FeatureFlags = source.FeatureFlags;
        dest.SensorLocation = source.SensorLocation;
        dest.Unknown3 = source.Unknown3;
        return dest;
    }

    // If you have to update these dynamically, be sure to call 
    // NotifyDeviceControlChangesWindows.OnGetUXCapabilitiesChanged
    // so the main window menus get updated.

    // TODO: should these be discoverable? Maybe from the Model which already has the user friendly names?
    List<string> _LineNames = new List<string>() { "Revolution" }; // CHANGE:
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
    private PlotModel OxyPlotModel { get; set; } = new PlotModel
    {
        Title = "Cadence Data", //CHANGE:
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
                MajorStep = 10, // CHANGE: Battery percentage run 0..100
                MinimumRange= 30, // CHANGE: set this match your graphing needs
                Title="Cadence", // CHANGE: set to something the user will recognize
                Key="RevolutionA" // CHANGE: Key has to match the YAxisKey in the Series
            },
        },
        Series =
        {
            new LineSeries // CHANGE:
            {
                Title = "Revolutions",
                Color = OxyColors.DarkBlue,
                StrokeThickness = 0.75,
                MarkerType = MarkerType.None,
                DataFieldX = "TimestampMostRecentDT", // All sensor data has a TimestampMostRecentDT
                DataFieldY = "RevolutionA", // CHANGE: Must match the data in the sensor data class
                YAxisKey= "RevolutionA", // CHANGE: this key has to match the one in the Axis field.
                // Suggestion is to set the YAxisKey to be the same as the DataFieldY
            },
        }
    };
    /// <summary>
    /// Set all of the axes to either visible or invisible. 
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


    ulong OriginalBTAddr = 0xFFFFFFFF_FFFFFFFF;
    /// <summary>
    /// This is a two-way street. Setting the DataContest to the KnownDevice will update some UX and will
    /// trigger looking up the SaveData and change more things. And it will actually connect to the device.
    /// AND this will update the KnownDevice with, e.g., the DeviceId and the BluetoothLEDevice which will be
    /// used by other bits of the system.
    /// </summary>
    private async void TAOPE_CyclingSpeedCadenceControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
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

        Device = new TAOPE_CyclingSpeedCadence()
        {
            ble = await BluetoothLEDevice.FromBluetoothAddressAsync(DataContextAsKnownDevice.Advertisement.Addr),
        };
        if (Device.ble == null)
        {
            Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr)}");
            return;
        }
        // It's critical to set these!
        DataContextAsKnownDevice.Id = Device.ble.DeviceId ?? ""; // never null :-)
        DataContextAsKnownDevice.BTLEDevice = Device.ble;

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        InitializeKeyLineColorsFromDefaultOxyPlot();


        var saveData = AllSaveData.FindWithId(DataContextAsKnownDevice.Id);
        if (saveData != null)
        {
            UpdateUX(saveData);
        }


        Device.PropertyChanged += Device_PropertyChanged;
        await Device.NotifyCSC_MeasurementAsync(); // CHANGE: and the next lines
        var battery = await Device.NotifyBattery_LevelAsync(); // I'm happy getting unchanged data? TODO: think about this more. 

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
        uiDeviceName.Text = name;

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
        // TODO: eventually there will be a speed in MPH or KPH!
        foreach (var data in HistoricalSpeed_and_Cadence_DataUnits.Data)
        {
            // CHANGE: demonstrate how to change what the user sees based on unit preferences.
            // For the TAOPE_CyclingSpeedCadence, there are no units to change
            if (oldPrefs != null && newPrefs.Temperature != oldPrefs.Temperature)
            {
                // data.Temperature = BluetoothWatcher.Units.Temperature.Convert(data.Temperature, oldPrefs.Temperature, CurrUserPrefs.Temperature);
            }
            if (oldPrefs != null && newPrefs.Pressure != oldPrefs.Pressure)
            {
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
            UpdateDeviceDataUX(e.PropertyName);
        });
    }


    Dictionary<string, int> NPropertyChanges { get; } = [];
    //List<string> Sparkles = new List<string>() { "\u00A0", "*" }; // ✨", "💫", "🌟", "⚡", "🔥", "💥" };
    readonly List<string> Sparkles = ["╺", "╼", "╾", "╸", "╾", "╼"];

    private void UpdateSparkles(string name)
    {
        // In practice, name is never "*". The code is set up this way to match the Govee code.
        if (name == "") return;
        NPropertyChanges[name] = NPropertyChanges.GetValueOrDefault(name, 0) + 1;

        if (name == TAOPE_CyclingSpeedCadence.CSC_MeasurementPropertyChangedName || name == "*")
        {
            uiRevolutionWheelChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
            uiRevolutionCrankChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }
    }

    double StartingTimeWheel = 0.0;
    double LastTimeWheel = 0.0;
    double LastRevolutionWheel = 0.0;

    double StartingTimeCrank = 0.0;
    double LastTimeCrank = 0.0;
    double LastRevolutionCrank = 0.0;

    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated.
    /// </summary>
    /// <param name="name"></param>
    private void UpdateDeviceDataUX(string name)
    {
        if (Device == null) return;
        CurrSpeed_and_Cadence_Data = Device.CurrCycling_Speed_and_Cadence_Data;

        // name is from e.PropertyName when the Device does a PropertyChanged.

        UpdateSparkles(name);

        // Update to match the current preferred units. Will create a new CurrEnvironment_DataUnits the first time
        // it's called
        CurrSpeed_and_Cadence_DataUnits = CopyAndUpdateUnits(CurrSpeed_and_Cadence_Data, CurrSpeed_and_Cadence_DataUnits);

        // Update this historical data; this will automatically update the table and graph.
        //
        // There's two kinds of sensors: ones like the Nordic_Thingy that send each bit of data separately,
        // and ones that send all the data at once (like the Govee). 
        //
        // Track the historical data
        switch (name)
        {
            case TAOPE_CyclingSpeedCadence.CSC_MeasurementPropertyChangedName:

                var flags = "";
                if (((int)CurrSpeed_and_Cadence_Data.Flags & 0x01) != 0)
                {
                    flags += "Wheel";
                }
                if (((int)CurrSpeed_and_Cadence_Data.Flags & 0x02) != 0)
                {
                    flags += "Crank";
                }
                uiFlags.Text = flags;

                if (StartingTimeWheel == 0.0)
                {
                    StartingTimeWheel = CurrSpeed_and_Cadence_Data.TimeWheel;
                }
                else
                {
                    var currTimeA = CurrSpeed_and_Cadence_Data.TimeWheel - StartingTimeWheel;
                    var instananeousTimeA = currTimeA - LastTimeWheel; // Seconds since the last report.
                    uiTimeWheel.Text = currTimeA.ToString("F3"); // Time is in seconds

                    //
                    // Calculate some instantanous values
                    //
                    var instantaneousRevolutionA = LastRevolutionWheel - CurrSpeed_and_Cadence_Data.RevolutionWheel;
                    var rps = instantaneousRevolutionA * (1.0 / instananeousTimeA);
                    uiRpsWheel.Text = rps.ToString("F0"); // Time is in seconds

                    LastTimeWheel = currTimeA;
                }
                uiRevolutionWheel.Text = CurrSpeed_and_Cadence_Data.RevolutionWheel.ToString("F0");
                LastRevolutionWheel = CurrSpeed_and_Cadence_Data.RevolutionWheel;


                var deltaInSeconds = CurrSpeed_and_Cadence_Data.TimestampMostRecent.Subtract(HistoricalSpeed_and_Cadence_DataUnits.TimestampMostRecentAdd).TotalSeconds;
                var verb = (deltaInSeconds > 5) ? Cycling_Speed_and_Cadence_DataCollection.Verb.Add : Cycling_Speed_and_Cadence_DataCollection.Verb.ReplaceMostRecent;
                HistoricalSpeed_and_Cadence_DataUnits.Update(CurrSpeed_and_Cadence_DataUnits, verb); // Will add or replace the data and will copy as needed.

                //
                // Update the OxyPlot because it doesn't track the INotifyCollectionChanged
                //
                if (verb == Cycling_Speed_and_Cadence_DataCollection.Verb.Add && HistoricalSpeed_and_Cadence_DataUnits.Count == 2)
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

        if (CurrSpeed_and_Cadence_Data != null)
        {
            if (name == "BatteryLevel" || name == "")
            {
                // TODO: uiBTConnectionControl.SetBatteryLevel(CurrBattery_Data.BatteryLevel);
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
        var retval = IDeviceControlBasic.UXCapabilities.CanRename
            | IDeviceControlBasic.UXCapabilities.CanGetGraphAsPng
            | IDeviceControlBasic.UXCapabilities.CanGetData
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


    public string ExportData(IExportData exporter)
    {
        string retval = "";
        var data = HistoricalSpeed_and_Cadence_DataUnits.Data;
        if (data.Count == 0)
        {
            Log("No data to export.");
            return retval;
        }
        data[0].ExportHeaders(exporter);
        foreach (var row in data)
        {
            row.ExportRow(exporter);
        }
        var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        retval = exporter.Export($"Data from {Device.Name} at {now}");
        return retval;
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

} // end of class TAOPE_CyclingSpeedCadenceControl
