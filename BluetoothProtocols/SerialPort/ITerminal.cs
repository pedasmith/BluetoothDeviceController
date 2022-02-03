using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.SerialPort
{
    public delegate void TerminalSendDataEventHandler(object sender, string data);
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

        void SetDeviceStatus(string status);
    }


}
