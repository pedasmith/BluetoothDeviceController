using BluetoothProtocols;
using BluetoothWinUI3.BluetoothWinUI3Registration;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3;
#if NET8_0_OR_GREATER
#nullable disable
#endif


public sealed partial class BTNordic_ThingyControl : UserControl
{
    Nordic_Thingy Device;
    public BTNordic_ThingyControl()
    {
        InitializeComponent();
        this.DataContextChanged += BTNordic_ThingyControl_DataContextChanged;
    }

    KnownDevice kd = null;
    private void BTNordic_ThingyControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
    {
        // Will be a KnownDevice
        kd = args.NewValue as KnownDevice;
        if (kd == null) return;

        uiAddress.Text = BluetoothAddress.AsString(kd.Advertisement.Addr);

        Device = new Nordic_Thingy();
    }
}
