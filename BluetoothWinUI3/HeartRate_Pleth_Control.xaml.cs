using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3
{
    /// <summary>
    /// Pleth stands for Plethysmograph and show pulse data. Recommended size is 200x10
    /// </summary>
    public sealed partial class HeartRate_Pleth_Control : UserControl
    {
        const int NLINE = 140; // Makes a decent display
        const double THICKNESS = 1.8;
        double HEIGHT { get { return ActualHeight < 10 ? 10 : ActualHeight; } }
        double WIDTH { get { return ActualWidth < 10 ? 10 : ActualWidth; } }
        int NextLineIndex = 0;
        public HeartRate_Pleth_Control()
        {
            InitializeComponent();
            uiBars.Width = NLINE * THICKNESS;
            uiBars.Height = HEIGHT;
            var stroke = new SolidColorBrush(Colors.DarkGoldenrod);
            for (int i=0; i<NLINE; i++)
            {
                var line = new Line()
                {
                    Stroke = stroke,
                    StrokeThickness = THICKNESS,
                    X1 = i * (WIDTH / NLINE),
                    X2 = i * (WIDTH / NLINE),
                    Y1 = HEIGHT,
                    Y2 = HEIGHT, // I used to set this to make a display... - (((i*5) % 256) * (HEIGHT / 255)),
                };
                uiBars.Children.Add(line);
            }
        }

        /// <summary>
        /// Add a value 0..255
        /// </summary>
        public void AddNextPulse(double value)
        {
            var line = uiBars.Children[NextLineIndex] as Line;
            if (line == null) return;
            line.Y1 = HEIGHT;
            line.Y2 = HEIGHT - (value * (HEIGHT / 255));
            line.X1 = NextLineIndex * (WIDTH / NLINE);
            line.X2 = NextLineIndex * (WIDTH / NLINE);

            // the cursor is just the next line. It's displayed by
            // zeroing out the size.
            var cursor = uiBars.Children[(NextLineIndex + 1) % NLINE] as Line;
            if (cursor == null) return;
            cursor.Y1 = HEIGHT;
            cursor.Y2 = HEIGHT;


            // Set up for the next call
            NextLineIndex = (NextLineIndex + 1) % NLINE;
        }
    }
}
