using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using static BluetoothWinUI3.MarkdownHelpSystem;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BluetoothWinUI3
{
    public sealed partial class TextBlockWithHistory : UserControl
    {
        public const int MAX_DISPLAYED_HISTORY_ENTRIES = 20;
        public const int MAX_NOT_YET_DISPLAYED_HISTORY_ENTRIES = 10;
        public TextBlockWithHistory()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Adds text to the "most recent" history entry. 
        /// </summary>
        public void AddToMostRecent(string newDetails)
        {
            List<string> list = AmDisplayingMostRecent ? TextDisplayed : TextNotYetDisplayed;
            if (list.Count == 0)
            {
                StartMostRecent(newDetails);
                return;
            }

            list[list.Count - 1] += newDetails;
            if (AmDisplayingMostRecent)
            {
                if (TextIndex < 0) TextIndex = 0; // Can happen if user didn't call StartMostRecent first
                SetMarkdownText(TextDisplayed[TextIndex]);
            }
            UpdateHistoryPrevNextButtons();
        }

        /// <summary>
        /// Adds a new "most recent" history entry and sets the text. It will only be displayed if the user
        /// is viewing the most recent entry. If the user is viewing a previous entry, it will be added to 
        /// the list of entries that have not yet been displayed.
        /// </summary>
        public void StartMostRecent(string newText = "")
        {
            var savedDisplayingRecent = AmDisplayingMostRecent;
            List<string> list = AmDisplayingMostRecent ? TextDisplayed : TextNotYetDisplayed;
            list.Add(newText);
            if (savedDisplayingRecent)
            {
                TextIndex = TextDisplayed.Count - 1;
                SetMarkdownText(TextDisplayed[TextIndex]);
            }
            while (TextDisplayed.Count > MAX_DISPLAYED_HISTORY_ENTRIES)
            {
                TextDisplayed.RemoveAt(0);
                if (TextIndex > 0) TextIndex -= 1;
            }
            while (TextNotYetDisplayed.Count > MAX_NOT_YET_DISPLAYED_HISTORY_ENTRIES)
            {
                TextNotYetDisplayed.RemoveAt(0);
            }
            UpdateHistoryPrevNextButtons();
        }

        private void SetMarkdownText(string text)
        {
            // In markdown, add two spaces to the end of a line to make a real CR. Otherwise, the
            // markdown renderer will ignore the CR and just make a space. 
            uiHistoryMarkdown.Text = text.Replace("\n", "  \n");
        }

        /// <summary>
        /// All the text which has been displayed. If the user never pressed the next or prev buttons,
        /// all the text will be here and none will be in TextNotYetDisplayed.
        /// </summary>
        private List<String> TextDisplayed = new(); // All details

        /// <summary>
        /// All the details which are ready but the TextIndex isn't at the end
        /// </summary>
        private List<String> TextNotYetDisplayed = new();
        /// <summary>
        /// Index into the TextDisplayed list. Starts at -1 meaning there's no text yet.
        /// </summary>
        private int TextIndex = -1;
        private bool AmDisplayingMostRecent {  get {  return TextIndex == TextDisplayed.Count -1; } } 
        private void UpdateHistoryPrevNextButtons()
        {
            bool haveNext = TextIndex < TextDisplayed.Count - 1;
            bool havePrev = TextIndex > 0;

            uiDetailNext.Visibility = haveNext ? Visibility.Visible : Visibility.Collapsed;
            uiDetailPrev.Visibility = havePrev ? Visibility.Visible : Visibility.Collapsed;
            uiDetailCatchUp.Visibility = haveNext ? Visibility.Visible : Visibility.Collapsed  ;

            uiLog.Text = $"Index={TextIndex} Count={TextDisplayed.Count} NotDisplayedCount={TextNotYetDisplayed.Count}";
        }
        private void OnCopy(object sender, RoutedEventArgs e)
        {
            var text = TextDisplayed[TextIndex].Replace("&nbsp;", " "); // make it look nicer in clipboard

            DataPackage dataPackage = new DataPackage();
            dataPackage.SetText(text);
            dataPackage.Properties.Title = "Bluetooth Advertisement Data";
            Clipboard.SetContent(dataPackage);
        }

        private void OnPrevHistory(object sender, RoutedEventArgs e)
        {
            TextIndex = Math.Max(TextIndex - 1, 0);
            if (TextIndex >= TextDisplayed.Count) return; // can never happen
            SetMarkdownText(TextDisplayed[TextIndex]);
            UpdateHistoryPrevNextButtons();
        }

        private void OnCatchupHistory(object sender, RoutedEventArgs e)
        {
            if (TextNotYetDisplayed.Count > 0)
            {
                // Empty out the not yet displayed!
                foreach (var text in TextNotYetDisplayed)
                {
                    TextDisplayed.Add(text);
                }
                int n = TextNotYetDisplayed.Count;
                TextNotYetDisplayed.Clear();
            }
            TextIndex = TextDisplayed.Count - 1;
            SetMarkdownText(TextDisplayed[TextIndex]);
            UpdateHistoryPrevNextButtons();
        }

        private void OnNextHistory(object sender, RoutedEventArgs e)
        {
            // User had clicked the prev and then the next button. This stopped the 
            // TextDisplayed from getting more texts and they went into the 
            // TextNotYetDisplayed list. Now the user has clicked next and we need to
            // empty out the TextNotYetDisplayed list into the TextDisplayed list.
            if (AmDisplayingMostRecent && TextNotYetDisplayed.Count > 0)
            {
                // Empty out the not yet displayed!
                foreach (var text in TextNotYetDisplayed)
                {
                    TextDisplayed.Add(text);
                }
                int n = TextNotYetDisplayed.Count;
                TextNotYetDisplayed.Clear();
            }

            if (AmDisplayingMostRecent) return; 
            // Can happen only if the user had been at the end
            // and clicked the next button. The TextNotYetDisplayed list must be empty.

            TextIndex += 1;
            SetMarkdownText(TextDisplayed[TextIndex]);
            UpdateHistoryPrevNextButtons();
       }

        private static void Log(string str)
        {
            System.Diagnostics.Debug.WriteLine(str);
        }

        private async void OnLinkClicked(object sender, CommunityToolkit.WinUI.Controls.LinkClickedEventArgs e)
        {
            if (e == null)
            {
                Log("Markdown: user clicked on a link, but the Url is null");
                return;
            }
            var link = e.Uri;

            e.Handled = true;
            await Launcher.LaunchUriAsync(link);
        }
    }
}
