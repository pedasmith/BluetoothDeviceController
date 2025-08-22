using Microsoft.UI.Xaml;
using System;
using System.Runtime.InteropServices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace TestNmeaGpsParserWinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static UserPreferences UP = new UserPreferences();
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }



        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            _window.Activate();

            // Fix the Alt key issue in WinUI 3. See #region WORKAROUND_BUG_4379_ALT_KEY_BEEPS
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(_window);
            SetWindowSubclass(hWnd, _subclassProc, IntPtr.Zero, IntPtr.Zero);

        }

        #region WORKAROUND_BUG_4379_ALT_KEY_BEEPS
        // WM_MENUCHAR: https://learn.microsoft.com/en-us/windows/win32/menurc/wm-menuchar
        // WM_SYSCHAR: https://learn.microsoft.com/en-us/windows/win32/menurc/wm-syschar
        //private const int WM_SYSCHAR = 0x0106; // not needed for working workaround.
        private const int WM_MENUCHAR = 0x0120;

        public const int MNC_IGNORE = 0;
        public const int MNC_CLOSE = 1;
        public const int MNC_EXECUTE = 2;
        public const int MNC_SELECT = 3;

        // Delegate for the subclass procedure
        private delegate IntPtr SubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData);

        [DllImport("comctl32.dll", SetLastError = true)]
        private static extern bool SetWindowSubclass(IntPtr hWnd, SubclassProc pfnSubclass, IntPtr uIdSubclass, IntPtr dwRefData);

        [DllImport("comctl32.dll", SetLastError = true)]
        private static extern bool RemoveWindowSubclass(IntPtr hWnd, SubclassProc pfnSubclass, IntPtr uIdSubclass);

        [DllImport("comctl32.dll", SetLastError = true)]
        private static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        private static SubclassProc _subclassProc = SubclassCallback;

        private static IntPtr SubclassCallback(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
        {
            switch (uMsg)
            {
                //case WM_SYSCHAR:
                //    return new IntPtr(0); // Example: returning 1 to indicate the character is handled
                case WM_MENUCHAR:
                    return new IntPtr(MNC_CLOSE<<16); // Example: returning 1 to indicate the character is handled
            }

            // Call the default subclass procedure
            return DefSubclassProc(hWnd, uMsg, wParam, lParam);
        }

        // Investigation:
        // https://github.com/microsoft/microsoft-ui-xaml/issues/4379
        // Summary: bug reporting the beep; proposed solution is to return MNC_CLOSE<<16 for WM_MENUCHAR
        // Result: proposed solution works like a champ!

        // https://learn.microsoft.com/en-us/answers/questions/1688262/how-to-not-play-default-beep-sound-when-pressing-a
        // Web summary: Says to handle the WM_SYSCHAR. 
        // Result: no effect.

        // https://www.travelneil.com/wndproc-in-uwp.html
        // Summary: gives advise on getting the HWND. 
        // Result: not actually useful because WinUI3 isn't UWP.

        // https://github.com/microsoft/microsoft-ui-xaml/issues/9074
        // Summary: closed, won't fix

        // non-useful links
        // https://github.com/microsoft/microsoft-ui-xaml/issues/6978
        // Summary: wants ALT to open a menu directly; doesn't mention the beep

        #endregion
    }
}
