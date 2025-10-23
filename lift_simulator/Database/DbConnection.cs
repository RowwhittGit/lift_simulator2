using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace lift_simulator.Database
{
    public class DbConnection
    {
        private readonly string masterConnectionString =
            "Server=ROHIT\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";
        private readonly string databaseName = "LiftSimulatorDB";
        private readonly string tableName = "LiftEvents";
        
        public string ConnectionString =>
            $"Server=ROHIT\\SQLEXPRESS;Database={databaseName};Trusted_Connection=True;TrustServerCertificate=True;";

        public DbConnection()
        {
            EnsureDatabase();
            EnsureTable();
        }

        private void EnsureDatabase()
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

        private void EnsureTable()
        {
            using (var connection = new SqlConnection(ConnectionString))
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

        public void LogEvent(string message)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                cmd.CommandText = @"
            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='LiftEvents' AND xtype='U')
            BEGIN
                CREATE TABLE LiftEvents (
                    Id INT IDENTITY(1,1) PRIMARY KEY,
                    EventTime DATETIME DEFAULT GETDATE(),
                    Message NVARCHAR(255)
                );
            END;
            INSERT INTO LiftEvents (Message) VALUES (@Message);
        ";
                cmd.Parameters.AddWithValue("@Message", message);
                cmd.ExecuteNonQuery();
            }
        }

        public DataTable GetAllEvents()
        {
            var dt = new DataTable();
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();
                // FIXED: Only select columns that actually exist in the table
                cmd.CommandText = $"SELECT Id, EventTime, Message FROM {tableName} ORDER BY EventTime";
                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}