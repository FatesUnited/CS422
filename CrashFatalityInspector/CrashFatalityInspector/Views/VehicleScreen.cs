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
    class VehicleScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private Grid content;
        private KinectRegion kRegion;
        //
        private Grid infoGrid;
        private Grid buttonGrid;
        //
        private Utilities.State state;
        //
        private Binding regionSensorBinding;

        public VehicleScreen(Window MainWindow, KinectSensorChooser SensorChooser, Utilities.State State)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Remember some info
            this.state = State;
            // Initialize display containers
            CreateContentGrid();
            CreateInfoGrid();
            CreateKinectRegion();
            //
            Grid.SetColumn(this.infoGrid, 0);
            Grid.SetRow(this.infoGrid, 0);
            this.content.Children.Add(this.infoGrid);
            //
            Grid.SetColumn(this.kRegion, 0);
            Grid.SetRow(this.kRegion, 1);
            this.content.Children.Add(this.kRegion);
            // Bind the Kinect sensor
            //var regionSensorBinding = new Binding("Kinect") { Source = SensorChooser };
            this.regionSensorBinding = new Binding("Kinect") { Source = SensorChooser };
            BindingOperations.SetBinding(this.kRegion, KinectRegion.KinectSensorProperty, this.regionSensorBinding);
        }

        public void Show()
        {
            // Display this screen
            this.mainWindow.Content = this.content;
        }

        private void CreateContentGrid()
        {
            this.content = new Grid();
            this.content.Name = Constants.ViewNames.StatesScreen.ToString();
            ImageBrush iBrush = new ImageBrush();
            iBrush.ImageSource = new BitmapImage(new Uri(Constants.CFI_VEHICLES_IMAGE, UriKind.Relative));
            this.content.Background = iBrush;
            ColumnDefinition cd = new ColumnDefinition();
            this.content.ColumnDefinitions.Add(cd);
            RowDefinition top = new RowDefinition();
            RowDefinition bot = new RowDefinition();
            this.content.RowDefinitions.Add(top);
            this.content.RowDefinitions.Add(bot);
            this.content.SizeChanged += contentGridSizeChanged;
        }

        void contentGridSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ColumnDefinition left = this.infoGrid.ColumnDefinitions[0];
            left.Width = new GridLength(20 + this.mainWindow.ActualWidth / 4.8611111111111111111111111111111);
            Grid dataGrid = (Grid)this.infoGrid.Children[1];
            Label regionLabel = (Label)dataGrid.Children[0];
            regionLabel.FontSize = this.mainWindow.ActualHeight / 10;
            Label regionInfo = (Label)dataGrid.Children[1];
            regionInfo.FontSize = this.mainWindow.ActualHeight / 23.333333333333333333333333333333;
            foreach (KinectTileButton ktb in this.buttonGrid.Children)
            {
                ktb.MaxHeight = this.mainWindow.ActualHeight / 3.75;
                ktb.MaxWidth = this.mainWindow.ActualHeight / 3.75;
                ktb.FontSize = this.mainWindow.ActualHeight / 22;
            }
        }

        private void CreateInfoGrid()
        {
            this.infoGrid = new Grid();
            ColumnDefinition left = new ColumnDefinition();
            ColumnDefinition right = new ColumnDefinition();
            RowDefinition rd = new RowDefinition();
            //
            left.Width = new GridLength(20 + this.mainWindow.ActualWidth / 4.8611111111111111111111111111111);
            //
            this.infoGrid.ColumnDefinitions.Add(left);
            this.infoGrid.ColumnDefinitions.Add(right);
            this.infoGrid.RowDefinitions.Add(rd);
            //
            Image stateImage = new Image();
            BitmapImage stateBitmap = new BitmapImage();
            stateBitmap.BeginInit();
            stateBitmap.UriSource = new Uri(@"Images/States/" + this.state.Image, UriKind.Relative);
            stateBitmap.EndInit();
            stateImage.Source = stateBitmap;
            stateImage.Stretch = Stretch.Uniform;
            stateImage.Margin = new Thickness(20, 0, 0, 0);
            //
            Grid.SetColumn(stateImage, 0);
            Grid.SetRow(stateImage, 0);
            stateImage.HorizontalAlignment = HorizontalAlignment.Left;
            this.infoGrid.Children.Add(stateImage);
            //
            Grid dataGrid = new Grid();
            ColumnDefinition cd = new ColumnDefinition();
            dataGrid.ColumnDefinitions.Add(cd);
            RowDefinition top = new RowDefinition();
            RowDefinition bot = new RowDefinition();
            //
            top.Height = new GridLength(this.mainWindow.ActualHeight / 8);
            //
            dataGrid.RowDefinitions.Add(top);
            dataGrid.RowDefinitions.Add(bot);
            //
            Label regionLabel = new Label();
            regionLabel.Content = this.state.Name + " _";
            regionLabel.Margin = new Thickness(0, 0, 20, 0);
            regionLabel.FontSize = this.mainWindow.ActualHeight / 15;
            regionLabel.FontFamily = new FontFamily("Veranda");
            regionLabel.Foreground = Brushes.White;
            regionLabel.HorizontalAlignment = HorizontalAlignment.Right;
            regionLabel.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumn(regionLabel, 0);
            Grid.SetRow(regionLabel, 0);
            dataGrid.Children.Add(regionLabel);
            //
            Label regionInfo = new Label();
            regionInfo.Content = "+population:\t\t" + this.state.Population + "\r\n+poplation rank:\t\t" + this.state.PopulationRank + "\r\n+road milage:\t\t" + this.state.Milage + "\r\n+road milage rank:\t" + this.state.MilageRank + "\r\n+select a vehicle type";
            regionInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            regionInfo.VerticalContentAlignment = VerticalAlignment.Bottom;
            regionInfo.FontSize = this.mainWindow.ActualHeight / 30;
            regionInfo.Margin = new Thickness(0, 0, 20, 0);
            regionInfo.FontFamily = new FontFamily("Veranda");
            regionInfo.Foreground = Brushes.White;
            regionInfo.HorizontalAlignment = HorizontalAlignment.Right;
            regionInfo.VerticalAlignment = VerticalAlignment.Bottom;
            Grid.SetColumn(regionInfo, 0);
            Grid.SetRow(regionInfo, 1);
            dataGrid.Children.Add(regionInfo);
            //
            Grid.SetColumn(dataGrid, 1);
            Grid.SetRow(dataGrid, 0);
            this.infoGrid.Children.Add(dataGrid);
        }

        private void CreateKinectRegion()
        {
            this.kRegion = new KinectRegion();
            this.buttonGrid = new Grid();
            this.buttonGrid.ShowGridLines = false;
            for (int i = 0; i < 5; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                this.buttonGrid.ColumnDefinitions.Add(cd);
            }
            RowDefinition rd = new RowDefinition();
            this.buttonGrid.RowDefinitions.Add(rd);
            //
            KinectTileButton largeTrucks = CreateTileButton(Constants.VEHICLE_TYPE_LARGE_TRUCKS);
            KinectTileButton buses = CreateTileButton(Constants.VEHICLE_TYPE_BUSES);
            KinectTileButton motorCoaches = CreateTileButton(Constants.VEHICLE_TYPE_MOTOR_COACHES);
            KinectTileButton allVehicles = CreateTileButton(Constants.VEHICLE_TYPE_ALL_VEHICLES);
            //
            KinectTileButton goBackButton = new KinectTileButton();
            goBackButton.Content = "go back";
            goBackButton.MaxHeight = this.mainWindow.ActualHeight / 3.75;
            goBackButton.MaxWidth = this.mainWindow.ActualHeight / 3.75;
            goBackButton.FontSize = this.mainWindow.ActualHeight / 22;
            goBackButton.Click += GoBackButtonClick;
            goBackButton.Foreground = Brushes.White;
            goBackButton.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            //
            Grid.SetRow(largeTrucks, 0);
            Grid.SetColumn(largeTrucks, 0);
            Grid.SetRow(buses, 0);
            Grid.SetColumn(buses, 1);
            Grid.SetRow(motorCoaches, 0);
            Grid.SetColumn(motorCoaches, 2);
            Grid.SetRow(allVehicles, 0);
            Grid.SetColumn(allVehicles, 3);
            Grid.SetRow(goBackButton, 0);
            Grid.SetColumn(goBackButton, 4);
            //
            this.buttonGrid.Children.Add(largeTrucks);
            this.buttonGrid.Children.Add(buses);
            this.buttonGrid.Children.Add(motorCoaches);
            this.buttonGrid.Children.Add(allVehicles);
            this.buttonGrid.Children.Add(goBackButton);
            this.kRegion.Content = this.buttonGrid;
        }

        private KinectTileButton CreateTileButton(string Name)
        {
            KinectTileButton ktb = new KinectTileButton();
            ktb.Content = Name;
            ktb.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            ktb.MaxHeight = this.mainWindow.ActualHeight / 3.75;
            ktb.MaxWidth = this.mainWindow.ActualHeight / 3.75;
            ktb.FontSize = this.mainWindow.ActualHeight / 22;
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
            // Clear the binding
            BindingOperations.ClearBinding(this.kRegion, KinectRegion.KinectSensorProperty);
            // Show the desired screen
            ds.Show();
        }

        void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
#if (DEBUG)
            Console.WriteLine("User went back to states screen!");
#endif
            StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser, this.state.TimeZone);
            // Clear the binding
            BindingOperations.ClearBinding(this.kRegion, KinectRegion.KinectSensorProperty);
            // Show the desired screen
            ss.Show();
        }
    }
}
