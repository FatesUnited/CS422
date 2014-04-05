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

        public const string STATES_DATA_FILE_NAME = "Data/States.csv";

        public const string DATA_SET_NAME = "Fatality Data";

        public const string LT_DATA_FILE = "Data/LTFatalities.csv";
        public const string MC_DATA_FILE = "Data/MCBusFatalities.csv";
        public const string BS_DATA_FILE = "Data/BusFatalities.csv";
        public const string AV_DATA_FILE = "Data/AllFatalities.csv";

        public const string LT_DATA_NAME = "Large Trucks";
        public const string MC_DATA_NAME = "MotorCoaches";
        public const string BS_DATA_NAME = "Buses";
        public const string AV_DATA_NAME = "All Vehicles";
    }
}
