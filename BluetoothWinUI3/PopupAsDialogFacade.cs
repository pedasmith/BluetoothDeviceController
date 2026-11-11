using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Threading.Tasks;

namespace BluetoothWinUI3
{
    /// <summary>
    /// Facade to treat a low-level Popup as a contentdialog. 
    /// </summary>
    internal class PopupAsDialogFacade
    {
        public PopupAsDialogFacade(Popup popup, Button primaryButton, Button secondaryButton)
        {
            Popup = popup;
            PrimaryButton = primaryButton;
            SecondaryButton = secondaryButton;
        }

        private void PrimaryButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Result = ContentDialogResult.Primary;
            Close();
        }

        private void SecondaryButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            Result = ContentDialogResult.Secondary;
            Close();
        }

        private void Popup_Closed(object? sender, object e)
        {
            Popup.Closed -= Popup_Closed;
            PrimaryButton.Click -= PrimaryButton_Click;
            SecondaryButton.Click -= SecondaryButton_Click;
            PopupClosedCompletion.TrySetResult(true);
        }
        private ContentDialogResult Result = ContentDialogResult.None;
        private readonly Popup Popup;
        private readonly Button PrimaryButton;
        private readonly Button SecondaryButton;
        private TaskCompletionSource<bool> PopupClosedCompletion = new(TaskCreationOptions.RunContinuationsAsynchronously);

        /// <summary>
        /// Shows the popup and waits for the user to close it.
        /// </summary>
        public async Task<ContentDialogResult> ShowAsync()
        {
            if (Popup.IsOpen) return ContentDialogResult.None; // can't show twice!

            Popup.Opened += OnPopupOpen;

            PopupClosedCompletion = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            Result = ContentDialogResult.None;
            Popup.Closed += Popup_Closed;
            PrimaryButton.Click += PrimaryButton_Click;
            SecondaryButton.Click += SecondaryButton_Click;

            Popup.IsOpen = true;


            await PopupClosedCompletion.Task;
            Popup.Opened -= OnPopupOpen;
            return Result;
        }

        private void OnPopupOpen(object? sender, object e)
        {
            // Why Keyboard and not the obviously more correct Programmatic? because WinUI3
            // has another bug, this one around focus changes. 
            // https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.controls.control.focus?view=winrt-28000
            var focusResult = PrimaryButton.Focus(Microsoft.UI.Xaml.FocusState.Keyboard);
            ;
        }

        public void Close()
        {
            Popup.IsOpen = false;
        }
    }
}
