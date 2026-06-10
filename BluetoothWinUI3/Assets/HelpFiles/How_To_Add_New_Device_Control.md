# How to add a new Device Control

The device controls are the 400x400 (approx) square(ish) panels. There is one per each instance of a device (e.g., if you have two Nordic Thingy:52s, there will be two panes visible).

It's important to distinguish

- **Supported** devices are like a Nordic Thingy:53. This code knows about the Nordic Thingy.
- **Known** device are specific instances of a supported device. If you have several of them, you will have several instances. Each known device gets its own control in the  uiKnownDevices panel in MainWindow.


# The easy path: start with BTStandard_Demo

A cut-down sample is part of the source code! The BTStandard_Demo is a fully commented XAML and XAML.CS file that provides a complete example of connecting a device to this app.





# Critical quality items

## Outermost panel is alway a Border named rootPanel

```
<Border Style="{StaticResource sDeviceBorder}" x:Name="rootPanel">
```


## Size is the CurrWindowSize. And there's a standard InternalDeviceType string for logging purposes.

```
    /// <summary>
    /// Standard: Panel size. Set in UpdateUX from MainWindow.
    /// </summary>
    MainWindow.WindowSize CurrWindowSize = MainWindow.WindowSize.Normal; // Normal is 400x400

    /// <summary>
    /// Used for logging only
    /// </summary>
    private readonly string InternalDeviceType = "Nordic_Thingy";

```

```C#
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
```


