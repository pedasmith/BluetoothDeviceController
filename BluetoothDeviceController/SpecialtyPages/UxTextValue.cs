namespace BluetoothDeviceController.SpecialtyPages
{
    internal class UxTextValue
    {
        public UxTextValue(string text, System.Globalization.NumberStyles dec_or_hex) 
        {
            Text = text;
            Dec_or_hex = dec_or_hex;
        }
        public string Text;
        public System.Globalization.NumberStyles Dec_or_hex;
    }
}
