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

namespace Geo_Tasks
{
    public partial class Settings : PhoneApplicationPage
    {
        public Settings()
        {
            InitializeComponent();
            
            sliderLoopDistance.Value = (App.Current as App).LoopDistance;
            txtLoopSize.Text = (App.Current as App).LoopDistance.ToString() + "m";

            if ((App.Current as App).appSettings.Contains("ZoomLevel"))
            {
                SliderZoomLevel.Value = (int)(App.Current as App).appSettings["ZoomLevel"];
                txtZoomLevel.Text = SliderZoomLevel.Value.ToString();
            }

            if ((App.Current as App).appSettings.Contains("PositioningAccuracy"))
            {
                if ((string)(App.Current as App).appSettings["PositioningAccuracy"] == "High")
                {
                    ChkBxPositioningAccuracy.IsChecked = true;
                    (App.Current as App).appSettings["PositioningAccuracy"] = "High";
                }
                else
                {
                    ChkBxPositioningAccuracy.IsChecked = false;
                    (App.Current as App).appSettings["PositioningAccuracy"] = "Default";
                }
            }
            else
            {
                (App.Current as App).appSettings["PositioningAccuracy"] = "High";
                ChkBxPositioningAccuracy.IsChecked = true;
            }

            sliderLoopDistance.ValueChanged += new RoutedPropertyChangedEventHandler<double>(sliderLoopDistance_ValueChanged);
            SliderZoomLevel.ValueChanged += new RoutedPropertyChangedEventHandler<double>(SliderZoomLevel_ValueChanged);
            ChkBxPositioningAccuracy.Click += new RoutedEventHandler(ChkBxPositioningAccuracy_Click);
        }

        void ChkBxPositioningAccuracy_Click(object sender, RoutedEventArgs e)
        {
            if((bool)((CheckBox)sender).IsChecked)
            {
                (App.Current as App).appSettings["PositioningAccuracy"] = "High";
            }
            else
            {
                (App.Current as App).appSettings["PositioningAccuracy"] = "Default";
            }
        }


        void SliderZoomLevel_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            (App.Current as App).appSettings["ZoomLevel"] = (int)e.NewValue;
            txtZoomLevel.Text = e.NewValue.ToString();
        }


        private void sliderLoopDistance_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            
            (App.Current as App).LoopDistance = (int) e.NewValue;
            (App.Current as App).appSettings["LoopDistance"] = (int)e.NewValue;
            txtLoopSize.Text = (App.Current as App).LoopDistance.ToString() + "m";
        }
    }
}