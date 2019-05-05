using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Device.Location;
using Microsoft.Phone.Marketplace;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;


namespace Geo_Tasks
{
    public partial class App : Application
    {
        private static LicenseInformation _licenseInfo = new LicenseInformation();
        private static bool _isTrial = true;

        public List<Pins> PinList = new List<Pins>();        

        public IsolatedStorageSettings appSettings = IsolatedStorageSettings.ApplicationSettings;
        
        public int LoopDistance = 500;
        public Pins CurrPinSelection; 

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are handed off to GPU with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                // Disable the application idle detection by setting the UserIdleDetectionMode property of the
                // application's PhoneApplicationService object to Disabled.
                // Caution:- Use this under debug mode only. Application that disables user idle detection will continue to run
                // and consume battery power when the user is not using the phone.
                PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
            }
            
        }

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            CheckLicense();
            if (appSettings.Contains("LoopDistance"))
            {
                LoopDistance = (int)appSettings["LoopDistance"];
            }
            else
            {
                LoopDistance = 500;
                appSettings["LoopDistance"] = 500;
            }
            if (!appSettings.Contains("ZoomLevel"))
            {
                appSettings["ZoomLevel"] = 15;
            }
            if(!appSettings.Contains("PositioningAccuracy"))
            {
                appSettings["PositioningAccuracy"] = "High";
            }


            
            /*
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Encoding = System.Text.Encoding.UTF8,
                Indent = true,
                NewLineOnAttributes = true,
            };
            
            XmlSerializerNamespaces xmlnsEmpty = new XmlSerializerNamespaces();
            xmlnsEmpty.Add("", "http://www.wow.thisworks.com/2010/05");
            MemoryStream memStr = new MemoryStream();
            using (XmlWriter writer = XmlTextWriter.Create(memStr, settings))
            {
                XmlSerializer tokenSerializer = new XmlSerializer(typeof(TargetType));
                tokenSerializer.Serialize(writer, theInstance, xmlnsEmpty);
            }
            */

            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("TaskList.xml", FileMode.Open))
                    {                        
                        XmlSerializer serializer = new XmlSerializer(typeof(Pins));
                        //stream.Position = 0;
                        PinList = (List<Pins>)serializer.Deserialize(stream);                        
                    }
                }
            }
            catch (Exception ex)
            {
                Pins p = new Pins(ex.Message, 0, 0);
            }

        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            CheckLicense();
            if (appSettings.Contains("LoopDistance"))
            {
                LoopDistance = (int)appSettings["LoopDistance"];
            }
            else
            {
                LoopDistance = 500;
                appSettings["LoopDistance"] = 500;
            }
            if (!appSettings.Contains("ZoomLevel"))
            {
                appSettings["ZoomLevel"] = 15;
            }
            if (!appSettings.Contains("PositioningAccuracy"))
            {
                appSettings["PositioningAccuracy"] = "High";
            }


            try
            {
                using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("TaskList.xml", FileMode.Open))
                    {                        
                        XmlSerializer serializer = new XmlSerializer(typeof(List<Pins>));
                        PinList = (List<Pins>)serializer.Deserialize(stream);                        
                    }
                }
            }
            catch
            {
                //add some code here
            }
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {
        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion

        private void Home_Click(object sender, EventArgs e)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        private void Tasks_Click(object sender, EventArgs e)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri("/Tasks.xaml", UriKind.Relative));    
        }

        private void Settings_Click(object sender, EventArgs e)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri("/Settings.xaml", UriKind.Relative));    
        }

        private void About_Click(object sender, EventArgs e)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri("/About.xaml", UriKind.Relative));  
        }

        private void Add_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btnAdd = (ApplicationBarIconButton)sender;
            btnAdd.IsEnabled = false; 
            
            PinList.Add(CurrPinSelection);
            CurrPinSelection = null;

            // ApplicationBar appBar= ((ApplicationBar)Application.Current.Resources["AppBar"]);
            
        }

        private void Del_Click(object sender, EventArgs e)
        { 
            
            ApplicationBarIconButton btnDel = (ApplicationBarIconButton)sender; 
            btnDel.IsEnabled = false; 

            (App.Current as App).PinList.Remove(CurrPinSelection);
            CurrPinSelection = null; 
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            ApplicationBarIconButton btnAdd = (ApplicationBarIconButton)sender;
            btnAdd.IsEnabled = false;

            ApplicationBarIconButton btnDel = (ApplicationBarIconButton)sender;
            btnDel.IsEnabled = false;

            CurrPinSelection = null;
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri("/Tasks.xaml", UriKind.Relative));   
        }

        
        public bool IsTrial
        {
            get
            {
                return _isTrial;
            }
        }

        /// <summary>
        /// Check the current license information for this application
        /// </summary>
        private void CheckLicense()
        {
            // When debugging, we want to simulate a trial mode experience. The following conditional allows us to set the _isTrial 
            // property to simulate trial mode being on or off. 
#if DEBUG
            string message = "Press 'OK' to simulate trial mode. Press 'Cancel' to run the application in normal mode.";
            if (MessageBox.Show(message, "Debug Trial",
                 MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _isTrial = true;
            }
            else
            {
                _isTrial = false;
            }
#else
            _isTrial = _licenseInfo.IsTrial();
#endif
        }
    }

    [XmlRoot("Pins")]
    //[XmlType("Pins")]
    public class Pins
    {
        private string details;
        private GeoCoordinate address;

        [System.Obsolete("use Pins()", true)]
        public Pins() 
        {
            //this.address = null;
            //this.details = "";
        }

        public Pins(string Message, double Lat, double Lon)
        {
            this.address = new GeoCoordinate(Lat, Lon);
            this.details = Message;
        }

        [XmlAttribute(DataType = "string", AttributeName = "Details")]
        public string Details
        {
            get { return details; }
            set { details = value; }
        }

        [XmlElement("Address")]
        public GeoCoordinate Address
        {
            get { return address; }
            set { //address = new GeoCoordinate(value latitude, longitude);
            }
        }
    }
}