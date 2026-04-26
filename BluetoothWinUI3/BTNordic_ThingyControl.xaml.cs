using BluetoothProtocols;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis; // Required for the DynamicallyAccessedMembers attribute needed for trimming to not fail.
using Utilities;
using Windows.Devices.Bluetooth;
using static System.Runtime.InteropServices.JavaScript.JSType;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif

public interface IDeviceControl
{
    /// <summary>
    /// Returns the KnownDevice associated with the device
    /// </summary>
    /// <returns></returns>
    KnownDevice GetKnownDevice();

    /// <summary>
    /// updates the device control user interface base on the user preferences in SaveData.
    /// As of 2026-04-05 this includes just the name
    /// </summary>
    /// <param name="saveData"></param>
    void UpdateUX(SaveData saveData);

    void UpdateUX(UserPreferences userprefs);

}

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public sealed partial class BTNordic_ThingyControl : UserControl, IDeviceControl
{
    /// <summary>
    /// Used for logging only
    /// </summary>
    private string InternalDeviceType = "Nordic_Thingy";
    Nordic_Thingy Device;
    BluetoothProtocols.NS_Nordic_Thingy.Environment_DataCollection HistoricalEnvironment_Data 
        = new BluetoothProtocols.NS_Nordic_Thingy.Environment_DataCollection();

    Nordic_Thingy.Environment_Data CurrEnvironment_Data = null;
    Nordic_Thingy.Battery_Data CurrBattery_Data = null;
    Nordic_Thingy.EnvironmentColor_Data CurrEnvironmentColor_Data = null;

    public BTNordic_ThingyControl()
    {
        InitializeComponent();
        this.DataContextChanged += BTNordic_ThingyControl_DataContextChanged;
        this.Loaded += BTNordic_ThingyControl_Loaded;
    }

    private void BTNordic_ThingyControl_Loaded(object sender, RoutedEventArgs e)
    {
        (OxyPlotModel.Series[0] as LineSeries).ItemsSource = HistoricalEnvironment_Data.Data; //DOC:
        (OxyPlotModel.Series[1] as LineSeries).ItemsSource = HistoricalEnvironment_Data.Data; //DOC:
        (OxyPlotModel.Series[2] as LineSeries).ItemsSource = HistoricalEnvironment_Data.Data; //DOC:
        uiOxyPlot.Model = OxyPlotModel; // DOC:
    }

    KnownDevice KnownDeviceFromDataContext { get { return DataContext as KnownDevice; } }
    public KnownDevice GetKnownDevice()
    {
        return DataContext as KnownDevice;
    }

    // H.Oxyplot
    private PlotModel OxyPlotModel { get; set; } = new PlotModel
    {
        Title = "Environment Data",
        PlotAreaBorderColor = OxyColors.Transparent,
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
            },
        Series =
            {
                new LineSeries
                {
                    Title = "Temperature",
                    MarkerType = MarkerType.Circle,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "Temperature",
                    YAxisKey= "Temperature",
                },
                new LineSeries
                {
                    Title = "Pressure",
                    MarkerType = MarkerType.Circle,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "Pressure",
                    YAxisKey= "Pressure",
                },
                new LineSeries
                {
                    Title = "Humidity",
                    MarkerType = MarkerType.Circle,
                    DataFieldX = "TimestampMostRecentDT",
                    DataFieldY = "Humidity",
                    YAxisKey= "Humidity",
                },
            }
    };

    /// <summary>
    /// This is a two-way street. Setting the DataContest to the KnownDevice will update some UX and will
    /// trigger looking up the SaveData and change more things. And it will actually connect to the device.
    /// AND this will update the KnownDevice with, e.g., the DeviceId and the BluetoothLEDevice which will be
    /// used by other bits of the system.
    /// </summary>
    private async void BTNordic_ThingyControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // Set up the OxyPlot model
        uiOxyPlot.Model = OxyPlotModel;

        // FYI: by the time this method is called, the DataContext is already set

        if (args.NewValue == null) return; // just bogus; ignore.

        // Must have been set as a KnownDevice; otherwise we're in a very weird state.
        if (KnownDeviceFromDataContext == null)
        {
            Log($"Impossible Error: {InternalDeviceType}: Data context change, but it's not a KnownDevice. Type is {args.NewValue.GetType()}");
            return;
        }

        uiAddress.Text = BluetoothAddress.AsString(KnownDeviceFromDataContext.Advertisement.Addr);

        Device = new Nordic_Thingy();

        Device.ble = await BluetoothLEDevice.FromBluetoothAddressAsync(KnownDeviceFromDataContext.Advertisement.Addr);
        if (Device.ble == null)
        {
            Log($"Error: {InternalDeviceType}: Unable to get BLE from {BluetoothAddress.AsString(KnownDeviceFromDataContext.Advertisement.Addr)}");
            return;
        }
        // It's critical to set these!
        KnownDeviceFromDataContext.Id = Device.ble.DeviceId ?? ""; // never null :-)
        KnownDeviceFromDataContext.BTLEDevice = Device.ble;


        var saveData = AllSaveData.FindWithId(KnownDeviceFromDataContext.Id);
        if (saveData != null)
        {
            UpdateUX(saveData);
        }


        Device.PropertyChanged += Device_PropertyChanged;
        await Device.NotifyBatteryLevelAsync();
        await Device.NotifyTemperature_cAsync();
        await Device.NotifyPressure_hpaAsync();
        await Device.NotifyHumidityAsync();
        await Device.NotifyAir_Quality_eCOS_TVOCAsync(); // both TVOC and eCOS
        await Device.NotifyColor_RGB_ClearAsync();
        await Device.ReadBatteryLevel(BluetoothCacheMode.Cached); // I'm happy getting unchanged data? TODO: think about this more. 
    }

    public void UpdateUX(SaveData saveData)
    {
        if (saveData != null)
        {
            var name = saveData.GetUserName();
            uiDeviceName.Text = name;

            var theme = Application.Current.RequestedTheme;
            var colors = saveData.GetDeviceColors(Application.Current.RequestedTheme);
            var brushes = new DeviceColorBrushes(colors);
            DeviceColorBrushes.SetUxColors(this.rootPanel, brushes);
        }
    }

    public void UpdateUX(UserPreferences userprefs)
    {
        CurrUserPrefs = userprefs;
        UpdateUI(""); // all of them. // TODO: I really made both an UpdateUI and an UpdateUX? Wrrong, bucko!
    }

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
            if (this.IsLoaded) // Won't be loaded when we exit the app!
            {
                UpdateUI(e.PropertyName);
            }
        });
    }


    Dictionary<string, int> NPropertyChanges = new Dictionary<string, int>();
    //List<string> Sparkles = new List<string>() { "\u00A0", "*" }; // ✨", "💫", "🌟", "⚡", "🔥", "💥" };
    List<string> Sparkles = new List<string>() { "╺", "╼", "╾", "╸", "╾", "╼" }; 
    private void UpdateUI(string name)
    {
        if (Device == null) return;
        CurrBattery_Data = Device.CurrBattery_Data;
        CurrEnvironment_Data = Device.CurrEnvironment_Data;
        CurrEnvironmentColor_Data = Device.CurrEnvironmentColor_Data;

        // from e.PropertyName when the Device does a PropertyChanged.
        if (name != "")
        {
            NPropertyChanges[name] = NPropertyChanges.GetValueOrDefault(name, 0) + 1;
            switch (name)
            {
                case "": break;
                case Nordic_Thingy.Temperature_cPropertyChangedName: uiTemperature_cChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count]; break;
                case Nordic_Thingy.Pressure_hpaPropertyChangedName: uiPressure_hpaChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count]; break;
                case Nordic_Thingy.HumidityPropertyChangedName: uiHumidityChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count]; break;
                case Nordic_Thingy.Air_Quality_eCOS_TVOCPropertyChangedName:
                    uieCOSChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
                    uiTVOCChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count];
                    break;
                case Nordic_Thingy.Color_RGB_ClearPropertyChangedName: uiColorChange.Text = Sparkles[NPropertyChanges[name] % Sparkles.Count]; break;
            }
        }

        // Track the historical data
        switch (name)
        {
            case Nordic_Thingy.Temperature_cPropertyChangedName:
            case Nordic_Thingy.Pressure_hpaPropertyChangedName:
            case Nordic_Thingy.HumidityPropertyChangedName:
            case Nordic_Thingy.Air_Quality_eCOS_TVOCPropertyChangedName:
                var deltaInSeconds = CurrEnvironment_Data.TimestampMostRecent.Subtract(HistoricalEnvironment_Data.TimestampMostRecentAdd).TotalSeconds;
                if (deltaInSeconds > 5)
                {
                    HistoricalEnvironment_Data.Add(CurrEnvironment_Data); // Will copy the data as needed.
                    uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines
                }
                else
                {
                    HistoricalEnvironment_Data.ReplaceMostRecent(CurrEnvironment_Data); // Will copy the data as needed.
                    uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines
                }
                break;
        }

        switch (name)
        {
            case Nordic_Thingy.Temperature_cPropertyChangedName:
                //TempSeries.Points.Add(new OxyPlot.DataPoint(HistoricalEnvironment_Data.Temperature.Count, CurrEnvironment_Data.Temperature));
                break;
        }

        if (CurrBattery_Data != null)
        {
            if (name == "BatteryLevel" || name == "")
            {
                uiBTConnectionControl.SetBatteryLevel(CurrBattery_Data.BatteryLevel);
            }
        }

        if (CurrEnvironment_Data != null)
        {
            if (name == Nordic_Thingy.Temperature_cPropertyChangedName || name == "")
            {
                uiTemperature_c.Text = BluetoothWatcher.Units.Temperature.ConvertToString(CurrEnvironment_Data.Temperature, BluetoothWatcher.Units.Temperature.TemperatureUnit.Celcius, CurrUserPrefs.Temperature);
            }
            if (name == Nordic_Thingy.Pressure_hpaPropertyChangedName || name == "")
            {
                uiPressure_hpa.Text = BluetoothWatcher.Units.Pressure.ConvertToString(CurrEnvironment_Data.Pressure, BluetoothWatcher.Units.Pressure.PressureUnit.hectoPascal_milliBar, CurrUserPrefs.Pressure);
            }
            if (name == Nordic_Thingy.HumidityPropertyChangedName || name == "")
            {
                uiHumidity.Text = CurrEnvironment_Data.Humidity.ToString("0.0") + "%";
            }
            if (name == Nordic_Thingy.Air_Quality_eCOS_TVOCPropertyChangedName || name == "")
            {
                uieCOS.Text = CurrEnvironment_Data.eCOS.ToString("0.0");
                uiTVOC.Text = CurrEnvironment_Data.TVOC.ToString("0.0");
            }
            if (name == Nordic_Thingy.Color_RGB_ClearPropertyChangedName || name == "")
            {
                var RGB = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)CurrEnvironmentColor_Data.Red, (byte)CurrEnvironmentColor_Data.Green, (byte)CurrEnvironmentColor_Data.Blue));
                uiColor.Background = RGB; //TODO: and the 'clear' value?
            }
        }

#if NEVER_EVER_DEFINED
        switch (name) 
        {
            case "Temperature_c":
                break;
            case "Pressure_hpa":
                uiPressure_hpa.Text = CurrEnvironment_Data.Pressure_hpa.ToString("0.0");
                break;
            case "Humidity":
                uiHumidity.Text = CurrEnvironment_Data.Humidity.ToString("0.0") + "%";
                break;
            case "TVOC": // sets both TVOC and eCOS
                uieCOS.Text = CurrEnvironment_Data.eCOS.ToString("0.0");
                uiTVOC.Text = CurrEnvironment_Data.TVOC.ToString("0.0");
                break;
            case "Color":
                {
                    var RGB = new SolidColorBrush(Windows.UI.Color.FromArgb(255, (byte)Device.Red, (byte)Device.Green, (byte)Device.Blue));
                    uiColor.Background = RGB; //TODO: and the 'clear' value?
                }
                break;
        }
#endif
    }
}
