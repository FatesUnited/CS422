using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using System.Data;
using System.IO;

namespace CrashFatalityInspector.Utilities
{
    static class Data
    {
        //
        public static DataSet FatalityData;
        public static DataTable LargeTrucks;
        public static DataTable MotorCoaches;
        public static DataTable Buses;
        public static DataTable AllFatalities;
        //
        public static List<State> States;
        public static List<string> Vehicles;
        public static List<string> Years;
        //
        public static void LoadData(string ApplicationPath)
        {
#if (DEBUG)
            Console.WriteLine("Attemping to load data...");
#endif
            States = ReadStateDataFile(ApplicationPath, Constants.STATES_DATA_FILE_NAME);
            //
            FatalityData = new DataSet(Constants.DATA_SET_NAME);
            //
            LargeTrucks = ReadDataFile(ApplicationPath, Constants.LT_DATA_FILE, Constants.LT_DATA_NAME);
            MotorCoaches = ReadDataFile(ApplicationPath, Constants.MC_DATA_FILE, Constants.MC_DATA_NAME);
            Buses = ReadDataFile(ApplicationPath, Constants.BS_DATA_FILE, Constants.BS_DATA_NAME);
            AllFatalities = ReadDataFile(ApplicationPath, Constants.AV_DATA_FILE, Constants.AV_DATA_NAME);
            //
            Vehicles = PopulateVehicles(FatalityData);
            Years = PopulateYears(LargeTrucks);
#if (DEBUG)
            Console.WriteLine("...All data loaded!");
#endif
        }

        #region Load Data Methods...
        private static List<State> ReadStateDataFile(string Path, string Filename)
        {
            List<State> states = new List<State>();
            try
            {
                string filePath = Path + Filename;
                using (StreamReader sr = new StreamReader(File.Open(filePath, FileMode.Open)))
                {
                    string line = string.Empty;
                    string[] stateData = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        stateData = line.Split(new char[] { ',' });
                        try
                        {
                            states.Add(new State(stateData[0], stateData[1]));
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
#if (DEBUG)
            Console.WriteLine("States data loaded!");
#endif
            return states;
        }

        private static DataTable ReadDataFile(string Path, string Filename, string DataTableName)
        {
            DataTable table = new DataTable(DataTableName);
            try
            {
                string filePath = Path + Filename;
                using (StreamReader sr = new StreamReader(File.Open(filePath, FileMode.Open)))
                {
                    string line = string.Empty;
                    string[] columnHeaders = null;
                    string[] rowData = null;
                    DataColumn column;
                    DataRow row;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.StartsWith("State"))
                        {
                            columnHeaders = line.Split(new char[] { ',' });
                            column = CreateDataColumn(columnHeaders[0].Trim(), typeof(String));
                            table.Columns.Add(column);
                            for (int i = 1; i < columnHeaders.Length; i++)
                            {
                                column = CreateDataColumn(columnHeaders[i].Trim(), typeof(Int32));
                                table.Columns.Add(column);
                            }
                            continue;
                        }
                        if (!line.StartsWith("Table") &&
                            !line.StartsWith("US") &&
                            !line.StartsWith(",") &&
                            !line.StartsWith("\"") &&
                            !line.StartsWith("Notes") &&
                            !line.StartsWith("Source"))
                        {
                            rowData = line.Split(new char[] { ',' });
                            row = table.NewRow();
                            row[columnHeaders[0]] = rowData[0].Trim();
                            for (int i = 1; i < columnHeaders.Length; i++)
                            {
                                row[columnHeaders[i]] = Convert.ToInt32(rowData[i]);
                            }
                            table.Rows.Add(row);
                        }
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine(e.Message);
            }
#if (DEBUG)
            Console.WriteLine(DataTableName + " data loaded!");
#endif
            return table;
        }

        private static DataColumn CreateDataColumn(string ColumnName, Type DataType)
        {
            DataColumn column = new DataColumn();
            column.DataType = DataType;
            column.ColumnName = ColumnName;
            column.ReadOnly = true;
            return column;
        }

        private static List<string> PopulateVehicles(DataSet FatalityDataSet)
        {
            List<string> vehicles = new List<string>();
            DataTableCollection dtc = FatalityDataSet.Tables;
            foreach (DataTable dt in dtc)
            {
                vehicles.Add(dt.TableName);
            }
#if (DEBUG)
            Console.WriteLine("Vehicle names loaded!");
#endif
            return vehicles;
        }

        private static List<string> PopulateYears(DataTable Table)
        {
            List<string> years = new List<string>();
            if (Table.Columns.Count != 0)
            {
                foreach (DataColumn col in Table.Columns)
                {
                    if (!col.ColumnName.Equals("State"))
                    {
                        years.Add(col.ColumnName);
                    }
                }
            }
#if (DEBUG)
            Console.WriteLine("Years loaded!");
#endif
            return years;
        }
        #endregion

        #region Data Debug Methods...
        public static void PrintDataTableToConsole(DataTable Table)
        {
            Console.WriteLine("Viewing Table: " + Table.TableName);
            foreach (DataColumn col in Table.Columns)
            {
                if (col.ColumnName.Equals("State"))
                {
                    Console.Write("{0,-18}", col.ColumnName);
                }
                else
                {
                    Console.Write("{0,-6}", col.ColumnName);
                }
            }
            Console.WriteLine();

            foreach (DataRow row in Table.Rows)
            {
                foreach (DataColumn col in Table.Columns)
                {
                    if (col.DataType.Equals(typeof(String)))
                        Console.Write("{0,-18}", row[col]);
                    else if (col.DataType.Equals(typeof(Int32)))
                        Console.Write("{0,-6}", row[col]);
                    else
                        Console.Write("{0,-6}", row[col]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        public static void PrintDataByStateAndVehicleTypeToConsole(Dictionary<string, int> Data, string State, string VType)
        {
            Console.WriteLine();
            Console.WriteLine("Fatalities in " + State + " in the vehicle type: " + VType);
            PrintTheData(Data, 6);
        }

        public static void PrintDataByStateAndYearToConsole(Dictionary<string, int> Data, string State, string Year)
        {
            Console.WriteLine();
            Console.WriteLine("Fatalities in " + State + " in the year " + Year);
            PrintTheData(Data, 20);
        }

        private static void PrintTheData(Dictionary<string, int> Data, int offset)
        {
            string format = "{0,-" + offset + "}";
            foreach (KeyValuePair<string, int> kvp in Data)
            {
                Console.Write(format, kvp.Key);
            }
            Console.WriteLine();
            foreach (KeyValuePair<string, int> kvp in Data)
            {
                Console.Write(format, kvp.Value.ToString());
            }
            Console.WriteLine();
        }
        #endregion
    }

    class State
    {
        public string Name { get; set; }
        public Constants.TimeZones TimeZone { get; set; }

        public State(string Name, string TimeZone)
        {
            this.Name = Name;
            if (TimeZone.Equals(Constants.TimeZones.Pacific.ToString()))
            {
                this.TimeZone = Constants.TimeZones.Pacific;
            }
            if (TimeZone.Equals(Constants.TimeZones.Mountain.ToString()))
            {
                this.TimeZone = Constants.TimeZones.Mountain;
            }
            if (TimeZone.Equals(Constants.TimeZones.Central.ToString()))
            {
                this.TimeZone = Constants.TimeZones.Central;
            }
            if (TimeZone.Equals(Constants.TimeZones.Eastern.ToString()))
            {
                this.TimeZone = Constants.TimeZones.Eastern;
            }
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
