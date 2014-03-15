using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Kinect.Toolkit.Controls;



namespace WizardOfOz
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Window cWindow;
        //
        private const int imageCount = 4;
        private Image iViewer;
        private List<BitmapImage> images;
        //
        private DockPanel dockPanelStates;
        private KinectRegion kRegion;
        private List<string> states;
        private Label stateInfoLabel;
        private ScrollViewer statesSV;
        private KinectScrollViewer statesKSV;
        //
        private DockPanel dockPanelVehicles;
        private List<string> vehicles;
        private Label vehicleInfoLabel;
        private ScrollViewer vehiclesSV;

        private readonly KinectSensorChooser sensorChooser;

        public MainWindow()
        {
            this.InitializeComponent();
            // Initialize components


            // initialize the sensor chooser and UI
            this.sensorChooser = new KinectSensorChooser();
            this.sensorChooser.KinectChanged += SensorChooserOnKinectChanged;
            //Commented out Kinect UI Camera region
            //this.sensorChooserUi.KinectSensorChooser = this.sensorChooser;
            this.sensorChooser.Start();

            this.iViewer = new Image();
            this.dockPanelStates = new DockPanel();
            this.statesSV = new ScrollViewer();
            this.kRegion = new KinectRegion();
            this.statesKSV = new KinectScrollViewer();
            //
            this.dockPanelVehicles = new DockPanel();
            this.vehiclesSV = new ScrollViewer();

            // Bind the sensor chooser's current sensor to the KinectRegion
            var regionSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.kRegion, KinectRegion.KinectSensorProperty, regionSensorBinding);

            // Set up component event handlers
            this.SizeChanged += MainWindow_SizeChanged;
            this.Closing += MainWindow_Closing;

            // Get application path
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appPath = System.IO.Path.GetDirectoryName(exePath);

            // Fill data containers
            this.PopulateImages(appPath, imageCount);
            this.PopulateCentralStates();
            this.PopulateVehicles();

            // Set up the image viewer
            this.iViewer.Visibility = System.Windows.Visibility.Visible;
            
            // Set up the states dock panel
            this.SetupStatesDockPanel(this.states);

            // Set up the vehicles dock panel
            this.SetupVehiclesDockPanel(this.vehicles);
            
            // Display the control window
            cWindow = new WindowControl(this, this.images, this.iViewer, 
                this.dockPanelStates, 
                this.dockPanelVehicles);
            cWindow.Show();
        }

        private static void SensorChooserOnKinectChanged(object sender, KinectChangedEventArgs args)
        {
            if (args.OldSensor != null)
            {
                try
                {
                    args.OldSensor.DepthStream.Range = DepthRange.Default;
                    args.OldSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    args.OldSensor.DepthStream.Disable();
                    args.OldSensor.SkeletonStream.Disable();
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }

            if (args.NewSensor != null)
            {
                try
                {
                    args.NewSensor.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
                    args.NewSensor.SkeletonStream.Enable();

                    try
                    {
                        args.NewSensor.DepthStream.Range = DepthRange.Near;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = true;
                    }
                    catch (InvalidOperationException)
                    {
                        // Non Kinect for Windows devices do not support Near mode, so reset back to default mode.
                        args.NewSensor.DepthStream.Range = DepthRange.Default;
                        args.NewSensor.SkeletonStream.EnableTrackingInNearRange = false;
                    }
                }
                catch (InvalidOperationException)
                {
                    // KinectSensor might enter an invalid state while enabling/disabling streams or stream features.
                    // E.g.: sensor might be abruptly unplugged.
                }
            }
        }


        private void SetupStatesDockPanel(List<string> States)
        {
            // Set up states info label
            this.stateInfoLabel = new Label();
            this.stateInfoLabel.Content = "Swipe to pick a state...";
            this.stateInfoLabel.Foreground = Brushes.White;
            this.stateInfoLabel.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            this.stateInfoLabel.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            DockPanel.SetDock(this.stateInfoLabel, Dock.Top);
            // Set up scroll viewer container
            this.statesSV = new ScrollViewer();
            this.statesSV.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            this.statesSV.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            // Kinext scroll viewer
            this.statesKSV = new KinectScrollViewer();
            this.statesKSV.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            this.statesKSV.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            // Set up grid container
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
                //Label s = new Label();
                //s.Content = this.states[i];
                //s.FontWeight = FontWeights.Bold;
                //s.Foreground = Brushes.White;
                //s.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                //s.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                //Grid.SetRow(s, 0);
                //Grid.SetColumn(s, i);
                //g.Children.Add(s);
                Microsoft.Kinect.Toolkit.Controls.KinectTileButton ktb = new Microsoft.Kinect.Toolkit.Controls.KinectTileButton();
                ktb.Content = this.states[i];
                ktb.Click += ktb_Click;
                ktb.HorizontalLabelAlignment = System.Windows.HorizontalAlignment.Center;
                ktb.VerticalLabelAlignment = System.Windows.VerticalAlignment.Center;
                ktb.Foreground = Brushes.White;
                Grid.SetRow(ktb, 0);
                Grid.SetColumn(ktb, i);
                g.Children.Add(ktb);
            }
            this.statesSV.Content = g;
            this.statesKSV.Content = g;
            this.kRegion.Content = this.statesKSV;
            DockPanel.SetDock(this.kRegion, Dock.Bottom);
            // Add the child elements
            this.dockPanelStates.Children.Add(this.stateInfoLabel);
            this.dockPanelStates.Children.Add(this.kRegion);
        }

        void ktb_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("BIG DONG");
        }

        private void SetupVehiclesDockPanel(List<string> Vehicles)
        {
            // Set up states info label
            this.vehicleInfoLabel = new Label();
            this.vehicleInfoLabel.Content = "Select a vehicle type...";
            this.vehicleInfoLabel.Foreground = Brushes.White;
            this.vehicleInfoLabel.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            this.vehicleInfoLabel.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
            DockPanel.SetDock(this.vehicleInfoLabel, Dock.Top);
            // Set up scroll viewer container
            this.vehiclesSV = new ScrollViewer();
            this.vehiclesSV.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            this.vehiclesSV.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            // Set up grid container
            Grid g = new Grid();
            g.ShowGridLines = true;
            g.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            g.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            for (int i = 0; i < this.vehicles.Count; i++)
            {
                ColumnDefinition cd = new ColumnDefinition();
                g.ColumnDefinitions.Add(cd);
            }
            RowDefinition rd = new RowDefinition();
            g.RowDefinitions.Add(rd);
            for (int i = 0; i < this.vehicles.Count; i++)
            {
                Label s = new Label();
                s.Content = this.vehicles[i];
                s.FontWeight = FontWeights.Bold;
                s.Foreground = Brushes.White;
                s.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
                s.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;
                Grid.SetRow(s, 0);
                Grid.SetColumn(s, i);
                g.Children.Add(s);
            }
            this.vehiclesSV.Content = g;
            DockPanel.SetDock(this.vehiclesSV, Dock.Bottom);
            // Add the child elements
            this.dockPanelVehicles.Children.Add(this.vehicleInfoLabel);
            this.dockPanelVehicles.Children.Add(this.vehiclesSV);
        }

        private void PopulateImages(string ApplicationPath, int SlideCount)
        {
            this.images = new List<BitmapImage>();
            for (int i = 0; i < SlideCount; i++)
            {
                BitmapImage bImage = new BitmapImage();
                bImage.BeginInit();
                bImage.UriSource = new Uri(ApplicationPath + "/" + i.ToString() + ".jpg");
                bImage.EndInit();
                this.images.Add(bImage);
            }
        }

        private void PopulateCentralStates()
        {
            this.states = new List<string>();
            this.states.Add("Alabama");
            this.states.Add("Arkansas");
            this.states.Add("Illinois");
            this.states.Add("Iowa");
            this.states.Add("Kansas");
            this.states.Add("Kentucky");
            this.states.Add("Louisiana");
            this.states.Add("Minnesota");
            this.states.Add("Mississippi");
            this.states.Add("Missouri");
            this.states.Add("Nebraska");
            this.states.Add("North Dakota");
            this.states.Add("Oklahoma");
            this.states.Add("South Dakota");
            this.states.Add("Tennessee");
            this.states.Add("Texas");
            this.states.Add("Wisconsin");
        }

        private void PopulateVehicles()
        {
            this.vehicles = new List<string>();
            this.vehicles.Add("Large Truck");
            this.vehicles.Add("Motorcoach");
            this.vehicles.Add("Bus");
            this.vehicles.Add("All Vehicles");
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Adjust state dock panel elements
            this.stateInfoLabel.Height = Application.Current.MainWindow.ActualHeight / 3;
            this.stateInfoLabel.FontSize = Application.Current.MainWindow.ActualHeight / 15;
            Grid s = (Grid)this.statesSV.Content;
            s.Height = Application.Current.MainWindow.ActualHeight / 3;
            UIElementCollection sKtButtons = s.Children;
            foreach (KinectTileButton l in sKtButtons)
            {
                l.Width = Application.Current.MainWindow.ActualWidth / 3.75;
                l.Height = s.ActualHeight;
                l.FontSize = Application.Current.MainWindow.ActualWidth / 30;
            }
            //foreach (Label l in sLabels)
            //{
            //    l.Width = Application.Current.MainWindow.ActualWidth / 3.75;
            //    l.FontSize = Application.Current.MainWindow.ActualWidth / 30;
            //}
            // Adjust vehicle dock panel elements
            this.vehicleInfoLabel.Height = Application.Current.MainWindow.ActualHeight / 3;
            this.vehicleInfoLabel.FontSize = Application.Current.MainWindow.ActualHeight / 15;
            Grid v = (Grid)this.vehiclesSV.Content;
            v.Height = Application.Current.MainWindow.ActualHeight / 3;
            UIElementCollection vLabels = v.Children;
            foreach (Label l in vLabels)
            {
                l.Width = Application.Current.MainWindow.ActualWidth / 3.75;
                l.FontSize = Application.Current.MainWindow.ActualWidth / 30;
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            cWindow.Close();
        }
    }
}