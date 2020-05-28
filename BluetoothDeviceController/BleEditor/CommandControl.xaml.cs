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

            //TODO: init with previous values???
            foreach (var (name, newValue) in command.Set)
            {
                CurrValues[name] = newValue;
            }
            foreach (var (name, variableDescription) in command.Parameters)
            {
                CurrValues[name] = variableDescription.Init;
            }
        }

        Dictionary<string, double> CurrValues = new Dictionary<string, double>();
        Dictionary<string, double> PrevValues = new Dictionary<string, double>();

        private async void OnDoCommand(object sender, RoutedEventArgs e)
        {
            ;
            Command command = DataContext as Command;
            if (command == null) return; // should never happen

            foreach (var (name, variableDescription) in command.Parameters)
            {
                CurrValues[name] = variableDescription.CurrValue;
            }

            // TODO: move all this to a common library -- it's shared between here
            // and the serial class.
            var list = command.Compute.Split(new char[] { ' ' });
            var cmd = "";
            foreach (var strcommand in list)
            {
                var calculateResult = BleEditor.ValueCalculate.Calculate(strcommand, double.NaN, null, PrevValues, CurrValues);
                cmd += calculateResult.S ?? calculateResult.D.ToString();
            }
            foreach (var (name, value) in CurrValues)
            {
                PrevValues[name] = value;
            }

            // Got the command to send! now what?
            var result = await command.WriteCharacteristic.DoWriteString(cmd);
            ;
        }
    }
}
