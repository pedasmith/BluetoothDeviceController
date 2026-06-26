using BluetoothProtocols;
using BluetoothProtocols.NS_BTStandard_HeartRate;
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

using DeviceSpecificBatteryData = BTStandard_HeartRate.Battery_Data; // Change: many device support battery
using ThisControlData = BTStandard_HeartRate.Heart_Rate_Data;
using ThisControlDataCollection = Heart_Rate_DataCollection;
using ThisControlDevice = BTStandard_HeartRate;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTStandard_HeartRateControl : UserControl, IDeviceControlBasic, IDeviceControlDevice
{
    bool HasData = true; // Data is the BatteryLevel data. But it might not exist.

    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>

    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400


    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "BTStandard_HeartRate"; //CHANGE:

    BTStandard_HeartRate Device; // CHANGE:
    string KnownDeviceName = "device";
    SaveData CurrSaveData = null;
    ulong OriginalBTAddr = 0xFFFFFFFF_FFFFFFFF;

    /// <summary>
    /// Collection of data from the sensor. This is all a copy and will be in the user's preferred units.
    /// The units are set right before the data is added to the colleciton.
    /// </summary>
    public ThisControlDataCollection HistoricalDataUnits { get; } = new (); // CHANGE:
    public IReadOnlyList<IBTCommonMetaData> GetDataAll() { return HistoricalDataUnits.Data; }
    public IBTCommonMetaData GetDataMostRecent()
    { 
        return HistoricalDataUnits.Count == 0 ? null : HistoricalDataUnits.Data[HistoricalDataUnits.Count - 1]; 
    }



    /// <summary>
    /// Sensor data from the device. Units are in the units that the device
    /// publishes. For example, a pressure monitor might always provide data
    /// in "hPa" (hecto-Pascals) regardless of what the user wants to see.
    /// </summary>
    ThisControlData CurrHeart_Rate_Data = null;
    /// <summary>
    /// Similar to CurrHeart_Rate_Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    ThisControlData CurrHeart_Rate_DataUnits = null;

    /// <summary>
    /// Current battery data from the Device
    /// </summary>
    BTStandard_HeartRate.Battery_Data CurrBattery_Data = null;
    /// <summary>
    /// Similar to CurrBattery_Data , but the values are converted to the user's preferred units. 
    /// This is what gets added to the HistoricalDataUnits collection.
    /// </summary>
    BTStandard_HeartRate.Battery_Data CurrBattery_DataUnits = null;

    public BTStandard_HeartRateControl()
    {
        InitializeComponent();
        this.Loaded += Control_Loaded;
        this.DataContextChanged += Control_DataContextChanged;

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


    private void Control_Loaded(object sender, RoutedEventArgs e)
    {
        // Loaded gets called first when it's first loaded an then each time it's 
        // attached to somewhere else (e.g., when the control is made large and then small)
        // We only want to do work the first time.

        if (uiTableView.ItemsSource != null) return;
        uiTableView.ItemsSource = HistoricalDataUnits.Data;
    }


    // If you have to update these dynamically, be sure to call 
    // NotifyDeviceControlChangesWindows.OnGetUXCapabilitiesChanged
    // so the main window menus get updated.

    // TODO: should these be discoverable? Maybe from the Model which already has the user friendly names?
    List<string> _LineNames = new List<string>() { "PulseRate" }; // CHANGE:
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
        Title = "Heart Rate", //CHANGE:
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
                    Title="Pulse Rate", // CHANGE: set to something the user will recognize
                    Key="PulseRate" // CHANGE: Key has to match the YAxisKey in the Series
                },
            },
        Series =
            {
                new LineSeries // CHANGE:
                {
                    Title = "Pulse",
                    Color = OxyColors.DarkBlue,
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT", // All sensor data has a TimestampMostRecentDT
                    DataFieldY = "PulseRate", // CHANGE: Must match the data in the sensor data class
                    YAxisKey= "PulseRate", // CHANGE: this key has to match the one in the Axis field.
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
        if (OriginalBTAddr != 0xFFFFFFFF_FFFFFFFF)
        {
            ; // duplicate call!
            return;
        }
        OriginalBTAddr = DataContextAsKnownDevice.Advertisement.Addr;
        uiAddress.Text = BluetoothAddress.AsString(DataContextAsKnownDevice.Advertisement.Addr);
        CurrSaveData = AllSaveData.FindWithAdvertisementAddress(DataContextAsKnownDevice.Advertisement.Addr); // might return null for the first connection

        Device = new BTStandard_HeartRate()
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
        CurrSaveData = AllSaveData.FindWithId(DataContextAsKnownDevice.Id); // now it's "guaranteed" to exist. Use the stable form of the device id.

        // Initialize the line colors from the default colors in the OxyPlotModel.
        // This will get over-ridden with the data from the saveData
        InitializeKeyLineColorsFromDefaultOxyPlot();

        UpdateUX(CurrSaveData); // Shouldn't be null, but also handles null gracefully.

        Device.PropertyChanged += Device_PropertyChanged;
        await Device.NotifyBatteryLevelAsync(); // CHANGE: and the next lines
        var battery = await Device.ReadBatteryLevel(BluetoothCacheMode.Cached); // I'm happy getting unchanged data? TODO: think about this more. 

        // CHANGE: for your particular device, the battery might always be present
        // the battery will never be null.
        if (battery == null)
        {
            // Happens when the device doesn't report a battery level (e.g., JBL Pro 4 Speakers, but lots of others)
            // BTW: if you know your device will never have a battery level but there is a connection control,
            // you should just set the visibility to collapsed in the OnLoaded event.

            HasData = false;
            LineNames.Clear();
            uiDeviceDataList.Items.Remove(uiDeviceDataBattery);
            uiBTConnectionControl.SetBatteryVisibility(Visibility.Collapsed);
            uiOxyPlot.Visibility = Visibility.Collapsed;
            uiTableView.Visibility = Visibility.Visible;

            // Notify MainWindow that the UX capabilities have changed. This might change
            // the UX (e.g., device> show graph/table might be removed)
            // Will also trigger redoing the graph line names via LineNames, which
            // technically isn't quite in accordance with the name.
            NotifyDeviceControlChangesWindows?.OnGetUXCapabilitiesChanged(this, GetUXCapabilities());
        }
        await Device.ReadDevice_Name(BluetoothCacheMode.Cached);
        await Device.NotifyHeart_Rate_MeasurementAsync();

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
            CurrHeart_Rate_DataUnits?.Name = KnownDeviceName;
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
            // CHANGE: demonstrate how to change what the user sees based on unit preferences.
            // For the BTStandard_HeartRate, there are no units to change
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
            CurrSaveData?.History.UpdateDataHistory(DateTimeOffset.Now);
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

        if (name == BTStandard_HeartRate.BatteryLevelPropertyChangedName || name == "*")
        {
            uiBatteryLevelChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }
        if (name == BTStandard_HeartRate.Heart_Rate_MeasurementPropertyChangedName || name == "*")
        {
            uiFlagsChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
        }

    }

    /// <summary>
    /// Called either when we have a single new data value (e.g., "Temperature") or when all the data
    /// needs to be updated.
    /// </summary>
    /// <param name="name"></param>
    private void UpdateDeviceDataUX(string name)
    {
        if (Device == null) return;
        CurrBattery_Data = Device.CurrBattery_Data;
        CurrHeart_Rate_Data = Device.CurrHeart_Rate_Data;

        // name is from e.PropertyName when the Device does a PropertyChanged.

        UpdateSparkles(name);

        // Update to match the current preferred units. Will create a new CurrEnvironment_DataUnits the first time
        // it's called
        CurrBattery_DataUnits = DeviceSpecificBatteryData.CopyToOrClone(CurrBattery_Data, CurrBattery_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);
        CurrHeart_Rate_DataUnits = ThisControlData.CopyToOrClone(CurrHeart_Rate_Data, CurrHeart_Rate_DataUnits, KnownDeviceName, CurrUserPrefs.Convert);

        // Update this historical data; this will automatically update the table and graph.
        //
        // There's two kinds of sensors: ones like the Nordic_Thingy that send each bit of data separately,
        // and ones that send all the data at once (like the Govee). 
        //
        // Track the historical data
        switch (name)
        {
            //            case BTStandard_HeartRate.Device_NamePropertyChangedName:
            //                uiName.Text = CurrCommon_Configuration_Data.Device_Name;
            //                break;

            case "*": // never used, but here so it matches the Govee code.
            case BTStandard_HeartRate.Heart_Rate_MeasurementPropertyChangedName:
                uiFlags.Text = ((int)CurrHeart_Rate_DataUnits.Flags).ToString("X2");
                uiBPM.Text = CurrHeart_Rate_DataUnits.PulseRate.ToString();
                uiBPMHighRes.Text = CurrHeart_Rate_DataUnits.PulseRateHighRes.ToString();
                uiRRInterval.Text = CurrHeart_Rate_DataUnits.RRInterval.ToString();


                var deltaInSeconds = CurrHeart_Rate_DataUnits.TimestampMostRecent.Subtract(HistoricalDataUnits.TimestampMostRecentAdd).TotalSeconds;
                var verb = (deltaInSeconds > 5) ? ThisControlDataCollection.Verb.Add : ThisControlDataCollection.Verb.ReplaceMostRecent;
                HistoricalDataUnits.Update(CurrHeart_Rate_DataUnits, verb); // Will add or replace the data and will copy as needed.

                //
                // Update the OxyPlot because it doesn't track the INotifyCollectionChanged
                //
                if (verb == ThisControlDataCollection.Verb.Add && HistoricalDataUnits.Count == 2)
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

            case BTStandard_HeartRate.BatteryLevelPropertyChangedName:
                uiBattery.Text = CurrBattery_DataUnits.BatteryLevel.ToString("F2");
                break;
        }

        //
        // Update the text values on the screen.
        //

        if (CurrBattery_Data != null)
        {
            if (name == "BatteryLevel" || name == "")
            {
                uiBTConnectionControl.SetBatteryLevel(CurrBattery_DataUnits.BatteryLevel);
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

        var retval = IDeviceControlBasic.UXCapabilities.CanRename;
        if (HasData)
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

} // end of class BTStandard_HeartRateControl
