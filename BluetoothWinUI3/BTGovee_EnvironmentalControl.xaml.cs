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
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.
using System.Linq;
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
public sealed partial class BTGovee_EnvironmentalControl : UserControl, IDeviceControlBasic, IDeviceControlDevice, IHandleMyBTAdvertisements
{

    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>

    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400

    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "Govee_Environmental";
    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the colleciton.
    /// </summary>
    public GoveeCollection HistoricalGoveeUnits { get; } = new GoveeCollection();
    /// <summary>
    /// The current environment data directly from the sensor (it's the original data, not a copy). The data is 
    /// always in the 'native' units (e.g., always celcius for temperature).
    /// </summary>
    Govee CurrGovee = null;

    /// <summary>
    /// Similar to CurrGoveeData , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalGoveeUnits collection.
    /// </summary>
    Govee CurrGoveeUnits = null;


    /// <summary>
    /// There are multiple Govee sensors that this one control can handle.
    /// </summary>
    Govee.SensorType GoveeSensorType = Govee.SensorType.NotGovee; // Initialize as not a Govee at all.


    public BTGovee_EnvironmentalControl()
    {
        InitializeComponent();
        this.Loaded += BTGovee_EnvironmentalControl_Loaded;
        this.DataContextChanged += BTGovee_EnvironmentalControl_DataContextChanged;

        //
        // Set up the OxyModel Series. Reminder that each series is, e.g., "Temperature" or "Pressure"
        // This can't be done at initialization time because of C#: it won't let me use a regular
        // field when doing an initialization.
        foreach (var series in OxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                lineSeries.ItemsSource = HistoricalGoveeUnits.Data; //DOC:
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


    private void BTGovee_EnvironmentalControl_Loaded(object sender, RoutedEventArgs e)
    {
        // Loaded gets called first when it's first loaded an then each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        if (uiTableView.ItemsSource != null) return;
        uiTableView.ItemsSource = HistoricalGoveeUnits.Data;
        uiDeviceName.Text = "Govee " + GoveeSensorType.ToString();
    }


    // TODO: should these be discoverable? Maybe from the Model which already has the user friendly names?
    public List<string> LineNames { get { return ["Temperature", "Humidity", "PM25",]; }  }

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
                    PositionTier = 0,
                    Title="Temperature",
                    Key="Temperature"
                },
                new LinearAxis
                {
                    Position = AxisPosition.Left,
                    PositionTier = 1,
                    Title="Humidity",
                    Key="Humidity"
                },
                new LinearAxis
                {
                    Position = AxisPosition.Right,
                    PositionTier = 1,
                    Title="PM25",
                    Key="PM25"
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
                    Title = "PM25",
                    Color = OxyColors.Gray,
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "PM25",
                    YAxisKey= "PM25",
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
    private async void BTGovee_EnvironmentalControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
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
        // for connecting via BluetoothLE to a device. But this Govee display is driven entirely by advertisements,
        // and the device is not needed.
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

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        InitializeKeyLineColorsFromDefaultOxyPlot();


        var saveData = AllSaveData.FindWithId(DataContextAsKnownDevice.Id);
        if (saveData != null)
        {
            UpdateUX(saveData);
        }

        // Initialize data values
        GoveeSensorType = Govee.AdvertIsGovee(DataContextAsKnownDevice.Advertisement);
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

        // Update the saved data in the HistoricalGoveeUnits to match the new user preferences.
        foreach (var data in HistoricalGoveeUnits.Data)
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

        UpdateGraphData(""); // the graph is changed, but not the data
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
            if (this.IsLoaded) // Won't be loaded when we exit the app!
            {
                UpdateGraphData(e.PropertyName);
            }
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
    /// needs to be updated. For the Govee, there's never just one piece of data; we either get it all 
    /// or just the one.
    /// </summary>
    /// <param name="name"></param>
    private void UpdateGraphData(string name)
    {
        if (CurrGovee == null) return;

        // name is from e.PropertyName when the Device does a PropertyChanged.

        UpdateSparkles(name);

        // Update to match the current preferred units. Will create a new CurrGoveeUnits the first time
        // it's called
        CurrGoveeUnits = Govee.CopyAndUpdateUnits(CurrGovee, CurrGoveeUnits, CurrUserPrefs);

        // Track the historical data
        switch (name)
        {
            case "*": // All the data changed. This is what always happens with the govee.
            case SensorDataRecord.TemperaturePropertyChangedName:
            case SensorDataRecord.PM25PropertyChangedName:
            case SensorDataRecord.HumidityPropertyChangedName:
                // Unlike the Nordic_Thingy52 where the different values come in at different
                // time, the Govee comes in all at once.
                var deltaInSeconds = CurrGovee.TimestampMostRecent.Subtract(HistoricalGoveeUnits.TimestampMostRecentAdd).TotalSeconds;
                var verb = (deltaInSeconds > 5) ? GoveeCollection.Verb.Add : GoveeCollection.Verb.ReplaceMostRecent;
                HistoricalGoveeUnits.Update(CurrGoveeUnits, verb); // Will add or replace the data and will copy as needed.

                //
                // Update the OxyPlot because it doesn't tracked the INotifyCollectionChanged
                //
                if (verb == GoveeCollection.Verb.Add && HistoricalGoveeUnits.Count == 2)
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



        if (CurrGovee != null)
        {
            if (name == SensorDataRecord.TemperaturePropertyChangedName || name == "" || name == "*")
            {
                uiTemperature.Text = BluetoothWatcher.Units.Temperature.AsString(CurrGoveeUnits.Temperature, CurrUserPrefs.Temperature);
            }
            if (name == SensorDataRecord.PM25PropertyChangedName || name == "" || name == "*")
            {
                uiPM25.Text = CurrGovee.PM25.ToString("0.0");
            }
            if (name == SensorDataRecord.HumidityPropertyChangedName || name == "" || name == "*")
            {
                uiHumidity.Text = CurrGovee.Humidity.ToString("0.0") + "%";
            }
            if (name == "BatteryLevel" || name == "BatteryInPercent" || name == "" || name == "*") // TODO: which is right?
            {
                uiBTConnectionControl.SetBatteryLevel(CurrGovee.BatteryInPercent);
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

    public string ExportData(IExportData exporter)
    {
        string retval = "";
        var data = HistoricalGoveeUnits.Data;
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
        retval = exporter.Export($"Data from {name} at {now}");
        return retval;
    }

    string name = "Govee";
    bool FirstCallWithIsValid = true;
    public void HandleMyAdvertisement(WatcherData data)
    {
        UIThreadHelper.CallOnUIThread(() =>
        {
            if (this.IsLoaded) // Won't be loaded when we exit the app!
            {
                name = data.BestName;
                CurrGovee = Govee.Parse(GoveeSensorType, data, CurrGovee);
                if (CurrGovee == null)
                {
                    Log($"ERROR: unable to parse Govee data for sensor type {GoveeSensorType}");
                    return;
                }
                CurrGovee.EventTime = data.MostRecentAdvertisement.Timestamp;
                if (CurrGovee.IsValid)
                {
                    if (FirstCallWithIsValid)
                    {
                        // Only display valid values
                        int row = 0;
                        int col = 0;
                        // These must be in the same order they they should be visible in
                        AdjustSensor(uiSensorTemperature, SensorDataRecord.SensorPresent.Temperature, ref row, ref col);
                        AdjustSensor(uiSensorHumidity, SensorDataRecord.SensorPresent.Humidity, ref row, ref col);
                        AdjustSensor(uiSensorPM25, SensorDataRecord.SensorPresent.PM25, ref row, ref col);
                        AdjustSensor(uiSensorPressure, SensorDataRecord.SensorPresent.Pressure, ref row, ref col);

                        FirstCallWithIsValid = false;
                    }
                    // Lots of reasons it might be invalid
                    UpdateGraphData("*"); // Update all the data!
                }
            }
        });

    }

    private void AdjustSensor(Panel panel, SensorDataRecord.SensorPresent flag, ref int row, ref int col)
    {
        if (CurrGovee.IsSensorPresent.HasFlag(flag)) // e.g., SensorDataRecord.SensorPresent.Temperature
        {
            Grid.SetRow(panel, row); // panel is e.g., uiSensorTemperature
            Grid.SetColumn(panel, col++);
            if (col >= 3) { row++; col = 0; }
        }
        else
        {
            panel.Visibility = Visibility.Collapsed;
        }
    }

    #endregion

} // end of class BTGovee_EnvironmentalControl
