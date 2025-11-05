using System;
using System.Data;
using System.Configuration;
using Microsoft.Data.SqlClient;

namespace lift_simulator.Database
{
    public class DbConnection
    {
        private readonly string masterConnectionString;
        private readonly string databaseName = "LiftSimulatorDB";
        private readonly string tableName = "LiftEvents";
        private string _connectionString;

        public string ConnectionString => _connectionString;

        public DbConnection()
        {
            try
            {
                masterConnectionString = ConfigurationManager.ConnectionStrings["LiftSimulatorMaster"]?.ConnectionString
                    ?? throw new ConfigurationErrorsException("Connection string 'LiftSimulatorMaster' not found");

                _connectionString = ConfigurationManager.ConnectionStrings["LiftSimulator"]?.ConnectionString
                    ?? throw new ConfigurationErrorsException("Connection string 'LiftSimulator' not found");

                EnsureDatabase();
                EnsureTable();
            }
            catch (Exception ex)
            {
                throw new Exception($"Database initialization failed: {ex.Message}", ex);
            }
        }

        private void EnsureDatabase()
        {
            try
            {
                using (var connection = new SqlConnection(masterConnectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $@"
                        IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'{databaseName}')
                        BEGIN
                            CREATE DATABASE [{databaseName}];
                        END";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating database '{databaseName}': {ex.Message}", ex);
            }
        }

        private void EnsureTable()
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $@"
                        IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='{tableName}' AND xtype='U')
                        BEGIN
                            CREATE TABLE {tableName} (
                                Id INT IDENTITY(1,1) PRIMARY KEY,
                                EventTime DATETIME DEFAULT GETDATE(),
                                Message NVARCHAR(500)
                            );
                        END";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error creating table '{tableName}': {ex.Message}", ex);
            }
        }

        public void LogEvent(string message)
        {
            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("Message", typeof(string));
                dt.Columns.Add("EventTime", typeof(DateTime));

                DataRow row = dt.NewRow();
                row["Message"] = message;
                row["EventTime"] = DateTime.Now;
                dt.Rows.Add(row);

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.InsertCommand = new SqlCommand(
                        "INSERT INTO LiftEvents (Message, EventTime) VALUES (@Message, @EventTime)",
                        connection);

                    adapter.InsertCommand.Parameters.Add("@Message", SqlDbType.NVarChar, 500).SourceColumn = "Message";
                    adapter.InsertCommand.Parameters.Add("@EventTime", SqlDbType.DateTime).SourceColumn = "EventTime";

                    adapter.Update(dt);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LogEvent error: {ex.Message}");
            }
        }

        public DataTable GetAllEvents()
        {
            try
            {
                var dt = new DataTable();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = $"SELECT Id, EventTime, Message FROM {tableName} ORDER BY EventTime";

                    // DataAdapter loads data into memory
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                    // Connection closes here, but data remains in DataTable
                }
                return dt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllEvents error: {ex.Message}");
                return new DataTable();
            }
        }

        public void DeleteAllEvents()
        {
            try
            {
                DataTable dt = new DataTable();
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = new SqlCommand($"SELECT Id FROM {tableName}", connection);
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }

                foreach (DataRow row in dt.Rows)
                {
                    row.Delete();
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.DeleteCommand = new SqlCommand(
                        $"DELETE FROM {tableName} WHERE Id = @Id",
                        connection);

                    adapter.DeleteCommand.Parameters.Add("@Id", SqlDbType.Int).SourceColumn = "Id";

                    adapter.Update(dt);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteAllEvents error: {ex.Message}");
                throw new Exception($"Error deleting events: {ex.Message}", ex);
            }
        }
    }
}