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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Phone.Shell;

namespace GeoTasks
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher Watcher;
        Pushpin CurrPin = new Pushpin();
        Pushpin PinTask = new Pushpin();
        static int counter = 0;

        List<Pins> PinList = new List<Pins>();

        public MainPage()
        {
            InitializeComponent();

            MyMap.ZoomBarVisibility = Visibility.Visible;
            MyMap.ZoomLevel = 18;
            MyMap.DoubleTap += new EventHandler<GestureEventArgs>(MyMap_DoubleTap);     
            
            Watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);            
            Watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Watcher_PositionChanged);
            Watcher.Start();        
        }

        void MyMap_DoubleTap(object sender, GestureEventArgs e)
        {
            txtNewTask.Visibility = System.Windows.Visibility.Visible;
            btnCancel.Visibility = System.Windows.Visibility.Visible;
        }


        void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            //GeoCoordinate MyTask = new GeoCoordinate(12.9401512145996, 77.6210422515869);
            counter++;

            CurrPin.Location = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            CurrPin.Opacity = 0.8; CurrPin.Content = "1";

            MyMap.Children.Clear();
            MyMap.Children.Add(CurrPin);

            MyMap.Center = CurrPin.Location;

            foreach (Pins PinTasks in PinList)
            {
                Pushpin NEWPin = new Pushpin();                
                NEWPin.Location = PinTasks.Address;

                if (NEWPin.Location.GetDistanceTo(CurrPin.Location) < 500)
                {
                    NEWPin.Content = PinTasks.Details; NEWPin.Opacity = 0.5; MyMap.Children.Add(NEWPin);
                    //PinTask.Location = NEWPin.Location; PinTask.Opacity = 0.5; PinTask.Content = "My Task";
                    //MyMap.Children.Add(PinTask);
                    MyMap.SetView(CurrPin.Location, 15);
                    txtLocationPopUp.Text = counter + ": MyTask: You are in Proximity! " + NEWPin.Location.GetDistanceTo(CurrPin.Location).ToString();
                    
                }
                else
                {
                    txtLocationPopUp.Text = counter + ":" + CurrPin.Location.ToString() + " | " + NEWPin.Location.GetDistanceTo(CurrPin.Location).ToString();
                }
            }


        }


        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            txtNewTask.Visibility = System.Windows.Visibility.Collapsed;
            btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            Watcher.Start();
            btnNewTask.Visibility = System.Windows.Visibility.Visible;
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn.IsEnabled = true;
        }

        private void txtNewTask_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Pins NewTask = new Pins(txtNewTask.Text, MyMap.Center.Latitude, MyMap.Center.Longitude);
                PinList.Add(NewTask);

                Pushpin MyNewPin = new Pushpin();
                MyNewPin.Content = txtNewTask.Text;
                MyNewPin.Location.Latitude = MyMap.Center.Latitude;
                MyNewPin.Location.Longitude = MyMap.Center.Longitude;
                MyMap.Children.Add(MyNewPin);
                Watcher.Start();
                txtNewTask.Visibility = System.Windows.Visibility.Collapsed;
                btnCancel.Visibility = System.Windows.Visibility.Collapsed;
                btnNewTask.Visibility = System.Windows.Visibility.Visible;
                ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
                btn1.IsEnabled = true;
                ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
                btn2.IsEnabled = false;
                ApplicationBarIconButton btn3 = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
                btn3.IsEnabled = false;
            }
        }

        private void btnNewTask_Click(object sender, RoutedEventArgs e)
        {
            Watcher.Stop();
            btnNewTask.Visibility = System.Windows.Visibility.Collapsed;
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn.IsEnabled = false;
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn2.IsEnabled = true;
            ApplicationBarIconButton btn3 = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            btn3.IsEnabled = true;
        }

        private void btnShowAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (Pins PinTasks in PinList)
            {
                Pushpin NEWPin = new Pushpin();
                NEWPin.Location = PinTasks.Address;
                NEWPin.Content = PinTasks.Details; NEWPin.Opacity = 0.5; MyMap.Children.Add(NEWPin);
                MyMap.SetView(CurrPin.Location, 13);
            }
        }

        private void BtnNewTask_Click(object sender, EventArgs e)
        {
            Watcher.Stop();
            ApplicationBarIconButton btn = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn.IsEnabled = false;
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn2.IsEnabled = true;
            ApplicationBarIconButton btn3 = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            btn3.IsEnabled = true;
            //ApplicationBar.Buttons.IndexOf(BtnNewTask) .IsEnabled = false;
            //BtnNewTask.IsEnabled = false;
            //btnNewTask.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            foreach (Pins PinTasks in PinList)
            {
                Pushpin NEWPin = new Pushpin();
                NEWPin.Location = PinTasks.Address;
                NEWPin.Content = PinTasks.Details; NEWPin.Opacity = 0.5; MyMap.Children.Add(NEWPin);
                //MyMap.SetView( (CurrPin.Location.Latitude,CurrPin.Location.Longitude);
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Pins NewTask = new Pins(txtNewTask.Text, MyMap.Center.Latitude, MyMap.Center.Longitude);
            PinList.Add(NewTask);

            Pushpin MyNewPin = new Pushpin();
            MyNewPin.Content = txtNewTask.Text;
            MyNewPin.Location.Latitude = MyMap.Center.Latitude;
            MyNewPin.Location.Longitude = MyMap.Center.Longitude;
            MyMap.Children.Add(MyNewPin);
            Watcher.Start();
            txtNewTask.Visibility = System.Windows.Visibility.Collapsed;
            btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            btnNewTask.Visibility = System.Windows.Visibility.Visible;
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn1.IsEnabled = true;
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn2.IsEnabled = false;
            ApplicationBarIconButton btn3 = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            btn3.IsEnabled = false;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            txtNewTask.Visibility = System.Windows.Visibility.Collapsed;
            btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            Watcher.Start();
            btnNewTask.Visibility = System.Windows.Visibility.Visible;
            ApplicationBarIconButton btn1 = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            btn1.IsEnabled = true;
            ApplicationBarIconButton btn2 = (ApplicationBarIconButton)ApplicationBar.Buttons[2];
            btn2.IsEnabled = false;
            ApplicationBarIconButton btn3 = (ApplicationBarIconButton)ApplicationBar.Buttons[3];
            btn3.IsEnabled = false;
        }


    }

    public class Pins
    {
        private string details;
        private GeoCoordinate address;

        public Pins(string Message, double Lat, double Lon)
        {
            address = new GeoCoordinate(Lat, Lon);
            details = Message;
        }

        public string Details
        {
            get
            {
                return details;
            }
            set
            {
                details = value;
            }
        }

        public GeoCoordinate Address
        {
            get
            {
                return address;
            }

            set
            {
                //address = new GeoCoordinate(value latitude, longitude);
            }
        }
    }
}