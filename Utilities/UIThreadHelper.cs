#if !NO_UX
// The NO_UX is required so that the BluetoothCodeGeneratordotNetCore will compile with VS 2022
// Otherwise is just complains that it can't find the Microsoft.UI.Dispatching namespace.

#if WINDOWS_UWP
using System;
#else
using Microsoft.UI.Dispatching;
using System;
using System.Runtime.CompilerServices;
#endif

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace Utilities
{
    static class UIThreadHelper
    {
#if !WINDOWS_UWP
        public static DispatcherQueue DQueue = null;
#endif

        /// <summary>
        /// Returns TRUE iff the current thread is the UI thread
        /// </summary>
        /// <returns></returns>
        public static bool IsOnUIThread()
        {
#if WINDOWS_UWP
            var dispather = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
            var retval = dispather.HasThreadAccess;
            return retval;
#else
            bool retval = false;
            if (DQueue != null)
            {
                retval = DQueue.HasThreadAccess;
            }
            else
            {
                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                retval = dispatcher.HasThreadAccess;
            }
            return retval;
#endif
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
#if WINDOWS_UWP
                var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                var task = dispatcher.TryRunAsync(priority, () => { f(); });
#else
                if (DQueue != null)
                {
                    var prioritydqueue = DispatcherQueuePriority.Low;
                    switch (priority)
                    {
                        case Windows.UI.Core.CoreDispatcherPriority.Idle: prioritydqueue = DispatcherQueuePriority.Low; break;
                        default:
                        case Windows.UI.Core.CoreDispatcherPriority.Low: prioritydqueue = DispatcherQueuePriority.Low; break;
                        case Windows.UI.Core.CoreDispatcherPriority.Normal: prioritydqueue = DispatcherQueuePriority.Normal; break;
                        case Windows.UI.Core.CoreDispatcherPriority.High: prioritydqueue = DispatcherQueuePriority.High; break;
                    }
                    var task = DQueue.TryEnqueue(prioritydqueue, () => { f(); });
                }
                else
                {
                    var dispatcher = Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher;
                    var task = dispatcher.TryRunAsync(priority, () => { f(); });
                }
#endif
            }
        }
    }
}
#endif