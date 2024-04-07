using Windows.Foundation;
using Windows.UI.ViewManagement;
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
            var preferredSize = new Size(410, 240);
            SetPreferredWindowSize(preferredSize);
        }

        private void SetPreferredWindowSize(Size preferredSize)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (true || localSettings.Values["launchedWithPrefSize"] == null)
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
