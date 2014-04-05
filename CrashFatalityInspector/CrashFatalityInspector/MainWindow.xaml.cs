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
//
using Microsoft.Kinect;

namespace CrashFatalityInspector
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //
            string exePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string appPath = System.IO.Path.GetDirectoryName(exePath);
#if (DEBUG)
            Console.WriteLine("Executable Path: " + exePath);
            Console.WriteLine("Application Path: " + appPath);
#endif
            //
            Utilities.Data.LoadData(appPath);
            //

        }
    }
}
