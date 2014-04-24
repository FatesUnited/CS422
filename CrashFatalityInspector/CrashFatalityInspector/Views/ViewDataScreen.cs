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
    class ViewDataScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private Dictionary<string, int> data;
        //
        private KinectRegion kRegion;
        private Grid buttonGrid;
        //
        private double maxButtonWidth;
        private double maxButtonHeight;
        private double maxButtonDiameter;
        //
        private int largestDataValue;
        private double multiplier;
        //
        List<DataButtonBubble> dataBubbles;

        public ViewDataScreen(Window MainWindow, KinectSensorChooser SensorChooser, Utilities.State State, string VehicleType)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Get the desired data
            this.data = Utilities.Data.SelectDataByStateAndVehicleType(State.Name, VehicleType);
#if (DEBUG)
            Utilities.Data.PrintDataByStateAndVehicleTypeToConsole(this.data, State.Name, VehicleType);
#endif
            // Initialize display containers
            this.kRegion = CreateKinectRegion();
            //
            this.maxButtonWidth = this.mainWindow.Width / 7;
            this.maxButtonHeight = this.mainWindow.Height / 2; // Also row separation point in Y
            this.maxButtonDiameter = this.maxButtonHeight <= this.maxButtonWidth ? this.maxButtonHeight : this.maxButtonWidth;
            //
            this.largestDataValue = this.data.Max(value => value.Value);
            this.multiplier = this.largestDataValue / this.maxButtonDiameter;
#if (DEBUG)
            Console.WriteLine("KRegion size: " + this.kRegion.Width + ", " + this.kRegion.Height);
            Console.WriteLine("Max Button Width: " + this.maxButtonWidth.ToString());
            Console.WriteLine("Max Button Height: " + this.maxButtonHeight.ToString());
            Console.WriteLine("Max Button Diameter: " + this.maxButtonDiameter.ToString());
            Console.WriteLine("Largest Data Value: " + this.largestDataValue.ToString());
            Console.WriteLine("Data Multiplier: " + this.multiplier.ToString());
#endif
            //
            this.dataBubbles = new List<DataButtonBubble>();
            Random rand = new Random();
            foreach (KeyValuePair<string, int> kvp in this.data)
            {
                KinectCircleButton kcb = new KinectCircleButton();
                kcb.Width = kvp.Value / this.multiplier;
                kcb.Height = kvp.Value / this.multiplier;
                byte[] buffer = new byte[3];
                rand.NextBytes(buffer);
                SolidColorBrush scb = new SolidColorBrush();
                scb.Color = Color.FromRgb(buffer[0], buffer[1], buffer[2]);
                kcb.BorderBrush = scb;
                kcb.Foreground = Brushes.White;
                kcb.Content = kvp.Value.ToString();
                kcb.Click += kcb_Click;
                this.dataBubbles.Add(new DataButtonBubble(kcb, kvp.Value));
            }
            //
            this.buttonGrid = new Grid();
            this.buttonGrid.Width = this.kRegion.Width;
            this.buttonGrid.Height = this.kRegion.Height;
            this.buttonGrid.HorizontalAlignment = HorizontalAlignment.Center;
            this.buttonGrid.VerticalAlignment = VerticalAlignment.Center;
            this.buttonGrid.ShowGridLines = true;
            for (int i = 0; i < 7; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                this.buttonGrid.ColumnDefinitions.Add(cd);
            }
            for (int i = 0; i < 2; i++)
            {
                RowDefinition rd = new RowDefinition();
                this.buttonGrid.RowDefinitions.Add(rd);
            }
            int bubblesPlaced = 0;
            int row = 0;
            foreach (DataButtonBubble db in this.dataBubbles)
            {
                Grid.SetColumn(db.Bubble, bubblesPlaced);
                Grid.SetRow(db.Bubble, row);
                bubblesPlaced++;
                if (bubblesPlaced == 7)
                {
                    bubblesPlaced = 0;
                    row = 1;
                }
                this.buttonGrid.Children.Add(db.Bubble);
            }
            this.kRegion.Content = this.buttonGrid;
        }

        void kcb_Click(object sender, RoutedEventArgs e)
        {
            TitleScreen ts = new TitleScreen(this.mainWindow, this.sensorChooser);
            ts.Show();
        }

        public void Show()
        {
            // Display this screen
            this.mainWindow.Content = this.kRegion;
        }

        private KinectRegion CreateKinectRegion()
        {
            KinectRegion kr = new KinectRegion();
            kr.Width = this.mainWindow.ActualWidth - 50;
            kr.Height = this.mainWindow.ActualHeight - 50;
            kr.HorizontalAlignment = HorizontalAlignment.Center;
            kr.VerticalAlignment = VerticalAlignment.Center;
            return kr;
        }
    }

    class DataButtonBubble
    {
        public KinectCircleButton Bubble { get; set; }
        public int Data { get; set; }

        public DataButtonBubble(KinectCircleButton Bubble, int Data)
        {
            this.Bubble = Bubble;
            this.Data = Data;
        }
    }
}
