using Microsoft.Data.Sqlite;
using Project1.Models;

namespace Project1.Database
{
    public class DatabaseWorker
    {
        private string _connectionString;
        public DatabaseWorkerEvents DatabaseWorkerEvents { get; private set; }
        public DatabaseWorkerTrends DatabaseWorkerTrends { get; private set; }
        public DatabaseWorkerPLCConnections DatabaseWorkerPLCConnections { get; private set; }

        public DatabaseWorker(string connectionString) 
        {
            _connectionString = connectionString;
            DatabaseWorkerEvents = new DatabaseWorkerEvents(connectionString);
            DatabaseWorkerTrends = new DatabaseWorkerTrends(connectionString);
            DatabaseWorkerPLCConnections = new DatabaseWorkerPLCConnections(connectionString);
        }

        private string _getCommandUpdateVariable(Variable variable) => 
            $"UPDATE variables SET value={variable.Value.ToString()} WHERE id={variable.Id};";

        public async Task<bool> WriteVariableValue(Variable variable)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = _getCommandUpdateVariable(variable);
            var result = command.ExecuteNonQuery() > 0;
            await connection.DisposeAsync();

            return result;
        }

        public async Task<int> WriteVariablesValues(List<Variable> variables) 
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "";

            variables.ForEach(variable =>
            {
                command.CommandText += _getCommandUpdateVariable(variable);
            });
            var result = command.ExecuteNonQuery();

            await connection.DisposeAsync();
            return result;
        }

        public async Task<List<Variable>> ReadVariables()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var variables = new List<Variable>(); 
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM variables";
            var reader = command.ExecuteReader();

            while (reader.Read()) 
            {
                var type = (VariableType)reader.GetInt32(2);
                variables.Add(new Variable()
                    {
                        Id = reader.GetInt32(0),
                        Address = reader.GetInt32(1),
                        Type = type,
                        Name = reader.GetString(3),
                        Value = ValuesParsing.Parse(reader.GetString(4), type),
                        Active = reader.GetInt32(5),
                        TrendingPeriod = reader.GetInt32(6),
                        Connection = reader.GetInt32(7),
                    });
            }

            await connection.DisposeAsync();
            return variables;
        }

        public async Task<bool> ReadVariableValue(Variable variable) 
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT value FROM variables WHERE id={variable.Id};";
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                await connection.DisposeAsync();
                return false;
            }

            reader.Read();
            variable.Value = ValuesParsing.Parse(reader.GetString(0), variable.Type);
            await connection.DisposeAsync();
            return true;
        }

        public async Task<bool> ReadVariablesValues(Variable[] variables)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT id, value FROM variables;";
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                await connection.DisposeAsync();
                return false;
            }

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var value = reader.GetString(1);
                var variable = variables.FirstOrDefault(_ => _.Id == id);
                variable.Value = ValuesParsing.Parse(value, variable?.Type);
            }

            await connection.DisposeAsync();
            return true;
        }

        public async Task<List<Group>> ReadGroups()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT id, name FROM groups;";
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                await connection.DisposeAsync();
                return null;
            }

            var groups = new List<Group>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                groups.Add(new Group() { Id = id, Name = name });
            }

            await connection.DisposeAsync();
            return groups;
        }

        public async Task<List<Subgroup>> ReadSubgroups()
        {
            var groups = await ReadGroups();

            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT id, name, \"group\" FROM subgroups;";
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                await connection.DisposeAsync();
                return null;
            }

            var subgroups = new List<Subgroup>();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                var group = reader.GetInt32(2);

                subgroups.Add(
                    new Subgroup() { Id = id, Name = name, Group = groups.FirstOrDefault(_ => _.Id == group) }
                    );
            }

            await connection.DisposeAsync();
            return subgroups;
        }

        public async Task<List<VariableEntity>> ReadVariablesEntities()
        {
            var subgroups = await ReadSubgroups();
            var variables = await ReadVariables();

            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT variable, subgroup, writable, id FROM variables_entity;";
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
            {
                await connection.DisposeAsync();
                return null;
            }

            var variablesEntities = new List<VariableEntity>();

            while (reader.Read())
            {
                var variable = reader.GetInt32(0);
                var subgroup = reader.GetInt32(1);
                var writable = reader.GetInt32(2);
                var id = reader.GetInt32(3);

                variablesEntities.Add(new VariableEntity() 
                { 
                    Variable = variables.FirstOrDefault(_=>_.Id == variable), 
                    Subgroup = subgroups.FirstOrDefault(_ => _.Id == subgroup),
                    Writable = writable>0,
                    Id = id
                }
                );
            }

            await connection.DisposeAsync();
            return variablesEntities;
        }
    }
}
