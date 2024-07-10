using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI;
using Windows.UI.Xaml.Media;
//using Microsoft.Toolkit.Uwp.Helpers;
using ColorHelper = Microsoft.Toolkit.Uwp.Helpers.ColorHelper;

namespace BluetoothProtocolsUwpXaml.ChartControl
{
    public class UserPersonalization

    {
        public static UserPersonalization Current { get; set; }
        public enum Item { None, ChartBackground, ThinCursor, Wave1, Wave2, Wave3, Wave4, ReticuleMajor, ReticuleMinor, TextLabel, TextLabelBackground };
        public const int NWaveValues = 4;
        public Brush GetBrush(Item item) { return AllBrushes[(int)item]; }
        public double GetThickness(Item item) { return AllThickness[(int)item]; }

        public static void Init()
        {
            if (Current != null) return;

            UserPersonalization pref = Get_Debug_Personalization();
            pref = Get_Tek_Personalization();
            pref = Get_Sigent_Personalization();
            Current = pref;
        }



        public static UserPersonalization GetPersonalization(string name)
        {
            switch (name)
            {
                default:
                case "debug": return Get_Debug_Personalization();
                case "sigent": return Get_Sigent_Personalization();
                case "tek": return Get_Tek_Personalization();
                case "ut": return Get_UT_Personalization();
            }
        }
        public class PersonalizationDetails
        {
            public PersonalizationDetails(string name, string tag, string description)
            {
                Name = name;
                Tag = tag;
                Description = description;
            }
            public string Name { get; }
            public string Tag { get; }
            public string Description { get; }
            public override string ToString()
            {
                return $"{Name} tag={Tag} description={Description}";
            }
        }
        public static List<PersonalizationDetails> GetPersonalizationList { get; } = new List<PersonalizationDetails>() 
        {
            new PersonalizationDetails("New", "ut", "New digital style"),
            new PersonalizationDetails("Modern", "sigent", "Modern style digital"),
            new PersonalizationDetails("Classic", "tek", "Classic analog"),
            new PersonalizationDetails("Debug", "debug", "Setup for debugging"),
        };

        private static UserPersonalization Get_Debug_Personalization()
        {
            var pref = new UserPersonalization();
            pref.SetColor(Item.ChartBackground, Colors.DarkGray);
            pref.SetColor(Item.ThinCursor, Colors.White);
            pref.SetColor(Item.Wave1, Colors.Green);
            pref.SetColor(Item.Wave2, Colors.Cyan);
            pref.SetColor(Item.Wave3, Colors.Blue);
            pref.SetColor(Item.Wave4, Colors.DarkGoldenrod);
            pref.SetColor(Item.ReticuleMajor, Colors.LimeGreen);
            pref.SetColor(Item.ReticuleMinor, Colors.LimeGreen);
            pref.SetColor(Item.TextLabel, Colors.Black);
            pref.SetColor(Item.TextLabelBackground, Colors.BlanchedAlmond);

            pref.SetThickness(Item.ThinCursor, 1.0);
            pref.SetThickness(Item.Wave1, 3.0);
            pref.SetThickness(Item.Wave2, 3.0);
            pref.SetThickness(Item.Wave3, 3.0);
            pref.SetThickness(Item.Wave4, 3.0);
            pref.SetThickness(Item.ReticuleMajor, 2.0);
            pref.SetThickness(Item.ReticuleMinor, 1.0);

            return pref;
        }

        private static UserPersonalization Get_Sigent_Personalization()
        {
            // From Sigent-2024-Oscilloscope-Tutorial-0001-title-image-1920x1080
            var pref = new UserPersonalization();
            pref.SetColor(Item.ChartBackground, ColorHelper.ToColor("#FF0E0910"));
            pref.SetColor(Item.ThinCursor, Colors.White);
            pref.SetColor(Item.Wave1, ColorHelper.ToColor("#FFD7DDB9"));
            pref.SetColor(Item.Wave2, ColorHelper.ToColor("#FFD7DDB9"));
            pref.SetColor(Item.Wave3, ColorHelper.ToColor("#FFD7DDB9"));
            pref.SetColor(Item.Wave4, ColorHelper.ToColor("#FFD7DDB9"));
            pref.SetColor(Item.ReticuleMajor, ColorHelper.ToColor("#FFD7DDB9"));
            pref.SetColor(Item.ReticuleMinor, ColorHelper.ToColor("#FFD7DDB9"));
            pref.SetColor(Item.TextLabel, ColorHelper.ToColor("#FFF1F3FC"));
            pref.SetColor(Item.TextLabelBackground, ColorHelper.ToColor("#FF55415A"));

            pref.SetThickness(Item.ThinCursor, 1.0);
            pref.SetThickness(Item.Wave1, 1.5);
            pref.SetThickness(Item.Wave2, 1.5);
            pref.SetThickness(Item.Wave3, 1.5);
            pref.SetThickness(Item.Wave4, 1.5);
            pref.SetThickness(Item.ReticuleMajor, 2.0);
            pref.SetThickness(Item.ReticuleMinor, 1.0);

            return pref;
        }
        private static UserPersonalization Get_Tek_Personalization()
        {
            var pref = new UserPersonalization();
            pref.SetColor(Item.ChartBackground, ColorHelper.ToColor("#FF19B7C2"));
            pref.SetColor(Item.ThinCursor, Colors.White);
            pref.SetColor(Item.Wave1, ColorHelper.ToColor("#FFA8FFFF"));
            pref.SetColor(Item.Wave2, ColorHelper.ToColor("#FFA8FFFF"));
            pref.SetColor(Item.Wave3, ColorHelper.ToColor("#FFA8FFFF"));
            pref.SetColor(Item.Wave4, ColorHelper.ToColor("#FFA8FFFF"));
            pref.SetColor(Item.ReticuleMajor, ColorHelper.ToColor("#FF016B78"));
            pref.SetColor(Item.ReticuleMinor, ColorHelper.ToColor("#FF016B78"));
            pref.SetColor(Item.TextLabel, ColorHelper.ToColor("#FF1B1A14"));
            pref.SetColor(Item.TextLabelBackground, ColorHelper.ToColor("#FF9E978D"));

            pref.SetThickness(Item.ThinCursor, 1.0);
            pref.SetThickness(Item.Wave1, 1.5);
            pref.SetThickness(Item.Wave2, 1.5);
            pref.SetThickness(Item.Wave3, 1.5);
            pref.SetThickness(Item.Wave4, 1.5);
            pref.SetThickness(Item.ReticuleMajor, 2.0);
            pref.SetThickness(Item.ReticuleMinor, 1.0);

            return pref;
        }
        private static UserPersonalization Get_UT_Personalization()
        {
            // From UN-T-2-2024.png
            var pref = new UserPersonalization();
            pref.SetColor(Item.ChartBackground, ColorHelper.ToColor("#FF000000"));
            pref.SetColor(Item.ThinCursor, ColorHelper.ToColor("#FFC6A113"));
            pref.SetColor(Item.Wave1, ColorHelper.ToColor("#FFFBFA44"));
            pref.SetColor(Item.Wave2, ColorHelper.ToColor("#FF87FFFF"));
            pref.SetColor(Item.Wave3, ColorHelper.ToColor("#FFFBFA44")); // duplicate the colors
            pref.SetColor(Item.Wave4, ColorHelper.ToColor("#FF87FFFF"));
            pref.SetColor(Item.ReticuleMajor, ColorHelper.ToColor("#FF585858"));
            pref.SetColor(Item.ReticuleMinor, ColorHelper.ToColor("#FF585858"));
            pref.SetColor(Item.TextLabel, ColorHelper.ToColor("#FFFAFAFA"));
            pref.SetColor(Item.TextLabelBackground, ColorHelper.ToColor("#FF020202"));

            pref.SetThickness(Item.ThinCursor, 1.0);
            pref.SetThickness(Item.Wave1, 1.5);
            pref.SetThickness(Item.Wave2, 1.5);
            pref.SetThickness(Item.Wave3, 1.5);
            pref.SetThickness(Item.Wave4, 1.5);
            pref.SetThickness(Item.ReticuleMajor, 2.0);
            pref.SetThickness(Item.ReticuleMinor, 1.0);

            return pref;
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
