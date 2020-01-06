using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.SerialPort
{
    class StandardRobotCar_Shortcuts : Shortcuts
    {
        public StandardRobotCar_Shortcuts()
        {
            DeviceName = "";
            Description = "The Standard Robot Car (SRC) protocol";

            Add("Forward", "F", "F=forward");
            Add("Left", "L", "L=go left");
            Add("Stop", "S", "S=stop");
            Add("Right", "R", "R=go right");
            Add("Backward", "B", "B=go backwards");

            Add("Left-Forward", "G", "G=left/foreward");
            Add("Right-Forward", "I", "I=right/forward");
            Add("Left-Backward", "H", "H=left/backward");
            Add("Right-Backward", "J", "J=right/backward");

            Add("Lights ON", "W", "W=lights on");
            Add("Lights OFF", "w", "w=lights off");
            Add("Rear Lights ON", "U", "U=rear lights on");
            Add("Rear Lights OFF", "u", "u=rear lights off");
            Add("Horn ON", "V", "V=horn on");
            Add("Horn OFF", "v", "v=horn off");

            Add("Query", "Q", "Ask device for information");
            Add("Ping", "P", "Ping the distance sensors");
        }
    }
}
