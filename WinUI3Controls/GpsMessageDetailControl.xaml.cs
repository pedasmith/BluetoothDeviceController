using Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinUI3Controls
{
    public sealed partial class GpsMessageDetailControl : UserControl
    {
        // This class can be very empty because the data all comes from data binding.
        public GpsMessageDetailControl()
        {
            this.DataContext = null;
            this.InitializeComponent();
        }
    }
}
