using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Project1.Models;
using System;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;

namespace Project1.Database
{
    public class DatabaseWorker
    {
        //private SqliteConnection _connection;
        private string _connectionString;
        public DatabaseWorker(string connectionString) 
        {
            _connectionString = connectionString;
            //_connection = new SqliteConnection(connectionString);
            //_connection.Open();
        }

        private string _getCommandUpdateVariable(Variable variable) => $"UPDATE variables SET value={variable.Value.ToString()} WHERE id={variable.Id};";

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
                        TrendingPeriod = reader.GetInt32(6)
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

        public async Task<int> WriteTrendValue(List<Variable> variableList)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = "";

            variableList.ForEach(variable => {
                command.CommandText += $"INSERT INTO trends (variable,datetime,value) VALUES ({variable.Id},{DateTime.Now.Ticks},{variable.Value})";
            });

            await connection.DisposeAsync();
            return command.ExecuteNonQuery();
        }

        public async Task<List<Trend>> ReadTrends(List<Variable> variables, DateTime? startTime, DateTime? endTime)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var trends = new List<Trend>();
            var command = connection.CreateCommand();
            var selectVariables = "";

            variables.ForEach(variable => { 
                if (selectVariables=="")
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
                trendData.Add((id,datetime, value));
            }

            foreach(var trend in trends)
            {
                trend.Data = trendData?
                    .Where(data=>data.Item1==trend.Variable.Id)?
                    .Select(data=>new TrendPoint() { Date = data.Item2, Value = data.Item3} )?
                    .OrderBy(data=>data.Date)?
                    .ToArray();
            }

            await connection.DisposeAsync();
            return trends;
        }

        public async Task<List<Event>> ReadEvents(List<Variable> variables, List<EventType> eventTypes)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM events;";
            var events = new List<Event>();
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var variable = variables.FirstOrDefault(_ => _.Id == reader.GetInt32(2));
                var eventTypeId = reader.GetInt32(1);
                var eventType = eventTypes.FirstOrDefault(_ => _.Id == eventTypeId);

                var ev = new Event() 
                { 
                    Id = reader.GetInt32(0),
                    Type = eventType,
                    Variable = variable,
                    Limit = ValuesParsing.Parse(reader.GetString(3),variable.Type),
                    Comparison = (EventVariableComparison)reader.GetInt32(4),
                    Message = reader.GetString(5)
                };
                events.Add(ev);
            }

            await connection.DisposeAsync();
            return events;
        }
        
        public async Task<bool> DeleteEvent(long id)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = $"DELETE FROM events WHERE id={id};";
            var result = command.ExecuteNonQuery() > 0;
            await connection.DisposeAsync();
            return result;
        }

        public async Task<bool> InsertEvent(Event ev)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO events (type,variable,limit,comparision,message) VALUES ({ev.Type},{ev.Variable.Id},{ev.Limit},{ev.Comparison},{ev.Message});";
            var result = command.ExecuteNonQuery() > 0;

            await connection.DisposeAsync();
            return result;
        }

        public async Task<List<EventHistory>> ReadOpenedEventsHistory(List<Event> events)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var listEventHistory = new List<EventHistory>();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM events_history WHERE end_time=0";
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var ev = events.FirstOrDefault(_=>_.Id==reader.GetInt64(1));

                if (ev != null)
                {
                    listEventHistory.Add(new EventHistory()
                    {
                        Id = reader.GetInt32(0),
                        Event = ev,
                        StartTime = new DateTime(reader.GetInt64(2)),
                        EndTime = new DateTime(reader.GetInt64(3))
                    });
                }
            }

            await connection.DisposeAsync();
            return listEventHistory;
        }

        public async Task<(List<Event>,List<EventHistory>)> ReadEventsAndEventsHistory(List<Variable> variables, List<EventType> eventTypes, Variable? variable = null, DateTime? start=null, DateTime? end=null, int? eventTypeId= null, int? count = 0)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var listEvent = new List<Event>();
            var listEventHistory = new List<EventHistory>();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT ";

            if (count!=null
                && count > 0)
            {
                command.CommandText += $"TOP {count} ";
            }

            command.CommandText += "events_history.id, events_history.event, events_history.start_time, events_history.end_time, events.type, events.variable, events.limit, events.comparision, events.message FROM events_history, events WHERE events_history.event=event.id";
            if (variable != null)
                command.CommandText += $" AND {variable.Id}=events.variable";
            if (start != null && start.HasValue)
                command.CommandText += $" AND {start.Value.Ticks}<events_history.start_time";
            if (end != null && end.HasValue)
                command.CommandText += $" AND {end.Value.Ticks}>events_history.end_time";
            if (eventTypeId!=null)
                command.CommandText += $" AND {eventTypeId}=events.type";

            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var eventId = reader.GetInt32(1);
                Event? ev= listEvent.FirstOrDefault(_ => _.Id == eventId);

                if (ev==null)
                {
                    var eventVariable = variables.FirstOrDefault(_ => _.Id == reader.GetInt32(5));
                    var eventTypeIdFromDB = reader.GetInt32(4);
                    var eventType = eventTypes.FirstOrDefault(_ => _.Id == eventTypeIdFromDB);

                    ev = new Event()
                    {
                        Id = eventId,
                        Type = eventType,
                        Variable = eventVariable,
                        Limit = ValuesParsing.Parse(reader.GetString(6), eventVariable.Type),
                        Comparison = (EventVariableComparison)reader.GetInt32(7),
                        Message = reader.GetString(8)
                    };
                    listEvent.Add(ev);
                }

                listEventHistory.Add(new EventHistory()
                {
                    Id = reader.GetInt32(0),
                    Event = ev,
                    StartTime = new DateTime(reader.GetInt32(2)),
                    EndTime = new DateTime(reader.GetInt32(3))
                });
            }

            await connection.DisposeAsync();
            return (listEvent, listEventHistory);
        }

        public async Task<List<EventHistory>> ReadEventsHistory(List<Event> events, Variable? variable = null, DateTime? start = null, DateTime? end = null, List<int>? types = null, int? count = 0)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var listEventHistory = new List<EventHistory>();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT ";

            if (count!=null
                && count > 0)
            {
                command.CommandText += $"TOP {count} ";
            }

            command.CommandText += "events_history.id, events_history.event, events_history.start_time, events_history.end_time, events.type, events.variable, events.value_to_compare, events.comparision, events.message FROM events_history, events WHERE events_history.event=events.id";

            if (variable != null)
                command.CommandText += $" AND {variable.Id}=events.variable";

            if (start != null && start.HasValue)
                command.CommandText += $" AND {start.Value.Ticks}<events_history.start_time";

            if (end != null && end.HasValue)
                command.CommandText += $" AND {end.Value.Ticks}>events_history.end_time";

            if (types != null
                && types.Count>0)
            {
                command.CommandText += $" AND events.type IN ({string.Join(',',types)})";
            }
                
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var eventId = reader.GetInt32(1);
                Event? ev = events.FirstOrDefault(_ => _.Id == eventId);

                listEventHistory.Add(new EventHistory()
                {
                    Id = reader.GetInt32(0),
                    Event = ev,
                    StartTime = new DateTime(reader.GetInt64(2)),
                    EndTime = reader.GetInt64(3) > 0
                    ? new DateTime(reader.GetInt64(3))
                    : null
                });
            }

            await connection.DisposeAsync();
            return listEventHistory;
        }

        public async Task<bool> WriteEventsHistory(EventHistory eventHistory)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            var command = connection.CreateCommand();
            command.CommandText = $"INSERT INTO events_history (event, start_time, end_time) VALUES ({eventHistory.Event.Id},{eventHistory.StartTime?.Ticks??0},{eventHistory.EndTime?.Ticks??0});";
            var result = command.ExecuteNonQuery() > 0;

            await connection.DisposeAsync();
            return result;
        }

        public async Task<int> WriteEventsHistories(List<EventHistory> eventHistories)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "";
            eventHistories.ForEach(eventHistory =>
            command.CommandText += $"INSERT INTO events_history (event, start_time, end_time) VALUES ({eventHistory.Event.Id},{eventHistory.StartTime?.Ticks??0},{eventHistory.EndTime?.Ticks??0});"
            );

            var result = await command.ExecuteNonQueryAsync();
            await connection.DisposeAsync();
            return result;
        }

        public async Task<bool> WriteEndTimeEventHistory(EventHistory eventHistory)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();

            if (eventHistory == null
                || !eventHistory.EndTime.HasValue)
            {
                return false;
            }

            command.CommandText = $"UPDATE events_history SET end_time={eventHistory.EndTime.Value.Ticks} WHERE id={eventHistory.Id};"; 

            return command.ExecuteNonQuery() > 0;
        }

        public async Task<List<Color>> ReadColors()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var colorsList = new List<Color>();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Red, Green, Blue FROM colors";
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                colorsList.Add(new Color()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Red = reader.GetInt32(2),
                    Green = reader.GetInt32(3),
                    Blue = reader.GetInt32(4),
                });
            }

            await connection.DisposeAsync();
            return colorsList;
        }

        public async Task<List<EventType>> ReadEventTypes()
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            
            var eventTypesList = new List<EventType>();
            var colorsList = await ReadColors();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT Id, Name, Color FROM event_types";
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var colorId = reader.GetInt32(2);
                Color? color = colorsList.FirstOrDefault(_ => _.Id == colorId);

                eventTypesList.Add(new EventType()
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Color = color
                });
            }

            await connection.DisposeAsync();
            return eventTypesList;
        }

    }
}
