using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CrashFatalityInspector.Utilities
{
    static class Plot
    {
        /*
        private Ellipse DrawDataBubble(int Diameter, Brush Color)
        {
            Ellipse dataBubble = new Ellipse();
            dataBubble.Width = Diameter;
            dataBubble.Height = Diameter;
            dataBubble.Fill = Color;
            return dataBubble;
        }
        this.CanvasParent.Children.Clear();
        Ellipse dataBubble = DrawDataBubble(100, Brushes.Red);
        this.CanvasParent.Children.Add(dataBubble);
        Canvas.SetLeft(dataBubble, 100);
        Canvas.SetTop(dataBubble, 200);
        */

        public static void PlotData(Dictionary<string, int> Data, Canvas ParentCanvas)
        {
            ParentCanvas.Children.Clear();
            List<DataBubble> DataBubbles = new List<DataBubble>();
            double canvasWidth = ParentCanvas.ActualWidth;
            double canvasHeight = ParentCanvas.ActualHeight;
            double maxWHVal = canvasHeight <= canvasWidth ? canvasHeight : canvasWidth;
            double maxDiameter = 0;
#if (DEBUG)
            Console.WriteLine("Canvas Width: " + canvasWidth.ToString());
            Console.WriteLine("Canvas Height: " + canvasHeight.ToString());
#endif
            Random rand = new Random();
            foreach (KeyValuePair<string, int> kvp in Data)
            {
                maxDiameter = maxWHVal * (kvp.Value / maxWHVal);// +kvp.Value;
                // Create the data bubble ellipse object
                Ellipse el = new Ellipse();
                el.Width = maxDiameter;
                el.Height = maxDiameter;
                byte[] buffer = new byte[4];
                rand.NextBytes(buffer);
                SolidColorBrush scb = new SolidColorBrush();
                scb.Color = Color.FromArgb(buffer[0], buffer[1], buffer[2], buffer[3]);
                el.Fill = scb;
                // Create the data bubble text object
                Label l = new Label();
                l.Content = kvp.Value.ToString();
                l.Width = maxDiameter;
                l.Height = maxDiameter;
                l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                l.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                l.FontSize = 30;
                l.FontWeight = System.Windows.FontWeights.ExtraBold;
                DataBubbles.Add(new DataBubble(el, l, kvp.Value));
            }

            // Greatest death-rate will appear on top after this...
            IEnumerable<DataBubble> orderedSet = DataBubbles.OrderBy(db => db.Data);
            foreach (DataBubble db in orderedSet)
            {
                double left = rand.NextDouble() * (ParentCanvas.ActualWidth - maxDiameter);
                double top = rand.NextDouble() * (ParentCanvas.ActualHeight - maxDiameter);
                Canvas.SetLeft(db.Bubble, left);
                Canvas.SetTop(db.Bubble, top);
                Canvas.SetLeft(db.DataLabel, left);
                Canvas.SetTop(db.DataLabel, top);
                ParentCanvas.Children.Add(db.Bubble);
                ParentCanvas.Children.Add(db.DataLabel);
            }
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
