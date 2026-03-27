using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothProtocols
{
    public class BluetoothCommunicationStatus
    {
        public BluetoothCommunicationStatus(string about, byte? error, GattCommunicationStatus status)
        {
            About = about;
            ProtocolError = error;
            Status = status;
        }

        public string About { get; internal set; }
        public byte? ProtocolError { get; internal set; }
        public GattCommunicationStatus Status { get; internal set; }

        /// <summary>
        /// Converts the status values here into a human readable but technical string
        /// </summary>
        public string AsStatusString {
            get
            {
                switch (Status)
                {
                    case GattCommunicationStatus.Success:
                        return $"OK: {About}";
                    case GattCommunicationStatus.AccessDenied:
                        return $"Access denied: {About}";
                    case GattCommunicationStatus.Unreachable:
                        return $"Unreachable: {About}";
                    case GattCommunicationStatus.ProtocolError:
                        if (ProtocolError.HasValue)
                        {
                            return $"Protocol error: value={ProtocolError.Value} {About}";
                        }
                        else
                        {
                            return $"Protocol error: {About}";
                        }
                    default:
                        return $"??: {About}";
                }
            }
        }
    }
}
