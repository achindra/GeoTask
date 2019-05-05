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
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Device.Location;
using Microsoft.Phone.Controls.Maps;
using Microsoft.Phone.Notification;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.IO.IsolatedStorage;

namespace Geo_Tasks
{
    public partial class Tasks : PhoneApplicationPage
    {
        //GeoCoordinateWatcher Watcher;
        Pushpin CurrPin = new Pushpin();
        

        public Tasks()
        {
            InitializeComponent();

            MyMap.ZoomLevel = 16;
            GeoCoordinateWatcher watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            watcher.Start();
            MyMap.Center = watcher.Position.Location;
            Pushpin CurrPin = new Pushpin();
            CurrPin.Location = watcher.Position.Location;
            CurrPin.Opacity = 0.8; CurrPin.Content = "We are here!";
            MyMap.Children.Clear();
            MyMap.Children.Add(CurrPin);
            watcher.Stop();

            MyMap.Children.Clear();

            foreach (Pins PinTasks in (App.Current as App).PinList)
            {
                Pushpin NEWPin = new Pushpin();
                NEWPin.Location = PinTasks.Address;
                NEWPin.Content = PinTasks.Details; NEWPin.Opacity = 0.5; MyMap.Children.Add(NEWPin);
                NEWPin.DoubleTap += new EventHandler<GestureEventArgs>(NEWPin_DoubleTap);
            }
            
            watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(watcher_PositionChanged);
            watcher.Start();
        }

        void watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {

            CurrPin.Location = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            CurrPin.Opacity = 0.8; CurrPin.Content = "We are here!";
            
            MyMap.Children.Clear();
            MyMap.Children.Add(CurrPin);

            //MyMap.Center = CurrPin.Location;

            foreach (Pins PinTasks in (App.Current as App).PinList)
            {
                Pushpin NEWPin = new Pushpin();
                NEWPin.Location = PinTasks.Address;

                //if (NEWPin.Location.GetDistanceTo(CurrPin.Location) < (App.Current as App).LoopDistance)
                try
                {
                    NEWPin.Content = PinTasks.Details; NEWPin.Opacity = 0.5; MyMap.Children.Add(NEWPin);
                    MyMap.Children.Add(NEWPin);
                }
                catch (Exception ex)
                {
                }
            }
        }

        void txtDetails_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                (App.Current as App).CurrPinSelection.Details = txtDetails.Text;
                ApplicationBarIconButton btnAdd = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                btnAdd.IsEnabled = true;
            }
        }

        void NEWPin_DoubleTap(object sender, GestureEventArgs e)
        {
            ApplicationBarIconButton BtnDel = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            BtnDel.IsEnabled = true;

            (App.Current as App).CurrPinSelection = new Pins(((Pushpin)sender).Content.ToString(), ((Pushpin)sender).Location.Latitude, ((Pushpin)sender).Location.Longitude);
        }


        private void ApplicationBarIconButton_New_Click(object sender, EventArgs e)
        {
            if ((Application.Current as App).IsTrial && (Application.Current as App).PinList.Count>=3)
            {
                string message = "You are running a trial mode, only 3 Tasks are allowed.\n" +
                    "Press OK to buy, Cancel to continue.";
                if (MessageBox.Show(message, "Trial Mode", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();
                    _marketPlaceDetailTask.Show();
                }
            }
            else
            {
                txtDetails.Visibility = Visibility.Visible;

                //New Button
                ApplicationBarIconButton BtnNew = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                BtnNew.IsEnabled = false;

                //Save Button
                ApplicationBarIconButton BtnSave = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
                BtnSave.IsEnabled = true;

                //Delete Button
                ApplicationBarIconButton BtnDel = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                BtnDel.IsEnabled = false;

                //Cancel Button
                ApplicationBarIconButton BtnCncl = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                BtnCncl.IsEnabled = true;
            }
        }

        private void ApplicationBarIconButton_Save_Click(object sender, EventArgs e)
        {
            (App.Current as App).PinList.Add((App.Current as App).CurrPinSelection = new Pins(txtDetails.Text, MyMap.Center.Latitude, MyMap.Center.Longitude));

            Pushpin CurrPin = new Pushpin(); 
            CurrPin.Location = new GeoCoordinate(MyMap.Center.Latitude, MyMap.Center.Longitude);
            CurrPin.Opacity = 0.8; CurrPin.Content = txtDetails.Text;
            CurrPin.DoubleTap += new EventHandler<GestureEventArgs>(CurrPin_DoubleTap);
            MyMap.Children.Add(CurrPin);

            (App.Current as App).CurrPinSelection = null;
            txtDetails.Visibility = Visibility.Collapsed;

            //New Button
            ApplicationBarIconButton BtnNew = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            BtnNew.IsEnabled = true;

            //Save Button
            ApplicationBarIconButton BtnSave = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            BtnSave.IsEnabled = false;

            //Delete Button
            ApplicationBarIconButton BtnDel = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            BtnDel.IsEnabled = false;

            //Cancel Button
            ApplicationBarIconButton BtnCncl = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            BtnCncl.IsEnabled = false;

            //Save File//
            // Write to the Isolated Storage
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true; xmlWriterSettings.Encoding = System.Text.Encoding.UTF8;
 
            using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = myIsolatedStorage.OpenFile("TaskList.xml", FileMode.Create))
                {                    
                    XmlSerializer serializer = new XmlSerializer(typeof(Pins));
                    using (XmlWriter xmlWriter = XmlWriter.Create(stream, xmlWriterSettings))
                    {
                        serializer.Serialize(xmlWriter, (App.Current as App).PinList);
                    }
                }
            }
 
        }

        void CurrPin_DoubleTap(object sender, GestureEventArgs e)
        {
            ApplicationBarIconButton BtnDel = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            BtnDel.IsEnabled = true;

            (App.Current as App).CurrPinSelection = new Pins(((Pushpin)sender).Content.ToString(), ((Pushpin)sender).Location.Latitude, ((Pushpin)sender).Location.Longitude);
        }

        private void ApplicationBarIconButton_Delete_Click(object sender, EventArgs e)
        {
            //MyMap.Children.Remove((Pushpin)(sender));
            (App.Current as App).PinList.Remove((App.Current as App).CurrPinSelection);
            (App.Current as App).CurrPinSelection = null; 
            txtDetails.Visibility = Visibility.Collapsed;
            

            //New Button
            ApplicationBarIconButton BtnNew = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            BtnNew.IsEnabled = true;

            //Save Button
            ApplicationBarIconButton BtnSave = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            BtnSave.IsEnabled = false;

            //Delete Button
            ApplicationBarIconButton BtnDel = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            BtnDel.IsEnabled = false;

            //Cancel Button
            ApplicationBarIconButton BtnCncl = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            BtnCncl.IsEnabled = false;
        }

        private void ApplicationBarIconButton_Cancel_Click(object sender, EventArgs e)
        {
            (App.Current as App).CurrPinSelection = null;
            txtDetails.Visibility = Visibility.Collapsed;

            //New Button
            ApplicationBarIconButton BtnNew = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            BtnNew.IsEnabled = true;

            //Save Button
            ApplicationBarIconButton BtnSave = (ApplicationBarIconButton)ApplicationBar.Buttons[1];
            BtnSave.IsEnabled = false;

            //Delete Button
            ApplicationBarIconButton BtnDel = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            BtnDel.IsEnabled = false;

            //Cancel Button
            ApplicationBarIconButton BtnCncl = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            BtnCncl.IsEnabled = false;
        }

        private void ApplicationBarIconButton_Home_Click(object sender, EventArgs e)
        {
            PhoneApplicationFrame root = Application.Current.RootVisual as PhoneApplicationFrame;
            root.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }        
    }
}