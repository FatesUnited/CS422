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
        public static DataSet FatalityData;
        public static DataTable LargeTrucks;
        public static DataTable MotorCoaches;
        public static DataTable Buses;
        public static DataTable AllFatalities;

        public static List<State> States;
        public static List<string> Vehicles;
        public static List<string> Years;

        public static void LoadData()
        {
            States = ReadStateDataFile(Constants.STATES_DATA_FILE_NAME);

            FatalityData = new DataSet(Constants.DATA_SET_NAME);

            LargeTrucks = ReadDataFile(Constants.LT_DATA_FILE, Constants.LT_DATA_NAME);
            MotorCoaches = ReadDataFile(Constants.MC_DATA_FILE, Constants.MC_DATA_NAME);
            Buses = ReadDataFile(Constants.BS_DATA_FILE, Constants.BS_DATA_NAME);
            AllFatalities = ReadDataFile(Constants.AV_DATA_FILE, Constants.AV_DATA_NAME);

            FatalityData.Tables.Add(LargeTrucks);
            FatalityData.Tables.Add(MotorCoaches);
            FatalityData.Tables.Add(Buses);
            FatalityData.Tables.Add(AllFatalities);

            Vehicles = PopulateVehicles(FatalityData);
            Years = PopulateYears(LargeTrucks);
        }

        #region Data Retrieval...
        private static List<State> ReadStateDataFile(string Filename)
        {
            List<State> states = new List<State>();
            try
            {
                using (StreamReader sr = new StreamReader(File.Open(Filename, FileMode.Open)))
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
            return states;
        }

        private static DataTable ReadDataFile(string Filename, string DataTableName)
        {
            DataTable table = new DataTable(DataTableName);
            try
            {
                using (StreamReader sr = new StreamReader(File.Open(Filename, FileMode.Open)))
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
            return years;
        }
        #endregion

        #region Select Data...
        public static Dictionary<string, int> SelectDataByStateAndVehicleType(string State, string VehicleType)
        {
            DataTable table = new DataTable();
            DataTableCollection dtc = FatalityData.Tables;
            object[] rowData = null;
            foreach (DataTable dt in dtc)
            {
                if (dt.TableName.Equals(VehicleType))
                {
                    table = dt;
                }
            }
            foreach (DataRow row in table.Rows)
            {
                if (row[0].ToString().Equals(State))
                {
                    rowData = row.ItemArray;
                }
            }
            Dictionary<string, int> data = new Dictionary<string, int>();
            for (int i = 0; i < rowData.Length - 1; i++)
            {
                data.Add(Years[i], (int)rowData[i + 1]);
            }
            return data;
        }

        public static Dictionary<string, int> SelectedDataByStateAndYear(string State, string Year)
        {
            DataTableCollection dtc = FatalityData.Tables;
            Dictionary<string, int> data = new Dictionary<string, int>();
            int j = 0;
            foreach (DataTable dt in dtc)
            {
                foreach (DataRow row in dt.Rows)
                {
                    if (row[0].ToString().Equals(State))
                    {
                        for (int i = 0; i < Years.Count; i++)
                        {
                            if (Year.Equals(Years[i]))
                            {
                                data.Add(dt.TableName, (int)row[i + 1]);
                            }
                        }
                    }
                }
                j++;
            }
            return data;
        }
        #endregion

        #region Debug...
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
