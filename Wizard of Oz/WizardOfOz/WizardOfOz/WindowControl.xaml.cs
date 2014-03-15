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
using System.Windows.Shapes;

namespace WizardOfOz
{
    /// <summary>
    /// Interaction logic for WindowControl.xaml
    /// </summary>
    public partial class WindowControl : Window
    {
        private Window mainWindow;
        private List<BitmapImage> images;
        private Image iViewer;
        private DockPanel dockPanelStates;
        private DockPanel dockPanelVehicles;
        private int slide;
        private int image;

        public WindowControl(Window MainWindow, 
            List<BitmapImage> Images, Image IViewer,    // Image viewer
            DockPanel DockPanelStates,                  // States Dock Panel
            DockPanel DockPanelVehicles)                // Vehicles Dock Panel
        {
            InitializeComponent();

            // Initialize
            this.mainWindow = MainWindow;
            this.images = Images;
            this.iViewer = IViewer;
            this.dockPanelStates = DockPanelStates;
            this.dockPanelVehicles = DockPanelVehicles;
            this.slide = 0;
            this.image = 0;

            // Set up the intial image viewer
            this.iViewer.Source = this.images[this.image];  // 0.jpg
            this.iViewer.Stretch = Stretch.Fill;

            // Configure the initial main window content
            this.mainWindow.Content = this.iViewer;

            // Update info
            this.LabelInfo.Content = "Viewing slide: " + this.slide.ToString();
            this.LabelNext.Content = "Next slide: " + (this.slide + 1).ToString();
        }

        private void ButtonNext_Click(object sender, RoutedEventArgs e)
        {
            switch (this.slide)
            {
                case 0:
                    this.slide++;   // Display image 1 (time zones)
                    this.image++;   // 1.jpg
                    this.iViewer.Source = this.images[this.image];
                    this.LabelInfo.Content = "Viewing slide: " + this.slide.ToString();
                    this.LabelNext.Content = "Next slide: " + (this.slide + 1).ToString();
                    this.mainWindow.Content = this.iViewer;
                    break;
                case 1:
                    this.slide++;   // Display slide 2 (times zones with dot)
                    this.image++;   // 2.jpg
                    this.iViewer.Source = this.images[this.image];
                    this.LabelInfo.Content = "Viewing slide: " + this.slide.ToString();
                    this.LabelNext.Content = "Next slide: " + (this.slide + 1).ToString();
                    this.mainWindow.Content = this.iViewer;
                    break;
                case 2:
                    this.slide++;   // Display slide 3 (states selector)
                    this.mainWindow.Content = this.dockPanelStates;
                    this.LabelInfo.Content = "Viewing slide: " + this.slide.ToString();
                    this.LabelNext.Content = "Next slide: " + (this.slide + 1).ToString();
                    break;
                case 3:
                    this.slide++;   // Display slide 4 (vehicle type selector)
                    this.mainWindow.Content = this.dockPanelVehicles;
                    this.LabelInfo.Content = "Viewing slide: " + this.slide.ToString();
                    this.LabelNext.Content = "Next slide: " + (this.slide + 1).ToString();
                    break;
                case 4:
                    this.slide++;   // Display slide 5 (data bubbles)
                    this.image++;   // 3.jpg
                    this.iViewer.Source = this.images[this.image];
                    this.LabelInfo.Content = "Viewing slide: " + this.slide.ToString();
                    this.LabelNext.Content = "Next slide: 0";
                    this.mainWindow.Content = this.iViewer;
                    break;
                default:
                    this.slide = 0; // Display slide 0
                    this.image = 0;
                    this.iViewer.Source = this.images[this.image];
                    this.LabelInfo.Content = "Viewing slide: " + this.slide.ToString();
                    this.LabelNext.Content = "Next slide: " + (this.slide + 1).ToString();
                    this.mainWindow.Content = this.iViewer;
                    break;
            }
        }
    }
}
