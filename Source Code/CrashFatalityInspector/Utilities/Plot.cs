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
            //New list to store coordinates to prevent overlapping
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
                Ellipse el = new Ellipse();
                el.Width = maxDiameter;
                el.Height = maxDiameter;
                byte[] buffer = new byte[4];
                rand.NextBytes(buffer);
                SolidColorBrush scb = new SolidColorBrush();
                scb.Color = Color.FromRgb(buffer[0], buffer[1], buffer[2]);
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
            List<DataCoordinates> coordList = new List<DataCoordinates>();
            foreach (DataBubble db in orderedSet)
            {
                double left = 0;
                double top = 0;
                double right = 0;
                double bottom = 0;
                //Check if list is empty
                bool listEmpty = !coordList.Any();
                if (listEmpty == true)
                {
                    left = rand.NextDouble() * (ParentCanvas.ActualWidth - maxDiameter);
                    right = left + db.Bubble.Width;
                    top = rand.NextDouble() * (ParentCanvas.ActualHeight - maxDiameter);
                    bottom = top + db.Bubble.Height;
                    coordList.Add(new DataCoordinates(left, top, left+db.Bubble.Width, top+db.Bubble.Height));
                }
                else
                {
                    bool goodCoords = false;
                    while (goodCoords == false)
                    {
                        left = rand.NextDouble() * (ParentCanvas.ActualWidth - maxDiameter);
                        top = rand.NextDouble() * (ParentCanvas.ActualHeight - maxDiameter);
                        right = left + db.Bubble.Width;
                        bottom = top + db.Bubble.Height;

                        for (int i = 0; i < coordList.Count(); i++)
                        {
                            DataCoordinates coords = coordList.ElementAt(i);
                            double height = coords.topCoord - coords.bottomCoord;
                            double width = coords.rightCoord - coords.leftCoord;
                            if ((left >= coords.leftCoord && left <= coords.rightCoord) || (top >= coords.topCoord && top <= coords.bottomCoord))
                            {
                                break;
                            }
                            else if ((right >= coords.leftCoord && right <= coords.rightCoord) || (bottom >= coords.topCoord && bottom <= coords.bottomCoord))
                            {
                                break;
                            }
                            else if ((right >= canvasWidth) || (bottom >= canvasHeight))
                            {
                                break;
                            }
                            else
                            {
                                if (i == coordList.Count() - 1)
                                {
                                    goodCoords = true;
                                }
                            }
                        }
                    }
                    coordList.Add(new DataCoordinates(left, top, left+db.Bubble.Width, top+db.Bubble.Height));
                }
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

    class DataCoordinates
    {
        public double leftCoord { get; set; }
        public double topCoord { get; set; }
        public double rightCoord { get; set; }
        public double bottomCoord { get; set; }

        public DataCoordinates(double leftCoord, double topCoord, double rightCoord, double bottomCoord)
        {
            this.leftCoord = leftCoord;
            this.topCoord = topCoord;
            this.rightCoord = rightCoord;
            this.bottomCoord = bottomCoord;
        }
    }
}
