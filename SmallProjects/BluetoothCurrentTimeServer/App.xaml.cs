﻿using SampleServerXaml;
using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace BluetoothCurrentTimeServer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                }
                //TODO: Load state from previously suspended application
                try
                {
                    var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;

                    // Save a setting locally on the device
                    object userUnitSettingsRaw = null;
                    settings.Values.TryGetValue(UserUnitName, out userUnitSettingsRaw);
                    var userUnitSettings = userUnitSettingsRaw as ApplicationDataCompositeValue;
                    if (userUnitSettings != null)
                    {
                        BtUnits.SavedBtUnits.TimePref = (BtUnits.Time)userUnitSettings["Time"];
                        BtUnits.SavedBtUnits.TemperaturePref = (BtUnits.Temperature)userUnitSettings["Temp"];
                    }
                }
                catch (Exception)
                {
                    ; // don't care why reloading settings failed! In all cases, the app should still work!
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        const string UserUnitName = "UserUnit";

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity

            var frame = Window.Current?.Content as Frame;
            var page = frame?.Content as MainPage;
            if (page != null)
            {
                // User Unit settings
                var btunit = page.FillBtUnits(BtUnits.SavedBtUnits);

                var settings = Windows.Storage.ApplicationData.Current.RoamingSettings;
                var composite = new ApplicationDataCompositeValue();
                composite["Time"] = (int)btunit.TimePref; // FYI: NoPreference = 0,hour24 = 0x8024, hour12ampm = 0x8025
                composite["Temp"] = (int)btunit.TemperaturePref; // FYI: NoPreference = 0, celsius = 0x272F,fahrenheit = 0x27AC, kelvin = 0x2705,
                settings.Values[UserUnitName] = composite;
            }


            deferral.Complete();
        }
    }
}
