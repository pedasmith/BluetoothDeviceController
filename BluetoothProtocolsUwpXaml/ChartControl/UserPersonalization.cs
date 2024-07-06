using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace BluetoothProtocolsUwpXaml.ChartControl
{
    public class UserPersonalization

    {
        public static UserPersonalization Current { get; set; }
        public enum Item { None, ChartBackground, ThinCursor, Wave1, Wave2, Wave3, Wave4, ReticuleMajor, ReticuleMinor };
        public Brush GetBrush(Item item) { return AllBrushes[(int)item]; }
        public double GetThickness(Item item) { return AllThickness[(int)item]; }

        public static void Init()
        {
            if (Current != null) return;

            var pref = new UserPersonalization();
            pref.SetColor(Item.ChartBackground, Colors.DarkGray);
            pref.SetColor(Item.ThinCursor, Colors.White);
            pref.SetColor(Item.Wave1, Colors.Green);
            pref.SetColor(Item.Wave2, Colors.Cyan);
            pref.SetColor(Item.Wave3, Colors.Blue);
            pref.SetColor(Item.Wave4, Colors.DarkGoldenrod);
            pref.SetColor(Item.ReticuleMajor, Colors.LimeGreen);
            pref.SetColor(Item.ReticuleMinor, Colors.LimeGreen);

            pref.SetThickness(Item.ThinCursor, 1.0);
            pref.SetThickness(Item.Wave1, 3.0);
            pref.SetThickness(Item.Wave2, 3.0);
            pref.SetThickness(Item.Wave3, 3.0);
            pref.SetThickness(Item.Wave4, 3.0);
            pref.SetThickness(Item.ReticuleMajor, 2.0);
            pref.SetThickness(Item.ReticuleMinor, 1.0);

            Current = pref;
        }
        public void SetColor(Item item, Color color)
        {
            AllColors[(int)item] = color;
            AllBrushes[(int)item] = new SolidColorBrush(color);
        }
        public void SetThickness(Item item, double value)
        {
            AllThickness[(int)item] = value;    
        }



        public UserPersonalization()
        {
            while (AllThickness.Count < AllThickness.Capacity)
            {
                AllThickness.Add(1.0);
            }

            while (AllColors.Count < AllColors.Capacity)
            {
                AllColors.Add(Colors.Red);
            }
            var b = new SolidColorBrush(Colors.Red);
            while (AllBrushes.Count < AllBrushes.Capacity)
            {
                AllBrushes.Add(b);
            }
        }
        private static int NEnum {  get {  return Enum.GetValues(typeof(Item)).Length; } }

        public List<SolidColorBrush> AllBrushes { get; } = new List<SolidColorBrush>(NEnum);
        public List<Color> AllColors { get; } = new List<Color>(NEnum);
        public Color GetColor(Item item) { return AllColors[(int)item]; }
        public List<Double> AllThickness { get; } = new List<Double>(NEnum);
    }
}
