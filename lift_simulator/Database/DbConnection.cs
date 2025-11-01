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
                // Read from App.config - portable!
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
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    var cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO LiftEvents (Message) VALUES (@Message)";
                    cmd.Parameters.AddWithValue("@Message", message);
                    cmd.ExecuteNonQuery();
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
                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        adapter.Fill(dt);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetAllEvents error: {ex.Message}");
                return new DataTable();
            }
        }
    }
}