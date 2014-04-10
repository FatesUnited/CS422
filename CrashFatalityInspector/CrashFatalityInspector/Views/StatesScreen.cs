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
    class StatesScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private DockPanel content;
        private KinectRegion kRegion;
        //
        private Label title;
        private KinectScrollViewer statesKSV;
        //
        private List<Utilities.State> states;

        public StatesScreen(Window MainWindow, KinectSensorChooser SensorChooser, Constants.TimeZones TimeZone)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Populate the states based on TimeZone parameter
            this.states = PopulateStates(TimeZone);
#if (DEBUG)
            Console.WriteLine(TimeZone.ToString() + " states loading...");
            foreach (Utilities.State s in this.states)
            {
                Console.WriteLine(s);
            }
            Console.WriteLine("..." + TimeZone.ToString() + " states loaded!");
#endif
            // Initialize display containers
            this.content = CreateDockPanel();
            this.kRegion = CreateKinectRegion();
            // Initialize display elements
            this.title = CreateLabel();
            this.statesKSV = CreateKinectScrollViewer();
            // Set up the display
            this.kRegion.Content = this.statesKSV;
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

        private List<Utilities.State> PopulateStates(Constants.TimeZones TimeZone)
        {
            List<Utilities.State> states = new List<Utilities.State>();
            foreach (Utilities.State s in Utilities.Data.States)
            {
                if (s.TimeZone == TimeZone)
                {
                    states.Add(s);
                }
            }
            return states;
        }

        private DockPanel CreateDockPanel()
        {
            DockPanel container = new DockPanel();
            container.Name = Constants.ViewNames.StatesScreen.ToString();
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
            label.Content = Constants.CFI_STATES_LABEL;
            label.Foreground = Brushes.White;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            DockPanel.SetDock(label, Dock.Top);
            return label;
        }

        private KinectScrollViewer CreateKinectScrollViewer()
        {
            KinectScrollViewer ksv = new KinectScrollViewer();
#if (DEBUG)
            ksv.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
#else
            ksv.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
#endif
            ksv.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //
            Grid g = new Grid();
            g.ShowGridLines = true;
            g.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            g.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            for (int i = 0; i < this.states.Count; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                g.ColumnDefinitions.Add(cd);
            }
            RowDefinition rd = new RowDefinition();
            g.RowDefinitions.Add(rd);
            for (int i = 0; i < this.states.Count; i++)
            {
                KinectTileButton ktb = new KinectTileButton();
                ktb.Content = this.states[i];
                ktb.Click += StateButtonClick;
                ktb.HorizontalLabelAlignment = System.Windows.HorizontalAlignment.Center;
                ktb.VerticalLabelAlignment = System.Windows.VerticalAlignment.Center;
                ktb.Foreground = Brushes.White;
                //
                Grid.SetRow(ktb, 0);
                Grid.SetColumn(ktb, i);
                g.Children.Add(ktb);
            }
            ksv.Content = g;
            this.kRegion.Content = ksv;
            DockPanel.SetDock(this.kRegion, Dock.Bottom);
            //
            return ksv;
        }

        void StateButtonClick(object sender, RoutedEventArgs e)
        {
            Utilities.State s = (Utilities.State)((KinectTileButton)sender).Content;
#if (DEBUG)
            Console.WriteLine(s.Name + " selected!");
#endif
            VehicleScreen vs = new VehicleScreen(this.mainWindow, this.sensorChooser, s);
            vs.Show();
        }
    }
}
