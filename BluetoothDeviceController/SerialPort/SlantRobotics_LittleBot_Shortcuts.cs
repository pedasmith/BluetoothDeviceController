namespace BluetoothDeviceController.SerialPort
{
    class SlantRobotics_LittleBot_Shortcuts : Shortcuts
    {
        public SlantRobotics_LittleBot_Shortcuts()
        {
            // Why these? Because that's what comes in the package :-)
            // BluetoothName = "HC 06";
            DeviceName = "Dev B";
            Description = "The Slant Robotics LittleBot robot";

            Add("Stop", "222\r\n", "Code 222=turn off all motors");
            Add("Go", "1 20 20\r\n", "forward at speed 20");
            Add("Go slow", "1 10 10\r\n", "forward at speed 10");
            Add("Left", "1 10 20\r\n", "turn left");
            Add("Right", "1 20 10\r\n", "turn right");
            Add("Back", "1 -15 -15\r\n", "backwards");
            Add("Automatic", "256\r\n", "automatic mode");

            // Things I've added to walteros
            Add("Distance", "4\r\n", "get distance");
            Add("Name", "3\r\n", "get name");
        }
    }
}
