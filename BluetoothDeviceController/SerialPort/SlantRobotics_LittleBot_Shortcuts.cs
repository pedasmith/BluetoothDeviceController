namespace BluetoothDeviceController.SerialPort
{
    class SlantRobotics_LittleBot_Shortcuts : Shortcuts
    {
        public SlantRobotics_LittleBot_Shortcuts()
        {
            // Why these? Because that's what comes in the package :-)
            // BluetoothName = "HC 06";
            Id = "SlantRobotics-LittleBot";
            Name = "Slant Robotics LittleBot+";
            DeviceName = "Dev B";
            Description = "The Slant Robotics LittleBot robot";

            Add("Stop", "222 0 0 0\n", "Code 222=turn off all motors");
            Add("Go", "1 20 20 0\n", "forward at speed 20");
            Add("Go slow", "1 10 10 0\n", "forward at speed 10");
            Add("Left", "1 10 20 0\n", "turn left");
            Add("Right", "1 20 10 0\n", "turn right");
            Add("Back", "1 -15 -15 0\n", "backwards");
            Add("Automatic", "256 0 0 0\n", "automatic mode");

            // Things I've added to walteros
            Add("Distance", "4 0 0 0\n", "get distance");
            Add("Name", "3 0 0 0\n", "get name");
        }
    }
}
