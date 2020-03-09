using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.WebUI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController
{
    public interface IDockParent
    {
        void DisplayFromDock(Page page);
    }

    public interface IFindPage
    {
        int FindPage (string id);
    }

    public interface IDeletePage
    {
        void DeletePage(PageDockAdapterControl pdac);
    }


    public sealed partial class PageDock : UserControl, IDeletePage, IFindPage
    {

        public PageDock()
        {
            this.InitializeComponent();
        }

        public IDockParent DockParent = null;

        // Each Page is wrapped in a ViewBox zooming viewer

        /// <summary>
        /// Adds a page to the dock. It might already have a spot in the dock.
        /// Each page is wrapped in a ViewBox zooming viewer and added to the uiDock panel
        /// The dock is made visible.
        /// </summary>
        /// <param name="page"></param>
        public void AddPage(Page page)
        {
            if (!(page is HasId)) return;
            this.Visibility = Visibility.Visible;
            var id = (page as HasId).GetId();
            var index = FindPage(id);
            if (index >= 0)
            {
                var vb = uiDock.Items[index] as PageDockAdapterControl;
                // Regardless of whether the child was already set, set it to the page.
                // We might set the child to be, e.g. a little icon or something.
                vb.SetPage (page);
            }
            else
            {
                // Gotta dock it for the first time
                var vb = PageDockAdapterControl.MakeAdapter(page, this);
                uiDock.Items.Insert(0, vb); //Always put as the first item?
            }
        }

        public void DeletePage(PageDockAdapterControl page)
        {
            uiDock.Items.Remove(page);
            if (uiDock.Items.Count == 0)
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Called when the viewbox holding a page is tapped.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnPageTapped(object sender, TappedRoutedEventArgs e)
        {
            var vb = sender as PageDockAdapterControl;
            DoPageTapped(vb);
        }

        private void DoPageTapped(PageDockAdapterControl vb)
        {
            if (vb == null) return;
            var page = vb.GetPage();
            //Page page = vb.Child as Page;
            if (page == null) return;
            if (DockParent == null) return;
            // It's critical to remove the page from the viewbox; otherwise when the DockParent aka main page tries to
            // display the page, it won't work.
            vb.RemovePage(page);

            // And reset it for use!

            DockParent.DisplayFromDock(page);
        }

        /// <summary>
        /// Says if an arbitrary page can be docked or not.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public bool CanDock(Page page)
        {
            var retval = (page is HasId);
            return retval;
        }

        /// <summary>
        /// Returns an index of the page in the 
        /// </summary>
        /// <param name="find"></param>
        /// <returns></returns>
        public int FindPage(string idToFind)
        {
            for (int i = 0; i < uiDock.Items.Count; i++)
            {
                var child = uiDock.Items[i];
                var vb = child as PageDockAdapterControl;
                string id = null;
                if (vb.Tag is string)
                {
                    id = vb.Tag as string;
                }
                else
                {
                    var page = vb.GetPage();
                    if (page != null)
                    {
                        var hasid = page as HasId;
                        if (hasid != null)
                        {
                            id = hasid.GetId();
                        }
                    }
                }
                if (id == idToFind)
                {
                    return i;
                }
            }
            return -1;
        }

        private void OnSelectPage(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count != 1) return;
            var vb = e.AddedItems[0] as PageDockAdapterControl;
            DoPageTapped(vb);

            // Reset so that nothing is selected.
            uiDock.SelectedItem = null;
        }
    }
}
