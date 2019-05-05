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
using Microsoft.Xna.Framework;
using System.Windows.Media.Imaging;

namespace Geo_Tasks
{
    public partial class MainPage : PhoneApplicationPage
    {
        GeoCoordinateWatcher Watcher;
        Pushpin CurrPin = new Pushpin();

        // Constructor
        public MainPage()
        {
            InitializeComponent(); //ApplicationBar = new ApplicationBar();

            this.ApplicationBar = (ApplicationBar)App.Current.Resources["AppBar"];

            MyMap.ZoomLevel = (int)(App.Current as App).appSettings["ZoomLevel"];
            Watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.High);
            Watcher.PositionChanged += new EventHandler<GeoPositionChangedEventArgs<GeoCoordinate>>(Watcher_PositionChanged);
            Watcher.Start();
        }

        void Watcher_PositionChanged(object sender, GeoPositionChangedEventArgs<GeoCoordinate> e)
        {
            
            //ViewModel.AccuracyLocationCollection = DrawMapsCircle(e.Position.Location);

            CurrPin.Location = new GeoCoordinate(e.Position.Location.Latitude, e.Position.Location.Longitude);
            CurrPin.Opacity = 0.8; CurrPin.Content = "We are here!";
            MapLayer.SetPosition(MapPoly, CurrPin.Location);
            MapLayer.SetPosition(ppLocation, CurrPin.Location);
            
            ppLocation.Visibility = Visibility.Visible;
            MapPoly.Visibility = Visibility.Visible;
            MyMap.Children.Clear();
            MyMap.Children.Add(CurrPin);

            /*
            Image image = new Image();
            //image.Source = new BitmapImage();

            BitmapImage bi = new BitmapImage();
            Ellipse MyCircle = new Ellipse();
            
            
            MyCircle.Width = 20; MyCircle.Height = 20;

            bi.UriSource = new Uri(@"/Images/appbar.basecircle.rest.png", UriKind.RelativeOrAbsolute);
            image.Source = bi;
            //ImageSource imgsrc = @"Images/appbar.add.rest.png"
            //image.Source = new ImageSource (@"Images/appbar.add.rest.png");
            image.Width = 40;
            image.Height = 40; 
            
            MapLayer.SetPosition(image, CurrPin.Location);
            MapLayer layer = new MapLayer();
            layer.Children.Add(image);
            layer.Children.Add(MyCircle);
            MyMap.Children.Add(layer);
            */
            MyMap.Center = CurrPin.Location;

            //foreach(Pins PinTasks in (App.Current as App).PinList)
            (App.Current as App).PinList.ForEach(delegate(Pins PinTasks)
            {
                Pushpin NEWPin = new Pushpin();
                NEWPin.Location = PinTasks.Address;

                if (NEWPin.Location.GetDistanceTo(CurrPin.Location) < (App.Current as App).LoopDistance)
                {
                    NEWPin.Content = PinTasks.Details; NEWPin.Opacity = 0.5; MyMap.Children.Add(NEWPin);
                    //PinTask.Location = NEWPin.Location; PinTask.Opacity = 0.5; PinTask.Content = "My Task";
                    //MyMap.Children.Add(PinTask);
                    MyMap.SetView(CurrPin.Location, (int)(App.Current as App).appSettings["ZoomLevel"]);
                    //txtLocationPopUp.Text = counter + ": MyTask: You are in Proximity! " + NEWPin.Location.GetDistanceTo(CurrPin.Location).ToString();
                }
                else
                {
                    //txtLocationPopUp.Text = counter + ":" + CurrPin.Location.ToString() + " | " + NEWPin.Location.GetDistanceTo(CurrPin.Location).ToString();
                }
            });
        }

        /*
        private static double ToDegrees(double radians)
        {
            return radians * (180 / Math.PI);
        }

        private static double ToRadian(this double val)
        {
            return (Math.PI / 180) * val;
        }

        public static LocationCollection DrawMapsCircle(GeoCoordinate location)
        {
            double earthRadiusInMeters = 6367.0 * 1000.0;
            var lat = ToRadian(location.Latitude);
            var lng = ToRadian(location.Longitude);
            var d = location.HorizontalAccuracy / earthRadiusInMeters;

            var locations = new LocationCollection();

            for (var x = 0; x <= 360; x++)
            {
                var brng = ToRadian(x);
                var latRadians = Math.Asin(Math.Sin(lat) * Math.Cos(d) + Math.Cos(lat) * Math.Sin(d) * Math.Cos(brng));
                var lngRadians = lng + Math.Atan2(Math.Sin(brng) * Math.Sin(d) * Math.Cos(lat), Math.Cos(d) - Math.Sin(lat) * Math.Sin(latRadians));

                locations.Add(new GeoCoordinate()
                {
                    Latitude = ToDegrees(latRadians),
                    Longitude = ToDegrees(lngRadians)
                });
            }

            return locations;
        }
        */

    }
}