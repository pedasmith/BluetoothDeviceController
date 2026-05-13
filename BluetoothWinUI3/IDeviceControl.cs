using BluetoothWinUI3.BluetoothWinUI3Registration;
using System.Collections.Generic;


namespace BluetoothWinUI3
{
    public interface IDeviceControl
    {
        /// <summary>
        /// Returns the KnownDevice associated with the device
        /// </summary>
        /// <returns></returns>
        KnownDevice KnownDeviceFromDataContext { get;  }

        /// <summary>
        /// Get the list of graph name (e.g., "Temperature", "Pressure", "Humidity". This will be used by
        /// e.g., the MainWindow to update the device menu display
        /// </summary>
        List<string> LineNames { get; }
        uint GetGraphColor(string lineName); // OxyColor is a set of 4 bytes ARGB

        /// <summary>
        /// Update the color of a single line
        /// </summary>
        void UpdateGraphColor(string lineName, uint color);

        /// <summary>
        /// updates the device control user interface base on the user preferences in SaveData.
        /// </summary>
        /// <param name="saveData"></param>
        void UpdateUX(SaveData saveData);

        void UpdateUX(UserPreferences newPrefs, UserPreferences oldPrefs);

        void UpdateUX(MainWindow.WindowSize windowSize);

        enum Visibility {  Visible, Collapsed, }
        Visibility GetDataGridVisibility();
        void SetDataGridVisibility(Visibility visibility);

    }
}
