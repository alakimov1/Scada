using Microsoft.Data.Sqlite;
using Project1.Models;

namespace Project1.Database
{
    public class DatabaseWorkerTrends
    {
        private string _connectionString;

        public DatabaseWorkerTrends(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<int> WriteTrendValue(List<Variable> variableList)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "";

            variableList.ForEach(variable => {
                command.CommandText += $"INSERT INTO trends (variable,datetime,value) VALUES ({variable.Id},{DateTime.Now.Ticks},{variable.Value});";
            });

            var result = command.ExecuteNonQuery();
            await connection.DisposeAsync();
            return result;
        }

        public async Task<List<Trend>> ReadTrends(List<Variable> variables, DateTime? startTime, DateTime? endTime)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var trends = new List<Trend>();
            var command = connection.CreateCommand();
            var selectVariables = "";

            variables.ForEach(variable => {
                if (selectVariables == "")
                {
                    selectVariables += $"{variable.Id}";
                }
                else
                {
                    selectVariables += $" OR {variable.Id}";
                }

                trends.Add(
                    new Trend()
                    {
                        Data = null,
                        Start = startTime ?? DateTime.MinValue,
                        End = endTime ?? DateTime.Now,
                        Variable = variable
                    }
                );
            });

            command.CommandText = $"SELECT variable, datetime, value FROM trends WHERE datetime<={endTime.Value.Ticks} AND datetime>={startTime.Value.Ticks} ORDER BY variable;";

            if (!string.IsNullOrEmpty(selectVariables))
            {
                command.CommandText += $" AND ({selectVariables})";
            }

            command.CommandText += ";";
            var reader = command.ExecuteReader();

            var trendData = new List<(int, DateTime, string)>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var datetime = new DateTime(reader.GetInt64(1));
                var value = reader.GetString(2);
                trendData.Add((id, datetime, value));
            }

            foreach (var trend in trends)
            {
                trend.Data = trendData?
                    .Where(data => data.Item1 == trend.Variable.Id)?
                    .Select(data => new TrendPoint() { Date = data.Item2, Value = data.Item3 })?
                    .OrderBy(data => data.Date)?
                    .ToArray();
            }

            await connection.DisposeAsync();
            return trends;
        }

    }
}
