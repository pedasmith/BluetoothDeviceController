using SampleServerXaml;
using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.WebUI;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace BluetoothCurrentTimeServer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            uiCurrentTimeServer.FillBtUnits = uiUnits;
            var preferredSize = new Size(410, 30+2*210);  // Kind of a guess for size. t+1x = 240 t+2x=450 x=210 t=30
            SetPreferredWindowSize(preferredSize);
        }

        public BtUnits FillBtUnits(BtUnits units = null)
        {
            return uiUnits.FillBtUnits(units);
        }

        private void SetPreferredWindowSize(Size preferredSize)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (true || localSettings.Values["launchedWithPrefSize"] == null) // TODO: shouldn't the true || be removed?
            {
                // first app launch only!!
                ApplicationView.PreferredLaunchViewSize = preferredSize;
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                localSettings.Values["launchedWithPrefSize"] = true;
            }
            // resetting the auto-resizing -> next launch the system will control the PreferredLaunchViewSize
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Auto;
            ApplicationView.PreferredLaunchViewSize = preferredSize;
            //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView().SetPreferredMinSize(preferredSize);
        }
    }
}
