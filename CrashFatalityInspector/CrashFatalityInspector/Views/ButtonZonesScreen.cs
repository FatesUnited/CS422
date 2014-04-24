using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace CrashFatalityInspector.Views
{
    class ButtonZonesScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private Grid content;
        private KinectRegion kRegion;
        //
        private KinectTileButton pacificButton;
        private KinectTileButton mountainButton;
        private KinectTileButton centralButton;
        private KinectTileButton easternButton;

        public ButtonZonesScreen(Window MainWindow, KinectSensorChooser SensorChooser)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Initialize display containers
            CreateContentGrid();
            CreateKinectRegion();
            // Set up the display
            this.kRegion.Content = this.content;
            // Bind the Kinect sensor
            var regionSensorBinding = new Binding("Kinect") { Source = SensorChooser };
            BindingOperations.SetBinding(this.kRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        public void Show()
        {
            // Display this screen
            this.mainWindow.Content = this.kRegion;
        }

        private void CreateContentGrid()
        {
            this.content = new Grid();
            this.content.Name = Constants.ViewNames.TimeZoneScreen.ToString();
            ImageBrush iBrush = new ImageBrush();
            iBrush.ImageSource = new BitmapImage(new Uri(Constants.CFI_BUTTON_ZONE_IMAGE, UriKind.Relative));
            this.content.Background = iBrush;
            this.content.SizeChanged += ContentGridSizeChanged;
            //
            for (int i = 0; i < 4; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                cd.Width = new GridLength(this.mainWindow.ActualWidth / 4);
                this.content.ColumnDefinitions.Add(cd);
            }
            //
            this.pacificButton = InitializeKinectTileButton(Constants.TimeZones.Pacific);
            this.mountainButton = InitializeKinectTileButton(Constants.TimeZones.Mountain);
            this.centralButton = InitializeKinectTileButton(Constants.TimeZones.Central);
            this.easternButton = InitializeKinectTileButton(Constants.TimeZones.Eastern);
            //
            Grid.SetColumn(this.pacificButton, 0);
            Grid.SetColumn(this.mountainButton, 1);
            Grid.SetColumn(this.centralButton, 2);
            Grid.SetColumn(this.easternButton, 3);
            //
            this.content.Children.Add(this.pacificButton);
            this.content.Children.Add(this.mountainButton);
            this.content.Children.Add(this.centralButton);
            this.content.Children.Add(this.easternButton);
        }

        void ContentGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (ColumnDefinition cd in this.content.ColumnDefinitions)
            {
                cd.Width = new GridLength(this.mainWindow.ActualWidth / 4);
            }
            foreach (KinectTileButton ktb in this.content.Children)
            {
                ktb.Width = this.mainWindow.ActualWidth / 4;
                ktb.Height = this.mainWindow.ActualHeight;
            }
        }

        private KinectTileButton InitializeKinectTileButton(Constants.TimeZones TimeZone)
        {
            KinectTileButton ktb = new KinectTileButton();
            ktb.Name = TimeZone.ToString();
            ktb.Height = this.mainWindow.ActualHeight;
            ktb.Background = new LinearGradientBrush(Color.FromRgb(200, 200, 200), Color.FromRgb(77, 77, 77), 90);
            ktb.Opacity = 0;
            ktb.BorderThickness = new Thickness(0);
            ktb.Padding = new Thickness(0);
            ktb.Margin = new Thickness(0);
            ktb.HorizontalAlignment = HorizontalAlignment.Center;
            ktb.VerticalAlignment = VerticalAlignment.Center;
            // TODO: We also need to add event handlers for the kinect sensor cursor...
            // We only have the mouse events (for debugging) set up currently!
            // Is this called the stylus, etc.?
            ktb.MouseEnter += ButtonMouseEnter;
            ktb.MouseLeave += ButtonMouseLeave;
            ktb.Click += ZoneButtonClick;
            return ktb;
        }

        void ButtonMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((KinectTileButton)sender).Opacity = 0.25;
        }

        void ButtonMouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            ((KinectTileButton)sender).Opacity = 0;
        }

        void ZoneButtonClick(object sender, RoutedEventArgs e)
        {
            if (((KinectTileButton)sender).Name == Constants.TimeZones.Pacific.ToString())
            {
#if (DEBUG)
                Console.WriteLine("User clicked " + Constants.TimeZones.Pacific.ToString());
#endif
                StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser, Constants.TimeZones.Pacific);
                ss.Show();
            }
            if (((KinectTileButton)sender).Name == Constants.TimeZones.Mountain.ToString())
            {
#if (DEBUG)
                Console.WriteLine("User clicked " + Constants.TimeZones.Mountain.ToString());
#endif
                StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser, Constants.TimeZones.Mountain);
                ss.Show();
            }
            if (((KinectTileButton)sender).Name == Constants.TimeZones.Central.ToString())
            {
#if (DEBUG)
                Console.WriteLine("User clicked " + Constants.TimeZones.Central.ToString());
#endif
                StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser, Constants.TimeZones.Central);
                ss.Show();
            }
            if (((KinectTileButton)sender).Name == Constants.TimeZones.Eastern.ToString())
            {
#if (DEBUG)
                Console.WriteLine("User clicked " + Constants.TimeZones.Eastern.ToString());
#endif
                StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser, Constants.TimeZones.Eastern);
                ss.Show();
            }
        }

        private void CreateKinectRegion()
        {
            this.kRegion = new KinectRegion();
        }
    }
}
