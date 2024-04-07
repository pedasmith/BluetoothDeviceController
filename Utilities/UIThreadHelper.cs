using System;

namespace Utilities
{
    static class UIThreadHelper
    {
        /// <summary>
        /// Returns TRUE iff the current thread is the UI thread
        /// </summary>
        /// <returns></returns>
        public static bool IsOnUIThread()
        {
            var dispather = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            var retval = dispather.HasThreadAccess;
            return retval;
        }

        /// <summary>
        /// Calls the given function either directly or on the UI thread the TryRunAsync(). The resulting task is just thrown away.
        /// </summary>
        /// <param name="f"></param>
        /// <param name="priority"></param>
        public static void CallOnUIThread(Action f, Windows.UI.Core.CoreDispatcherPriority priority = Windows.UI.Core.CoreDispatcherPriority.Low)
        {
            if (IsOnUIThread())
            {
                f();
            }
            else
            {
                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                var task = dispatcher.TryRunAsync(priority, () => { f(); });
            }
        }
    }
}
