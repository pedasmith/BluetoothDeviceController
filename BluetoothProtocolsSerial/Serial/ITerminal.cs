namespace BluetoothDeviceController.SerialPort
{

    public enum ConnectionState
    {
        NotStarted,
        UX,
        ScanningForDevices,
        VerifyDeviceCapabilities,
        ConnectingToDevice,
        SendingAndReceiving,
    };
    public enum ConnectionSubstate
    {
        UXReset,
        SfdStarted, SfdCompletedOk, SfdNoDeviceFound, SfdException, 
        VdcStarted, VdcCompletedOk, VdcGettingDevice, VdcGotDevice,  VdcReusingDevice, VdcCachedServiceCount, VdcUncachedServiceCount, VdcNoDevice, VdcNoServices, VdcException,
        CtdStarted, CtdCompletedOk, CtdHostName, CtdServiceName, CtdException,

        SRStarted, SRWaitingForData, SRGotData, SRCancelled, SRException,
    }

    /// <summary>
    /// Used by terminal-like controls to be the main interface between the code that handles a device and the user.
    /// This might be a "real" terminal like a Telnet terminal, or a serial port terminal (they are subtely different :-) )
    /// or to a serial-using device like a the Dual XGPS150A GPS device which has a serial interface but which produces
    /// a rich data stream which the GPS page can display.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public delegate void TerminalSendDataEventHandler(object sender, string data);

    public static class TerminalSupport
    {
        public static string StateAsString(ConnectionState state, ConnectionSubstate substate, double value=-999)
        {
            switch (state)
            {
                case ConnectionState.ScanningForDevices:
                    switch (substate)
                    {
                        case ConnectionSubstate.SfdStarted: return "Scanning";
                        case ConnectionSubstate.SfdCompletedOk: return $"Found {value} devices";
                    }
                    break;
            }
            return $"State={state} substate={substate}";
        }

        public static string StateAsIcon(ConnectionState state, ConnectionSubstate substate)
        {
            return "CHECK";
        }
    }

    public interface ITerminal
    {
        /// <summary>
        /// The device has gotten some data and calls this method to tell the terminal to display it.
        /// </summary>
        /// <param name="data"></param>
        void ReceivedData(string data);

        /// <summary>
        /// The terminal has gotten data from the user which needs to be send to the device.
        /// </summary>
        event TerminalSendDataEventHandler OnSendData;
        /// <summary>
        /// The device has errored out in some way and calls this method to tell the terminal to display it.
        /// </summary>
        /// <param name="error"></param>
        void ErrorFromDevice(string error);

        /// <summary>
        /// Cheap and cheerful status. The better status is the longer one that take in more details.
        /// </summary>
        void SetDeviceStatus(string status);
        void SetDeviceStatusEx(ConnectionState status, ConnectionSubstate substate, string text="", double value=-999);
    }
}
