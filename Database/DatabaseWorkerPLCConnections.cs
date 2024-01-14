using Microsoft.Data.Sqlite;
using Project1.Modbus.Models;
using System.IO.Ports;

namespace Project1.Database
{
    public class DatabaseWorkerPLCConnections
    {
        private string _connectionString;

        public DatabaseWorkerPLCConnections(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<ModbusMasterConnectionSettings>> ReadPLCConnections()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM plc_connections";

            var reader = command.ExecuteReader();

            var plcConnections = new List<ModbusMasterConnectionSettings>();

            while (reader.Read())
            {
                plcConnections.Add(
                    new ModbusMasterConnectionSettings(
                        reader.GetInt32(0),
                        (ConnectionTypeEnum)reader.GetInt32(1),
                        reader.GetString(2),
                        reader.GetString(3),
                        reader.GetInt32(4),
                        reader.GetInt32(5),
                        (Parity)reader.GetInt32(6),
                        (StopBits)reader.GetInt32(7),
                        reader.GetInt32(8),
                        reader.GetInt32(9)));
            }

            await connection.DisposeAsync();
            return plcConnections;
        }

    }
}
