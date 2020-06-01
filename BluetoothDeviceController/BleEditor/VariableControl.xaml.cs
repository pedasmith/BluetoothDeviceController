using BluetoothDeviceController.Names;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using static BluetoothDeviceController.Names.VariableDescription;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.BleEditor
{
    public sealed partial class VariableControl : UserControl
    {
        /// <summary>
        /// DataContext should always be a VariableDescription
        /// </summary>
        public VariableControl()
        {
            this.InitializeComponent();
            this.DataContextChanged += VariableControl_DataContextChanged;
        }

        private void VariableControl_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            uiPanel.Children.Clear();

            VariableDescription v = null;
            try
            {
                var kvp = (KeyValuePair<string,VariableDescription>)DataContext;
                v = kvp.Value;
            }
            catch (Exception)
            {
                return;
            }
            if (v == null) return; // should never happen.

            v.CurrValue = v.Init;
            v.CurrValueString = v.InitString;

            // Don't add this label; it merely duplicates the label on the slider or combo box. 
            // uiPanel.Children.Add(new TextBlock() { Text = v.Label };);
            switch (v.InputType)
            {
                case UiType.Slider:
                    {
                        var s = new Slider()
                        {
                            Header = v.Label ?? v.Name,
                            Minimum = v.Min,
                            Maximum = v.Max,
                            Value = v.Init,
                            Tag = v
                        };
                        s.ValueChanged += S_ValueChanged;
                        uiPanel.Children.Add(s);
                    }
                    break;
                case UiType.TextBox:
                    {
                        var tb = new TextBox()
                        {
                            Header = v.Label ?? v.Name,
                            Text = v.InitString,
                            Tag = v
                        };
                        tb.TextChanged += Tb_TextChanged;
                        uiPanel.Children.Add(tb);
                        v.CurrValueIsString = true;
                    }
                    break;
                case UiType.ComboBox:
                    {
                        var cb = new ComboBox()
                        { 
                            Header = v.Label ?? v.Name,
                            Tag = v
                        };
                        ComboBoxItem initSelected = null;
                        foreach (var (name, value) in v.ValueNames)
                        {
                            var cbi = new ComboBoxItem()
                            {
                                Content = name,
                                Tag = value,
                            };
                            cb.Items.Add(cbi);
                            if (value == v.Init || initSelected == null)
                            {
                                initSelected = cbi;
                            }

                        }
                        cb.SelectionChanged += Cb_SelectionChanged;
                        uiPanel.Children.Add(cb);
                        cb.SelectedItem = initSelected;
                    }
                    break;
            }
        }

        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            var v = (sender as FrameworkElement)?.Tag as VariableDescription;
            if (v == null) return; // should never happen.
            v.CurrValueString = (sender as TextBox)?.Text;
        }

        private void Cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var v = (sender as FrameworkElement)?.Tag as VariableDescription;
            if (v == null) return; // should never happen.
            if (e.AddedItems.Count != 1) return; // should never happen
            v.CurrValue = (double)((e.AddedItems[0] as FrameworkElement).Tag);
        }

        private void S_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            var v = (sender as FrameworkElement)?.Tag as VariableDescription;
            if (v == null) return; // should never happen.
            v.CurrValue = e.NewValue;
        }
    }
}
