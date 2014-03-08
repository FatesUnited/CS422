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
            Utilities.Data.LoadData();
            //
            this.ComboBoxTimeZone.SelectionChanged += ComboBoxTimeZone_SelectionChanged;
            this.CheckBoxByYear.Checked += CheckBoxByYear_Checked;
            this.CheckBoxByYear.Unchecked += CheckBoxByYear_Unchecked;
            this.ButtonViewData.Click += ButtonViewData_Click;
            //
            this.CanvasParent.Background = Brushes.LightGray;
            //
            this.ComboBoxTimeZone.Items.Add(Constants.TimeZones.Pacific);
            this.ComboBoxTimeZone.Items.Add(Constants.TimeZones.Mountain);
            this.ComboBoxTimeZone.Items.Add(Constants.TimeZones.Central);
            this.ComboBoxTimeZone.Items.Add(Constants.TimeZones.Eastern);
            this.ComboBoxTimeZone.SelectedItem = Constants.TimeZones.Central;
            //
            foreach (string s in Utilities.Data.Vehicles)
            {
                this.ComboBoxVehicleTypes.Items.Add(s);
            }
            this.ComboBoxVehicleTypes.SelectedIndex = 0;
            //
            foreach (string s in Utilities.Data.Years)
            {
                this.ComboBoxYears.Items.Add(s);
            }
            this.ComboBoxYears.SelectedIndex = 0;
            this.ComboBoxYears.IsEnabled = false;
        }

        private void ButtonViewData_Click(object sender, RoutedEventArgs e)
        {
            if (this.CheckBoxByYear.IsChecked == false)
            {
#if (DEBUG)
                Console.WriteLine(">>>User selected to view data by state and vehicle type...");
                Console.WriteLine(">>>[" + this.ComboBoxStates.SelectedItem.ToString() + "|" + this.ComboBoxVehicleTypes.SelectedItem.ToString() + "]");
#endif
                Dictionary<string, int> data = Utilities.Data.SelectDataByStateAndVehicleType(this.ComboBoxStates.SelectedItem.ToString(), this.ComboBoxVehicleTypes.SelectedItem.ToString());
#if (DEBUG)
                Utilities.Data.PrintDataByStateAndVehicleTypeToConsole(data, this.ComboBoxStates.SelectedItem.ToString(), this.ComboBoxVehicleTypes.SelectedItem.ToString());
#endif
                Utilities.Plot.PlotData(data, this.CanvasParent);
            }
            else
            {
#if (DEBUG)
                Console.WriteLine(">>>User selected to view data by state and year...");
                Console.WriteLine(">>>[" + this.ComboBoxStates.SelectedItem.ToString() + "|" + this.ComboBoxYears.SelectedItem.ToString() + "]");
#endif
                Dictionary<string, int> data = Utilities.Data.SelectedDataByStateAndYear(this.ComboBoxStates.SelectedItem.ToString(), this.ComboBoxYears.SelectedItem.ToString());
#if (DEBUG)
                Utilities.Data.PrintDataByStateAndYearToConsole(data, this.ComboBoxStates.SelectedItem.ToString(), this.ComboBoxYears.SelectedItem.ToString());
#endif
            }
        }

        private void CheckBoxByYear_Unchecked(object sender, RoutedEventArgs e)
        {
            this.ComboBoxVehicleTypes.IsEnabled = true;
            this.ComboBoxYears.IsEnabled = false;
        }

        private void CheckBoxByYear_Checked(object sender, RoutedEventArgs e)
        {
            this.ComboBoxVehicleTypes.IsEnabled = false;
            this.ComboBoxYears.IsEnabled = true;
        }

        private void ComboBoxTimeZone_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
#if (DEBUG)
            Console.WriteLine(">>>Time zone " + this.ComboBoxTimeZone.SelectedItem.ToString() + " selected");
#endif
            LoadStatesByTimeZone((Constants.TimeZones)this.ComboBoxTimeZone.SelectedItem);
#if (DEBUG)
            Console.WriteLine(">>>" + this.ComboBoxTimeZone.SelectedItem.ToString() + " time zone states loaded");
#endif
        }

        private void LoadStatesByTimeZone(Constants.TimeZones TimeZone)
        {
            this.ComboBoxStates.Items.Clear();
            foreach (Utilities.State s in Utilities.Data.States)
            {
                if (s.TimeZone == TimeZone)
                {
                    this.ComboBoxStates.Items.Add(s);
                }
            }
            this.ComboBoxStates.SelectedIndex = 0;
        }
    }
}
