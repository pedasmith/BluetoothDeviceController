# How to add a new Device Control

The device controls are the 400x400 (approx) square(ish) panels. There is one per each instance of a device (e.g., if you have two Nordic Thingy:52s, there will be two panes visible).


# Critical quality items

## Outermost panel is alway a Border named rootPanel

```
<Border Style="{StaticResource sDeviceBorder}" x:Name="rootPanel">
```


## Size is the CurrWindowSize. And there's a standard IntenralDeviceType string for logging purposes.

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


