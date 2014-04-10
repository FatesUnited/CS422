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
    class VehicleScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private DockPanel content;
        private KinectRegion kRegion;
        private Grid buttonGrid;
        //
        private Label title;
        //
        private Utilities.State state;

        public VehicleScreen(Window MainWindow, KinectSensorChooser SensorChooser, Utilities.State State)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Remember some info
            this.state = State;
            // Initialize display containers
            this.content = CreateDockPanel();
            this.kRegion = CreateKinectRegion();
            // Initialize display elements
            this.title = CreateLabel();
            // Set up the display
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
            container.Name = Constants.ViewNames.StatesScreen.ToString();
            container.SizeChanged += ContentSizeChanged;
            return container;
        }

        void ContentSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.title.FontSize = this.content.ActualHeight / 10;
        }

        private Label CreateLabel()
        {
            Label label = new Label();
            label.Content = Constants.CFI_VEHICLES_LABEL;
            label.Foreground = Brushes.White;
            label.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            label.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            DockPanel.SetDock(label, Dock.Top);
            return label;
        }

        private KinectRegion CreateKinectRegion()
        {
            this.kRegion = new KinectRegion();
            this.buttonGrid = new Grid();
            this.buttonGrid.ShowGridLines = true;
            ColumnDefinition leftColumn = new ColumnDefinition();
            ColumnDefinition rightColumn = new ColumnDefinition();
            this.buttonGrid.ColumnDefinitions.Add(leftColumn);
            this.buttonGrid.ColumnDefinitions.Add(rightColumn);
            RowDefinition topRow = new RowDefinition();
            RowDefinition bottomRow = new RowDefinition();
            this.buttonGrid.RowDefinitions.Add(topRow);
            this.buttonGrid.RowDefinitions.Add(bottomRow);
            KinectTileButton largeTrucks = CreateTileButton(Constants.VEHICLE_TYPE_LARGE_TRUCKS);
            KinectTileButton buses = CreateTileButton(Constants.VEHICLE_TYPE_BUSES);
            KinectTileButton motorCoaches = CreateTileButton(Constants.VEHICLE_TYPE_MOTOR_COACHES);
            KinectTileButton allVehicles = CreateTileButton(Constants.VEHICLE_TYPE_ALL_VEHICLES);
            Grid.SetRow(largeTrucks, 0);
            Grid.SetColumn(largeTrucks, 0);
            Grid.SetRow(buses, 0);
            Grid.SetColumn(buses, 1);
            Grid.SetRow(motorCoaches, 1);
            Grid.SetColumn(motorCoaches, 0);
            Grid.SetRow(allVehicles, 1);
            Grid.SetColumn(allVehicles, 1);
            this.buttonGrid.Children.Add(largeTrucks);
            this.buttonGrid.Children.Add(buses);
            this.buttonGrid.Children.Add(motorCoaches);
            this.buttonGrid.Children.Add(allVehicles);
            kRegion.Content = this.buttonGrid;
            return kRegion;
        }

        private KinectTileButton CreateTileButton(string Name)
        {
            KinectTileButton ktb = new KinectTileButton();
            ktb.Content = Name;
            ktb.Click += VehicleClick;
            ktb.HorizontalLabelAlignment = System.Windows.HorizontalAlignment.Center;
            ktb.VerticalLabelAlignment = System.Windows.VerticalAlignment.Center;
            ktb.Foreground = Brushes.White;
            return ktb;
        }

        void VehicleClick(object sender, RoutedEventArgs e)
        {
            string vehicleType = (string)((KinectTileButton)sender).Content;
#if (DEBUG)
            Console.WriteLine(vehicleType + " selected!");
#endif
            DataScreen ds = new DataScreen(this.mainWindow, this.sensorChooser, this.state, vehicleType);
        }
    }
}
