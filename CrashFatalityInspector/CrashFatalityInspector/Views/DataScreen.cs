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
using System.Windows.Shapes;
//
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;

namespace CrashFatalityInspector.Views
{
    class DataScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private Canvas canvas;
        //
        private Dictionary<string, int> data;
        //
        private double maxEllipseWidth;
        private double maxEllipseHeight;
        private double maxEllipseDiameter;
        //
        private int largestDataValue;
        private double multiplier;
        //
        List<DataBubble> dataBubbles;

        public DataScreen(Window MainWindow, KinectSensorChooser SensorChooser, Utilities.State State, string VehicleType)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            //
            this.data = Utilities.Data.SelectDataByStateAndVehicleType(State.Name, VehicleType);
#if (DEBUG)
            Utilities.Data.PrintDataByStateAndVehicleTypeToConsole(this.data, State.Name, VehicleType);
#endif
            //
            this.canvas = CreateCanvas();
            //
            this.maxEllipseWidth = canvas.Width / 7;
            this.maxEllipseHeight = canvas.Height / 2; // Also row separation point in Y
            this.maxEllipseDiameter = this.maxEllipseHeight <= this.maxEllipseWidth ? this.maxEllipseHeight : this.maxEllipseWidth;
            //
            this.largestDataValue = this.data.Max(value => value.Value);
            this.multiplier = this.largestDataValue / this.maxEllipseDiameter;
            
#if (DEBUG)
            Console.WriteLine("Canvas size: " + canvas.Width + ", " + canvas.Height);
            Console.WriteLine("Max Ellipse Width: " + this.maxEllipseWidth.ToString());
            Console.WriteLine("Max Ellipse Height: " + this.maxEllipseHeight.ToString());
            Console.WriteLine("Max Ellipse Diameter: " + this.maxEllipseDiameter.ToString());
            Console.WriteLine("Largest Data Value: " + this.largestDataValue.ToString());
            Console.WriteLine("Data Multiplier: " + this.multiplier.ToString());
#endif
            //
            this.dataBubbles = new List<DataBubble>();
            Random rand = new Random();
            foreach (KeyValuePair<string, int> kvp in this.data)
            {
                Ellipse el = new Ellipse();
                el.Width = kvp.Value / this.multiplier;
                el.Height = kvp.Value / this.multiplier;
                byte[] buffer = new byte[3];
                rand.NextBytes(buffer);
                SolidColorBrush scb = new SolidColorBrush();
                scb.Color = Color.FromRgb(buffer[0], buffer[1], buffer[2]);
                el.Fill = scb;
                Label l = new Label();
                l.Content = kvp.Value.ToString();
                l.Width = kvp.Value / this.multiplier;
                l.Height = kvp.Value / this.multiplier;
                l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                l.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                l.FontSize = 12;
                l.FontWeight = System.Windows.FontWeights.ExtraBold;
                l.MouseDoubleClick += l_MouseDoubleClick;
                this.dataBubbles.Add(new DataBubble(el, l, kvp.Value));
            }
            double xPos = 0, yPos = 0;
            int bubblesPlaced = 0;
            foreach (DataBubble db in this.dataBubbles)
            {
                // Random bubble placement in the canvas grid...
                int randX = rand.Next((int)(xPos), (int)(xPos + maxEllipseWidth - db.Bubble.Width));
                int randY = rand.Next((int)(yPos), (int)(yPos + maxEllipseHeight - db.Bubble.Height));
                Console.WriteLine("Bubble placed at " + randX + ", " + randY);
                Canvas.SetLeft(db.Bubble, randX);
                Canvas.SetTop(db.Bubble, randY);
                Canvas.SetLeft(db.DataLabel, randX);
                Canvas.SetTop(db.DataLabel, randY);
                //
                this.canvas.Children.Add(db.Bubble);
                this.canvas.Children.Add(db.DataLabel);
                bubblesPlaced++;
                xPos += this.maxEllipseWidth;
                if (bubblesPlaced == 7)
                {
                    xPos = 0;
                    yPos += this.maxEllipseHeight;
                }
            }
            this.mainWindow.Content = this.canvas;
        }

        void l_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            TitleScreen ts = new TitleScreen(this.mainWindow, this.sensorChooser);
            ts.Show();
        }

        private Canvas CreateCanvas()
        {
            Canvas canvas = new Canvas();
            canvas.Width = this.mainWindow.ActualWidth - 50;
            canvas.Height = this.mainWindow.ActualHeight - 50;
            canvas.HorizontalAlignment = HorizontalAlignment.Center;
            canvas.VerticalAlignment = VerticalAlignment.Center;
            return canvas;
        }
    }

    class DataBubble
    {
        public Ellipse Bubble { get; set; }
        public Label DataLabel { get; set; }
        public int Data { get; set; }

        public DataBubble(Ellipse Bubble, Label DataLabel, int Data)
        {
            this.Bubble = Bubble;
            this.DataLabel = DataLabel;
            this.Data = Data;
        }
    }
}
