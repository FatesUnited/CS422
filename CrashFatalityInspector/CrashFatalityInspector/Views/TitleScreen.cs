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
    class TitleScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private Grid content;
        private KinectRegion kRegion;
        //
        private KinectTileButton startButton;
        //
        private double sizeValue;

        public TitleScreen(Window MainWindow, KinectSensorChooser SensorChooser)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Initialize display containers
            CreateContentGrid();
            CreateKinectRegion();
            // Initialize display elements
            CreateStartButton();
            // Set up the display
            this.kRegion.Content = this.startButton;
            this.content.Children.Add(kRegion);
            // Bind the Kinect sensor
            var regionSensorBinding = new Binding("Kinect") { Source = SensorChooser };
            BindingOperations.SetBinding(this.kRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        public void Show()
        {
            // Display this screen
            this.mainWindow.Content = this.content;
        }

        private void CreateContentGrid()
        {
            this.content = new Grid();
            this.content.Name = Constants.ViewNames.TitleScreen.ToString();
            ImageBrush iBrush = new ImageBrush();
            iBrush.ImageSource = new BitmapImage(new Uri(Constants.CFI_TITLE_IMAGE, UriKind.Relative));
            this.content.Background = iBrush;
            this.content.SizeChanged += ContentGridSizeChanged;
        }

        void ContentGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.sizeValue = this.mainWindow.ActualWidth <= this.mainWindow.ActualHeight ? this.mainWindow.ActualWidth : this.mainWindow.ActualHeight;
            this.startButton.Width = this.sizeValue / 3.5;
            this.startButton.Height = this.sizeValue / 3.5;
            this.startButton.FontSize = this.mainWindow.ActualHeight / 17.5;
        }

        private void CreateKinectRegion()
        {
            this.kRegion = new KinectRegion();
        }

        private void CreateStartButton()
        {
            this.startButton = new KinectTileButton();
            this.startButton.Content = Constants.CFI_TITLE_BUTTON_LABEL;
            this.startButton.FontFamily = new FontFamily("Veranda");
            this.startButton.Foreground = Brushes.White;
            this.startButton.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            this.startButton.FontSize = this.mainWindow.Height / 17.5;
            this.startButton.HorizontalAlignment = HorizontalAlignment.Right;
            this.startButton.Margin = new Thickness(this.mainWindow.Width / 21);
            this.startButton.HorizontalLabelAlignment = System.Windows.HorizontalAlignment.Center;
            this.startButton.VerticalLabelAlignment = System.Windows.VerticalAlignment.Center;
            this.startButton.Click += StartButtonClick;
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
#if (DEBUG)
            Console.WriteLine("Start button clicked!");
#endif
            ButtonZonesScreen bzs = new ButtonZonesScreen(this.mainWindow, this.sensorChooser);
            bzs.Show();
        }
    }
}
