using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace lift_simulator
{
    public class DbConnection
    {
        private readonly string masterConnectionString =
            "Server=ROHIT\\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";

        private readonly string databaseName = "LiftSimulatorDB";
        private readonly string tableName = "ElevatorLogs";

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
                            EventType NVARCHAR(100),
                            Message NVARCHAR(500)
                        );
                    END";
                cmd.ExecuteNonQuery();
            }
        }

        public void LogEvent(string eventType, string message)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var cmd = connection.CreateCommand();

                // Explicitly insert EventTime with GETDATE()
                cmd.CommandText = $"INSERT INTO {tableName} (EventTime, EventType, Message) VALUES (GETDATE(), @EventType, @Message)";

                cmd.Parameters.AddWithValue("@EventType", eventType);
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
                cmd.CommandText = $"SELECT Id, EventTime, EventType, Message FROM {tableName} ORDER BY EventTime DESC";

                using (var adapter = new SqlDataAdapter(cmd))
                {
                    adapter.Fill(dt);
                }
            }

            return dt;
        }
    }
}