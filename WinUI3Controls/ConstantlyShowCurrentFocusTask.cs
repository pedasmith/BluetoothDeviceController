using Microsoft.UI.Xaml; // Needed for Window
using Microsoft.UI.Xaml.Controls; // Needed for TextBlock
using Microsoft.UI.Xaml.Input; // Needed for FocusManager
using System;
using System.Threading;
using System.Threading.Tasks;

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

// About the ConstantlyShowCurrentFocusTask class 
//
// WinUI3 has a terrible problem with maintaining focus on useful elements, and a worse problem when
// trying to debug issues. The ConstantlyShowCurrentFocusTask class is a simple class that will, every
// second, find the control with the focus and will update a TextBox with what it's found. 
//
//
// How ConstantlyShowCurrentFocusTask works
// 
// The ConstantlyShowCurrentFocusTask class kicks off a task which, every second, calls the FocusManager
// to get the control that has focus. Data shown includes the full type of the control with focus, and
// the name of either the control or the parent (or parent's parent, etc.), going up in the hierarchy until
// a control with a name is found. (Or until there are no more parents, possibly until there's a parent 
// that isn't a FrameworkElement).
//
// Also shown is the number of times the Focus call has happened. This is useful in case the calls fail 
// (which has happened)
//
//
// Critical information
//
// The ConstantlyShowCurrentFocusTask class uses WinUI3. If you're using some other library (like WinUI2,
// or you're using UWP or WPF or SilverLight or lots of other things), then this class won't work. Don't blame
// me for that; it's 100% the fault of Microsoft constantly fiddling with their UX code and make new, 
// imcompatible libraries.
//
//
// Getting started with ConstantlyShowCurrentFocusTask
//
// Use this class when you aren't getting the focus values you expect. To use it, in some control you are
// having problems with:
// 1. Make a field like this: private ConstantlyShowCurrentFocusTask ConstantlyShowCurrentFocus
//    and tb is a TextBlock where the logging will be written to.
// 2. Also make thesee fields fields:
//    CancellationTokenSource CTS
//    Task FocusReporterTask
// 3. In your constructor, initialize the ConstantlyShowCurrentFocus like this :
//    ConstantlyShowCurrentFocus = new ConstantlyShowCurrentFocusTask(w, tb)
//    where w is the window associated with the control (you might be able to get this from your apps the MainWindow class)
// 4. In your Loaded event, initialize the CancellationTokenSource
// 5. Also in your Loaded event, call ConstantlyShowCurrentFocus.CreateTask and start it:
//    FocusReporterTask = ConstantlyShowCurrentFocus.CreateTake(CTS.Token);
//    FocusReporterTask.Start()
// 6. In your Unloaded event, cancel the task
//    CTS.Cancel()
//
// What should now happen is that when your control is shown, it will automatically start updating the 
// TextBox with information on the currently focused element.

namespace WinUI3Controls
{
    public class ConstantlyShowCurrentFocusTask
    {
        public ConstantlyShowCurrentFocusTask(Window w, TextBlock tb)
        {
            AppWindow = w;
            LogBlock = tb;
            NFocusCalls = 0;
        }
        Window AppWindow;
        TextBlock LogBlock;
        CancellationToken CT;
        int NFocusCalls;
        int NQueueFailures = 0;


        public Task CreateTask(CancellationToken ct)
        {
            CT = ct;
            var FocusReporterTask = new Task(async () =>
            {
                NFocusCalls = 0;
                while (!CT.IsCancellationRequested)
                {
                    await Task.Delay(1000); // update every second

                    // Similar to my UIThreadHelper.CallInUIThread utility method, but split out here
                    // so that this file can be used without any other special code.

                    if (AppWindow != null && AppWindow.DispatcherQueue != null)
                    {
                        bool isQueued = AppWindow.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () =>
                        {
                            if (CT.IsCancellationRequested) return; // early return to reduce errors
                            ShowFocusedElement();
                        });
                        if (!isQueued) NQueueFailures++;
                        // when isQueued==false there was a failured, but it's not clear why or how to report it.
                    }
                }
            });
            return FocusReporterTask;
        }

        /// <summary>
        /// Find the currently focussed element and write out some logging info. Must be done on the UI thread
        /// </summary>
        private void ShowFocusedElement()
        {
            try
            {
                NFocusCalls++;
                var obj = FocusManager.GetFocusedElement(AppWindow.Content.XamlRoot);
                var el = obj as FrameworkElement;
                if (obj == null)
                {
                    UpdateFocusText($"No focussed element ({NFocusCalls})");
                }
                else if (el == null)
                {
                    UpdateFocusText($"Object with focus is {obj.GetType().FullName} not FrameworkElement ({NFocusCalls})  ");
                }
                else
                {
                    int nparent = 0;
                    var originalElement = el;
                    while (el != null && string.IsNullOrWhiteSpace(el.Name))
                    {
                        nparent++;
                        el = el.Parent as FrameworkElement;
                    }
                    if (el != null)
                    {
                        UpdateFocusText($"Focus: name={el.Name} type={el.GetType().FullName} nparent={nparent} original={originalElement.GetType().FullName} ({NFocusCalls})");
                    }
                    else
                    {
                        UpdateFocusText($"Focus: null? name={el?.Name} type={el?.GetType().FullName} nparent={nparent} original={originalElement.GetType().FullName} ({NFocusCalls})");
                    }
                }
            }
            catch (Exception ex)
            {
                UpdateFocusText($"Focus: error: {ex.Message} ({NFocusCalls})");
            }
        }

        /// <summary>
        /// Method to display the text to be logged. Must be called on UI thread
        /// </summary>
        private void UpdateFocusText(string text)
        {
            LogBlock.Text = text;
        }

    }
}
