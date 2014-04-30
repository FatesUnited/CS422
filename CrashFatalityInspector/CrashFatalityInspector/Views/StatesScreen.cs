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
    class StatesScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private Grid content;
        private KinectRegion kRegion;
        //
        private Grid infoGrid;
        private KinectScrollViewer statesKSV;
        private Grid buttonGrid;
        //
        private List<Utilities.State> states;
        private Constants.TimeZones timeZone;
        //
        private Binding regionSensorBinding;

        public StatesScreen(Window MainWindow, KinectSensorChooser SensorChooser, Constants.TimeZones TimeZone)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Populate the states based on TimeZone parameter
            this.timeZone = TimeZone;
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
            CreateContentGrid();
            CreateKinectRegion();
            CreateInfoGrid();
            CreateKinectScrollViewer();
            // Set up the display
            Grid.SetColumn(this.infoGrid, 0);
            Grid.SetRow(this.infoGrid, 0);
            this.content.Children.Add(this.infoGrid);
            this.kRegion.Content = this.statesKSV;
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

        private void CreateContentGrid()
        {
            this.content = new Grid();
            this.content.Name = Constants.ViewNames.StatesScreen.ToString();
            ImageBrush iBrush = new ImageBrush();
            iBrush.ImageSource = new BitmapImage(new Uri(Constants.CFI_STATES_IMAGE, UriKind.Relative));
            this.content.Background = iBrush;
            ColumnDefinition cd = new ColumnDefinition();
            this.content.ColumnDefinitions.Add(cd);
            RowDefinition top = new RowDefinition();
            RowDefinition bot = new RowDefinition();
            this.content.RowDefinitions.Add(top);
            this.content.RowDefinitions.Add(bot);
            this.content.SizeChanged += ContentGridSizeChanged;
        }

        void ContentGridSizeChanged(object sender, SizeChangedEventArgs e)
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

        private void CreateKinectRegion()
        {
            this.kRegion = new KinectRegion();
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
            Image regionImage = new Image();
            BitmapImage regionBitmap = new BitmapImage();
            regionBitmap.BeginInit();
            if (this.timeZone == Constants.TimeZones.Pacific)
            {
                regionBitmap.UriSource = new Uri(Constants.CFI_STATES_PACIFIC_IMAGE, UriKind.Relative);
            }
            if (this.timeZone == Constants.TimeZones.Mountain)
            {
                regionBitmap.UriSource = new Uri(Constants.CFI_STATES_MOUNTAIN_IMAGE, UriKind.Relative);
            }
            if (this.timeZone == Constants.TimeZones.Central)
            {
                regionBitmap.UriSource = new Uri(Constants.CFI_STATES_CENTRAL_IMAGE, UriKind.Relative);
            }
            if (this.timeZone == Constants.TimeZones.Eastern)
            {
                regionBitmap.UriSource = new Uri(Constants.CFI_STATES_EASTERN_IMAGE, UriKind.Relative);
            }
            regionBitmap.EndInit();
            regionImage.Source = regionBitmap;
            regionImage.Stretch = Stretch.Uniform;
            regionImage.Margin = new Thickness(20, 0, 0, 0);
            //
            Grid.SetColumn(regionImage, 0);
            Grid.SetRow(regionImage, 0);
            regionImage.HorizontalAlignment = HorizontalAlignment.Left;
            this.infoGrid.Children.Add(regionImage);
            //
            Grid dataGrid = new Grid();
            ColumnDefinition cd = new ColumnDefinition();
            dataGrid.ColumnDefinitions.Add(cd);
            RowDefinition top = new RowDefinition();
            RowDefinition bot = new RowDefinition();
            dataGrid.RowDefinitions.Add(top);
            dataGrid.RowDefinitions.Add(bot);
            //
            Label regionLabel = new Label();
            regionLabel.Content = this.timeZone.ToString() + " region _";
            regionLabel.Margin = new Thickness(0, 0, 20, 0);
            regionLabel.FontSize = this.mainWindow.ActualHeight / 10;
            regionLabel.FontFamily = new FontFamily("Veranda");
            regionLabel.Foreground = Brushes.White;
            regionLabel.HorizontalAlignment = HorizontalAlignment.Right;
            regionLabel.VerticalAlignment = VerticalAlignment.Top;
            Grid.SetColumn(regionLabel, 0);
            Grid.SetRow(regionLabel, 0);
            dataGrid.Children.Add(regionLabel);
            //
            Label regionInfo = new Label();
            //regionInfo.Content = "+some additional info\r\n+some additional info\r\n+some additional info";
            regionInfo.HorizontalContentAlignment = HorizontalAlignment.Right;
            regionInfo.Content = "+select a state";
            regionInfo.FontSize = this.mainWindow.ActualHeight / 23.333333333333333333333333333333;
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

        private void CreateKinectScrollViewer()
        {
            this.statesKSV = new KinectScrollViewer();
            this.statesKSV.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            this.statesKSV.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            //
            this.buttonGrid = new Grid();
            for (int j = 0; j < this.states.Count + 1; j++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                this.buttonGrid.ColumnDefinitions.Add(cd);
            }
            RowDefinition rd = new RowDefinition();
            this.buttonGrid.RowDefinitions.Add(rd);
            int i = 0;
            for (i = 0; i < this.states.Count; i++)
            {
                KinectTileButton ktb = new KinectTileButton();
                ktb.Content = this.states[i];
                ktb.FontFamily = new FontFamily("Veranda");
                ktb.MaxHeight = this.mainWindow.ActualHeight / 3.75;
                ktb.MaxWidth = this.mainWindow.ActualHeight / 3.75;
                ktb.FontSize = this.mainWindow.ActualHeight / 22;
                ktb.Click += StateButtonClick;
                //
                ImageBrush iBrush = new ImageBrush();
                iBrush.ImageSource = new BitmapImage(new Uri(@"Images/States/" + this.states[i].Image, UriKind.Relative));
                iBrush.Opacity = 0.75;
                if (iBrush.ImageSource.Width > ktb.Width || iBrush.ImageSource.Height > ktb.Height)
                {
                    iBrush.Stretch = Stretch.Uniform;
                }
                else
                {
                    iBrush.Stretch = Stretch.None;
                }
                //
                ktb.Background = iBrush;
                ktb.Foreground = Brushes.White;
                //
                Grid.SetRow(ktb, 0);
                Grid.SetColumn(ktb, i);
                this.buttonGrid.Children.Add(ktb);
            }
            //
            KinectTileButton goBackButton = new KinectTileButton();
            goBackButton.Content = "go back";
            goBackButton.MaxHeight = this.mainWindow.ActualHeight / 3.75;
            goBackButton.MaxWidth = this.mainWindow.ActualHeight / 3.75;
            goBackButton.FontSize = this.mainWindow.ActualHeight / 22;
            goBackButton.Click += GoBackButtonClick;
            goBackButton.Foreground = Brushes.White;
            goBackButton.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            Grid.SetRow(goBackButton, 0);
            Grid.SetColumn(goBackButton, i + 1);
            this.buttonGrid.Children.Add(goBackButton);
            //
            this.statesKSV.Content = this.buttonGrid;
        }

        void GoBackButtonClick(object sender, RoutedEventArgs e)
        {
#if (DEBUG)
            Console.WriteLine("User went back to time zone screen!");
#endif
            ButtonZonesScreen bzs = new ButtonZonesScreen(this.mainWindow, this.sensorChooser);
            // Clear the binding
            BindingOperations.ClearBinding(this.kRegion, KinectRegion.KinectSensorProperty);
            // Show the desired screen
            bzs.Show();
        }

        void StateButtonClick(object sender, RoutedEventArgs e)
        {
            Utilities.State s = (Utilities.State)((KinectTileButton)sender).Content;
#if (DEBUG)
            Console.WriteLine(s.Name + " selected!");
#endif
            VehicleScreen vs = new VehicleScreen(this.mainWindow, this.sensorChooser, s);
            // Clear the binding
            BindingOperations.ClearBinding(this.kRegion, KinectRegion.KinectSensorProperty);
            // Show the desired screen
            vs.Show();
        }
    }
}
