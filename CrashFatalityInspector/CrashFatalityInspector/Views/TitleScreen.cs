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
        private DockPanel content;
        private KinectRegion kRegion;
        //
        private Label title;
        private KinectTileButton startButton;

        public TitleScreen(Window MainWindow, KinectSensorChooser SensorChooser)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Initialize display containers
            this.content = CreateDockPanel();
            this.kRegion = CreateKinectRegion();
            // Initialize display elements
            this.title = CreateLabel();
            this.startButton = CreateStartButton();
            // Set up the display
            this.kRegion.Content = this.startButton;
            this.content.Children.Add(title);
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

        private DockPanel CreateDockPanel()
        {
            DockPanel container = new DockPanel();
            container.Name = Constants.ViewNames.TitleScreen.ToString();
            container.SizeChanged += ContentSizeChanged;
            return container;
        }

        void ContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.title.FontSize = this.content.ActualHeight / 10;
        }

        private KinectRegion CreateKinectRegion()
        {
            return new KinectRegion();
        }

        private Label CreateLabel()
        {
            Label label = new Label();
            label.Content = Constants.CFI_TITLE_LABEL;
            label.Foreground = Brushes.White;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            DockPanel.SetDock(label, Dock.Top);
            return label;
        }

        private KinectTileButton CreateStartButton()
        {
            KinectTileButton button = new KinectTileButton();
            button.Content = Constants.CFI_TITLE_BUTTON_LABEL;
            button.Foreground = Brushes.White;
            button.HorizontalLabelAlignment = System.Windows.HorizontalAlignment.Center;
            button.VerticalLabelAlignment = System.Windows.VerticalAlignment.Center;
            button.Click += StartButtonClick;
            return button;
        }

        private void StartButtonClick(object sender, RoutedEventArgs e)
        {
#if (DEBUG)
            Console.WriteLine("Start button clicked!");
#endif
            // Uncomment two statements below for release version
            //TimeZoneScreen timeZoneScreen = new TimeZoneScreen(this.mainWindow, this.sensorChooser);
            //timeZoneScreen.Show();

            // We can jump straight to the states selection screen for debugging without Kinect...
            // TODO: Remove for release version.
            StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser,
                Constants.TimeZones.Central);
            ss.Show();
        }
    }
}
