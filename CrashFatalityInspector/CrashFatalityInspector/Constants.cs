using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrashFatalityInspector
{
    static class Constants
    {
        [Flags]
        public enum TimeZones : int { Pacific, Mountain, Central, Eastern };
        //
        public const string STATES_DATA_FILE_NAME = "/Data/States.csv";
        //
        public const string DATA_SET_NAME = "Fatality Data";
        //
        public const string LT_DATA_FILE = "/Data/LTFatalities.csv";
        public const string MC_DATA_FILE = "/Data/MCBusFatalities.csv";
        public const string BS_DATA_FILE = "/Data/BusFatalities.csv";
        public const string AV_DATA_FILE = "/Data/AllFatalities.csv";
        //
        public const string LT_DATA_NAME = "Large Trucks";
        public const string MC_DATA_NAME = "Motor Coaches";
        public const string BS_DATA_NAME = "Buses";
        public const string AV_DATA_NAME = "All Vehicles";
        //
        [Flags]
        public enum ViewNames : int { TitleScreen, TimeZoneScreen, StatesScreen, VehicleScreen, DataScreen };
        //
        public const string CFI_TITLE_IMAGE = @"Images/TitleScreenBackground.png";
        public const string CFI_TITLE_BUTTON_LABEL = "start";
        //
        public const string CFI_BUTTON_ZONE_IMAGE = @"Images/TimeZoneBackground.png";
        //
        public const string CFI_STATES_IMAGE = @"Images/GenericBackground.png";
        public const string CFI_STATES_PACIFIC_IMAGE = @"Images/PacificTimeZone.png";
        public const string CFI_STATES_MOUNTAIN_IMAGE = @"Images/MountainTimeZone.png";
        public const string CFI_STATES_CENTRAL_IMAGE = @"Images/CentralTimeZone.png";
        public const string CFI_STATES_EASTERN_IMAGE = @"Images/EasternTimeZone.png";
        //
        public const string CFI_VEHICLES_IMAGE = @"Images/GenericBackground.png";
        public const string VEHICLE_TYPE_LARGE_TRUCKS = "Large Trucks";
        public const string VEHICLE_TYPE_MOTOR_COACHES = "Motor Coaches";
        public const string VEHICLE_TYPE_BUSES = "Buses";
        public const string VEHICLE_TYPE_ALL_VEHICLES = "All Vehicles";
        //
        public const string CFI_DATA_IMAGE = @"Images/GenericBackground.png";
    }
}
