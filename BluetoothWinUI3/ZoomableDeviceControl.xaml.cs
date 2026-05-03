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

#if NET8_0_OR_GREATER
#nullable disable
#endif


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3
{
    public sealed partial class ZoomableDeviceControl : UserControl
    {
        public ZoomableDeviceControl()
        {
            InitializeComponent();
        }
        UserControl DeviceControl { get; set; }

        public UserControl GetDeviceControl()
        {
            //if (uiMainPanel.Children.Count < 2) return null;
            //var retval = uiMainPanel.Children[1] as UserControl;
            return DeviceControl;
        }

        public void SetDeviceControl(UserControl control)
        {
            while (uiMainPanel.Children.Count > 1)
            {
                uiMainPanel.Children.RemoveAt(1);
            }
            if (control != null)
            {
                uiMainPanel.Children.Add(control);
            }
            DeviceControl = control;
        }

        public void ReparentDeviceControl()
        {
            uiMainPanel.Children.Add(DeviceControl);
        }

        public void UnparentDeviceControl()
        {
            uiMainPanel.Children.Remove(DeviceControl);
        }
    }
}
