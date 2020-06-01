using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.BleEditor
{
    public sealed partial class CommandControl : UserControl
    {
        /// <summary>
        /// DataContext must be a Command
        /// </summary>
        public CommandControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += CommandControl_DataContextChanged;
        }

        private void CommandControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Command command = DataContext as Command;
            if (command == null) return; // should never happen
            command.InitVariables();
        }

        VariableSet Variables = new VariableSet();

        private async void OnDoCommand(object sender, RoutedEventArgs e)
        {
            ;
            Command command = DataContext as Command;
            if (command == null) return; // should never happen
            await command.DoCommand();
        }
    }
}
