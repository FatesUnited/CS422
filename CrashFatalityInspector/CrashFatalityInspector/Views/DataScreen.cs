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
        private Utilities.State state;
        //
        private KinectRegion kRegion;
        //
        private StackPanel displayStackPanel;
        private Grid navButtonGrid;
        private KinectScrollViewer dataScrollViewer;
        private Canvas dataCanvas;
        //
        private Dictionary<string, int> data;
        private List<DataBubble> dataBubbles;
        private double maxEllipseDiameter;
        private double multiplier;
        private int largestDataValue;

        public DataScreen(Window MainWindow, KinectSensorChooser SensorChooser, Utilities.State State, string VehicleType)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            //
            this.state = State;
            this.data = Utilities.Data.SelectDataByStateAndVehicleType(State.Name, VehicleType);
            //
            CreateKinectRegion();
            // Bind the Kinect sensor
            var regionSensorBinding = new Binding("Kinect") { Source = SensorChooser };
            BindingOperations.SetBinding(this.kRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);
        }

        public void Show()
        {
            // Display this screen
            this.mainWindow.Content = this.kRegion;
        }

        private void CreateKinectRegion()
        {
            this.kRegion = new KinectRegion();
            //
            this.displayStackPanel = new StackPanel();
            ImageBrush iBrush = new ImageBrush();
            iBrush.ImageSource = new BitmapImage(new Uri(Constants.CFI_DATA_IMAGE, UriKind.Relative));
            this.displayStackPanel.Background = iBrush;
            //
            this.navButtonGrid = new Grid();
            for (int i = 0; i < 4; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                this.navButtonGrid.ColumnDefinitions.Add(cd);
            }
            RowDefinition rd = new RowDefinition();
            this.navButtonGrid.RowDefinitions.Add(rd);
            //
            KinectTileButton NavToTitleButton = CreateNavTileButton(Constants.ViewNames.TitleScreen, "home");
            Grid.SetColumn(NavToTitleButton, 0);
            Grid.SetRow(NavToTitleButton, 0);
            KinectTileButton NavToZoneButton = CreateNavTileButton(Constants.ViewNames.TimeZoneScreen, "time zone");
            Grid.SetColumn(NavToZoneButton, 1);
            Grid.SetRow(NavToZoneButton, 0);
            KinectTileButton NavToStatesButton = CreateNavTileButton(Constants.ViewNames.StatesScreen, "states");
            Grid.SetColumn(NavToStatesButton, 2);
            Grid.SetRow(NavToStatesButton, 0);
            KinectTileButton NavToVehiclesButton = CreateNavTileButton(Constants.ViewNames.VehicleScreen, "vehicles");
            Grid.SetColumn(NavToVehiclesButton, 3);
            Grid.SetRow(NavToVehiclesButton, 0);
            //
            this.navButtonGrid.Children.Add(NavToTitleButton);
            this.navButtonGrid.Children.Add(NavToZoneButton);
            this.navButtonGrid.Children.Add(NavToStatesButton);
            this.navButtonGrid.Children.Add(NavToVehiclesButton);
            //
            this.displayStackPanel.Children.Add(this.navButtonGrid);
            //
            this.dataScrollViewer = new KinectScrollViewer();
            this.dataScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            this.dataScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            //
            this.dataCanvas = new Canvas();
            double canvasHeight = this.mainWindow.ActualHeight / 3;
            this.dataCanvas.Height = this.mainWindow.ActualHeight / 3; ;
            double canvasWidth = canvasHeight * Utilities.Data.Years.Count;
            this.dataCanvas.Width = canvasWidth;
            //
            this.dataBubbles = new List<DataBubble>();
            this.maxEllipseDiameter = canvasHeight;
            this.largestDataValue = this.data.Max(value => value.Value);
            this.multiplier = this.largestDataValue / this.maxEllipseDiameter;
            foreach (KeyValuePair<string, int> kvp in this.data)
            {
                Ellipse el = new Ellipse();
                el.Width = kvp.Value / this.multiplier;
                el.Height = kvp.Value / this.multiplier;
                el.Fill = new LinearGradientBrush(Color.FromRgb(17, 17, 17), Color.FromRgb(200, 200, 200), 90);
                el.Opacity = 0.5;
                //
                Label l = new Label();
                // Content adjustments
                if (kvp.Value.ToString().Length <= 1)
                {
                    l.Content = "   " + kvp.Value.ToString() + "\n" + kvp.Key;
                }
                if (kvp.Value.ToString().Length == 2)
                {
                    l.Content = "  " + kvp.Value.ToString() + "\n" + kvp.Key;
                }
                if (kvp.Value.ToString().Length == 3)
                {
                    l.Content = " " + kvp.Value.ToString() + "\n" + kvp.Key;
                }
                if (kvp.Value.ToString().Length >= 4)
                {
                    l.Content = kvp.Value.ToString() + "\n" + kvp.Key;
                }
                l.Width = kvp.Value / this.multiplier;
                l.Height = kvp.Value / this.multiplier;
                l.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                l.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                l.Foreground = Brushes.White;
                l.FontSize = this.mainWindow.ActualHeight / 30;
                l.FontWeight = System.Windows.FontWeights.ExtraBold;
                //
                this.dataBubbles.Add(new DataBubble(el, l, kvp.Value, kvp.Key));
            }
            //
            double xPos = 0, xOffset = 0, yPos = 0, yOffset = 0;
            foreach (DataBubble db in this.dataBubbles)
            {
                if (db.Bubble.Width != this.maxEllipseDiameter)
                {
                    xOffset = ((this.maxEllipseDiameter - db.Bubble.Width) / 2);
                    Canvas.SetLeft(db.Bubble, xPos + xOffset);
                    Canvas.SetLeft(db.DataLabel, xPos + xOffset);
                }
                else
                {
                    Canvas.SetLeft(db.Bubble, xPos);
                    Canvas.SetLeft(db.DataLabel, xPos);
                }
                if (db.Bubble.Height != this.maxEllipseDiameter)
                {
                    yOffset = ((this.maxEllipseDiameter - db.Bubble.Height) / 2);
                    Canvas.SetTop(db.Bubble, yPos + yOffset);
                    Canvas.SetTop(db.DataLabel, yPos + yOffset);
                }
                else
                {
                    Canvas.SetTop(db.Bubble, yPos);
                    Canvas.SetTop(db.DataLabel, yPos);
                }
                this.dataCanvas.Children.Add(db.DataLabel);
                this.dataCanvas.Children.Add(db.Bubble);
                xPos += this.maxEllipseDiameter;
            }
            //
            this.dataScrollViewer.Content = this.dataCanvas;
            this.displayStackPanel.Children.Add(this.dataScrollViewer);
            //
            this.kRegion.Content = this.displayStackPanel;
        }

        private KinectTileButton CreateNavTileButton(Constants.ViewNames Name, string Content)
        {
            KinectTileButton ktb = new KinectTileButton();
            ktb.Name = Name.ToString();
            ktb.Content = Content;
            ktb.Background = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            ktb.MaxHeight = this.mainWindow.ActualHeight / 3.75;
            ktb.MaxWidth = this.mainWindow.ActualHeight / 3.75;
            ktb.FontSize = this.mainWindow.ActualHeight / 22;
            ktb.Click += NavButtonClick;
            ktb.HorizontalLabelAlignment = System.Windows.HorizontalAlignment.Center;
            ktb.VerticalLabelAlignment = System.Windows.VerticalAlignment.Center;
            ktb.Foreground = Brushes.White;
            return ktb;
        }

        void NavButtonClick(object sender, RoutedEventArgs e)
        {
            if (((KinectTileButton)sender).Name == Constants.ViewNames.TitleScreen.ToString())
            {
                TitleScreen ts = new TitleScreen(this.mainWindow, this.sensorChooser);
                ts.Show();
            }
            if (((KinectTileButton)sender).Name == Constants.ViewNames.TimeZoneScreen.ToString())
            {
                ButtonZonesScreen bzs = new ButtonZonesScreen(this.mainWindow, this.sensorChooser);
                bzs.Show();
            }
            if (((KinectTileButton)sender).Name == Constants.ViewNames.StatesScreen.ToString())
            {
                StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser, this.state.TimeZone);
                ss.Show();
            }
            if (((KinectTileButton)sender).Name == Constants.ViewNames.VehicleScreen.ToString())
            {
                VehicleScreen vs = new VehicleScreen(this.mainWindow, this.sensorChooser, this.state);
                vs.Show();
            }
        }
    }

    class DataBubble
    {
        public Ellipse Bubble { get; set; }
        public Label DataLabel { get; set; }
        public int Data { get; set; }
        public string Year { get; set; }

        public DataBubble(Ellipse Bubble, Label DataLabel, int Data, string Year)
        {
            this.Bubble = Bubble;
            this.DataLabel = DataLabel;
            this.Data = Data;
            this.Year = Year;
        }
    }
}
