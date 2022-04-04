namespace BluetoothDeviceController.BluetoothProtocolsCustom
{
    /// <summary>
    /// Just the data from the pulse oximeter. Make this with the _factory which will take in a set
    /// of bytes and let you create this class.
    /// </summary>

    public class Viatom_PulseOximeter_PC60FW
    {


        public double PulsePerMinute { get; set; }
        public double OxygenSaturationInPercent { get; set; }
        public double PerfusionIndexInPercent { get; set; }
    }
}
