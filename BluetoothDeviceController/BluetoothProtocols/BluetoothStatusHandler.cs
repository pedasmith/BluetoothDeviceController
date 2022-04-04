using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{

    /// <summary>
    /// Common event delegate for all Bluetooth status values.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="status"></param>
    public delegate void BluetoothStatusHandler(object source, BluetoothCommunicationStatus status);

    public class BluetoothStatusEvent
    {
        public event BluetoothStatusHandler OnBluetoothStatus;
        public void ReportStatus(string report, GattDeviceServicesResult status)
        {
            if (status == null)
            {
                ;
            }
            OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, status.ProtocolError, status.Status));
        }
        public void ReportStatus(string report, GattCharacteristicsResult status)
        {
            if (status == null)
            {
                OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, null, GattCommunicationStatus.ProtocolError));
            }
            else
            {
                OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, status.ProtocolError, status == null ? GattCommunicationStatus.ProtocolError : status.Status));
            }
        }
        public void ReportStatus(string report, GattCommunicationStatus status)
        {
            OnBluetoothStatus?.Invoke(this, new BluetoothCommunicationStatus(report, null, status));
        }
    }
}
