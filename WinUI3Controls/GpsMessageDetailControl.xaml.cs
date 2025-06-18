using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using TestNmeaGpsParserWinUI;
using Windows.Foundation;
using Windows.Foundation.Collections;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinUI3Controls
{
    public sealed partial class GpsMessageDetailControl : UserControl
    {
        public GpsMessageDetailControl()
        {
            this.DataContext = null;
            this.InitializeComponent();
        }
    }
}
