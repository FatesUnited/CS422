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
    class TimeZoneScreen
    {
        private Window mainWindow;
        private KinectSensorChooser sensorChooser;
        //
        private Grid grid;
        private Viewbox viewBox;
        private Image image;
        private DrawingGroup drawingGroup;
        private DrawingImage imageSource;
        //
        private static Utilities.WaveGesture gesture = new Utilities.WaveGesture();
        //
        private double jointThickness = Constants.DEFAULT_JOINT_THICKNESS;
        private double bodyCenterThickness = Constants.DEFAULT_BODY_CENTER_THICKNESS;
        private readonly Brush centerPointBrush = Brushes.Blue;
        private readonly Brush trackedJointBrush = Brushes.Red;
        private readonly Brush inferredJointBrush = Brushes.Yellow;
        private readonly Pen trackedBonePen = new Pen(Brushes.Green, 6);
        private readonly Pen inferredBonePen = new Pen(Brushes.Gray, 1);
        // These values will adjust based on window size
        // The initial values are based upon the initial 525 x 350 window size
        private float renderWidth = 600.0F;
        private float renderHeight = 375.0F;
        private int pacificRight = 90;
        private int mountainRight = 241;
        private int centralRight = 433;
        //

        public TimeZoneScreen(Window MainWindow, KinectSensorChooser SensorChooser)
        {
            // Get the main window and sensor objects
            this.mainWindow = MainWindow;
            this.sensorChooser = SensorChooser;
            // Reset all window-size specific variables
            this.renderWidth = (float)(this.mainWindow.Width * 1.1428571428571428571428571428571);
            this.renderHeight = (float)(this.mainWindow.Height * 1.0714285714285714285714285714286);
            this.pacificRight = (int)(this.mainWindow.Width * 0.17142857142857142857142857142857);
            this.mountainRight = (int)(this.mainWindow.Width * 0.45904761904761904761904761904762);
            this.centralRight = (int)(this.mainWindow.Width * 0.8247619047619047619047619047619);
            // Create the display objects
            this.grid = CreateGrid();
            this.viewBox = CreateViewbox();
            this.image = CreateImage();
            this.drawingGroup = CreateDrawingGroup();
            this.imageSource = CreateDrawingImage();
            this.image.Source = this.imageSource;
            // Set up the display objects
            this.viewBox.Child = this.image;
            this.grid.Children.Add(this.viewBox);
            // Set up the kinect to track the skeleton
            this.sensorChooser.Kinect.SkeletonFrameReady += SkeletonFrameReady;
            gesture.GestureRecognized += GestureRecognized;
        }

        public void Show()
        {
            // Display this screen
            this.mainWindow.Content = this.grid;
        }

        private Grid CreateGrid()
        {
            Grid grid = new Grid();
            grid.Name = Constants.ViewNames.TimeZoneScreen.ToString();
            ImageBrush iBrush = new ImageBrush();
            iBrush.ImageSource = new BitmapImage(new Uri(Constants.CFI_TIME_ZONE_IMAGE, UriKind.Relative));
            grid.Background = iBrush;
            return grid;
        }

        private Viewbox CreateViewbox()
        {
            return new Viewbox();
        }

        private Image CreateImage()
        {
            return new Image();
        }

        private DrawingGroup CreateDrawingGroup()
        {
            return new DrawingGroup();
        }

        private DrawingImage CreateDrawingImage()
        {
            return new DrawingImage(this.drawingGroup);
        }

        void SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            Skeleton[] skeletons = new Skeleton[0];
            using (SkeletonFrame skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);
                }
            }
            using (DrawingContext dc = this.drawingGroup.Open())
            {
                dc.DrawRectangle(Brushes.Transparent, null, new Rect(0.0, 0.0, renderWidth, renderHeight));
                if (skeletons.Length != 0)
                {
                    var user = skeletons.Where(u => u.TrackingState == SkeletonTrackingState.Tracked).FirstOrDefault();
                    if (user != null)
                    {
                        gesture.Update(user);
                    }
                    foreach (Skeleton skel in skeletons)
                    {
                        if (skel.TrackingState == SkeletonTrackingState.Tracked)
                        {
                            this.DrawBonesAndJoints(skel, dc);
                        }
                        else if (skel.TrackingState == SkeletonTrackingState.PositionOnly)
                        {
                            dc.DrawEllipse(
                            this.centerPointBrush,
                            null,
                            this.SkeletonPointToScreen(skel.Position),
                            bodyCenterThickness,
                            bodyCenterThickness);
                        }
                    }
                }
                this.drawingGroup.ClipGeometry = new RectangleGeometry(new Rect(0.0, 0.0, renderWidth, renderHeight));
            }
        }

        private void DrawBonesAndJoints(Skeleton skeleton, DrawingContext drawingContext)
        {
            Brush drawBrush = this.trackedJointBrush;
            drawingContext.DrawEllipse(drawBrush, null, this.SkeletonPointToScreen(skeleton.Joints[JointType.Spine].Position), jointThickness * 5, jointThickness * 5);
        }

        private Point SkeletonPointToScreen(SkeletonPoint skelpoint)
        {
            try
            {
                DepthImagePoint depthPoint = this.sensorChooser.Kinect.CoordinateMapper.MapSkeletonPointToDepthPoint(skelpoint, DepthImageFormat.Resolution640x480Fps30);
                //Console.WriteLine("Spine Coordinate: (" + depthPoint.X + "," + depthPoint.Y, ")");
                return new Point(depthPoint.X, depthPoint.Y);
            }
            catch (NullReferenceException e)
            {
#if (DEBUG)
                Console.WriteLine(e.Message);
#endif
                return new Point(0, 0);
            }
        }

        void GestureRecognized(object sender, EventArgs e)
        {
            Utilities.WaveGesture wg = (Utilities.WaveGesture)sender;
            Skeleton skel = wg.skeleton;
            Point coordinates = this.SkeletonPointToScreen(skel.Joints[JointType.Spine].Position);
            if (coordinates.X > pacificRight)
            {
                if (coordinates.X > mountainRight)
                {
                    if (coordinates.X > centralRight)
                    {
#if (DEBUG)
                        Console.WriteLine("Easetern Time Zone Selected!");
#endif
                        StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser,
                            Constants.TimeZones.Eastern);
                        ss.Show();
                    }
                    else
                    {
#if (DEBUG)
                        Console.WriteLine("Central Time Zone Selected!");
#endif
                        StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser,
                            Constants.TimeZones.Central);
                        ss.Show();
                    }
                }
                else
                {
#if (DEBUG)
                    Console.WriteLine("Mountain Time Zone Selected!");
#endif
                    StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser,
                            Constants.TimeZones.Mountain);
                    ss.Show();
                }
            }
            else
            {
#if (DEBUG)
                Console.WriteLine("Pacific Time Zone Selected!");
#endif
                StatesScreen ss = new StatesScreen(this.mainWindow, this.sensorChooser,
                            Constants.TimeZones.Pacific);
                ss.Show();
            }
        }
    }
}
