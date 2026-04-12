#if !NO_UX
// The NO_UX is required so that the BluetoothCodeGeneratordotNetCore will compile with VS 2022
// Otherwise is just complains that it can't find the Microsoft.UI.Dispatching namespace.

using Microsoft.UI.Dispatching;
using System;
using System.Runtime.CompilerServices;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace Utilities
{
    static class UIThreadHelper
    {
        public static DispatcherQueue DQueue = null;

        /// <summary>
        /// Returns TRUE iff the current thread is the UI thread
        /// </summary>
        /// <returns></returns>
        public static bool IsOnUIThread()
        {
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
            }
        }
    }
}
#endif