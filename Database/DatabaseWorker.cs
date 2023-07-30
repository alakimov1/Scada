using Microsoft.Data.Sqlite;
using Project1.Models;
using System;
using System.Security.Cryptography;

namespace Project1.Database
{
    public class DatabaseWorker
    {
        private SqliteConnection _connection;
        public DatabaseWorker(string connectionString) 
        {
            _connection = new SqliteConnection(connectionString);
            _connection.Open();
        }

        private string _getCommandUpdateVariable(Variable variable) => $"UPDATE variables SET value={variable.Value.ToString()} WHERE id={variable.Id};";

        public bool WriteVariableValue(Variable variable)
        {
            var command = _connection.CreateCommand();
            command.CommandText = _getCommandUpdateVariable(variable);
            return command.ExecuteNonQuery()>0;
        }

        public int WriteVariablesValues(List<Variable> variables) 
        {
            var command = _connection.CreateCommand();
            command.CommandText = "";
            variables.ForEach(variable => {
                command.CommandText += _getCommandUpdateVariable(variable);
            });
            return command.ExecuteNonQuery();
        }

        public List<Variable> ReadVariables()
        {
            var variables = new List<Variable>(); 
            var command = _connection.CreateCommand();
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

            return variables;
        }

        public bool ReadVariableValue(Variable variable) 
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT value FROM variables WHERE id={variable.Id};";
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return false;

            reader.Read();
            variable.Value = ValuesParsing.Parse(reader.GetString(0), variable.Type);
            return true;
        }

        public bool ReadVariablesValues(Variable[] variables)
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"SELECT id, value FROM variables;";
            var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return false;

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var value = reader.GetString(1);

                if (id!=null)
                {
                    var variable = variables.FirstOrDefault(_ => _.Id == id);
                    variable.Value = ValuesParsing.Parse(value, variable?.Type);
                }
            }
            return true;
        }

        public int WriteTrendValue(List<Variable> variableList)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "";
            variableList.ForEach(variable => {
                command.CommandText += $"INSERT INTO trends (variable,datetime,value) VALUES ({variable.Id},{DateTime.Now.Ticks},{variable.Value})";
            });
            return command.ExecuteNonQuery();
        }

        public List<Trend> ReadTrends(List<Variable> variables, DateTime? startTime, DateTime? endTime)
        {
            var trends = new List<Trend>();
            var command = _connection.CreateCommand();
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
                        Data = new List<(DateTime, object)>(),
                        Start = startTime??DateTime.MinValue,
                        End = endTime??DateTime.Now,
                        Variable = variable
                    }
                );
            });

            command.CommandText = $"SELECT variable, datetime, value FROM trends WHERE datetime<={endTime} AND datetime>={startTime} ORDER BY variable;";

            if (!string.IsNullOrEmpty(selectVariables))
            {
                command.CommandText += $" AND ({selectVariables})";
            }

            command.CommandText += ";";
            var reader = command.ExecuteReader();

            var currentTrend = trends.First();

            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var datetime = new DateTime(reader.GetInt64(1));
                var value = reader.GetString(2);

                if (currentTrend.Variable.Id!=id)
                {
                    currentTrend = trends.FirstOrDefault(_ => _.Variable.Id == id);

                    if (currentTrend == null)
                    {
                        throw new Exception("Ошибка в данных трендов");
                    }
                }

                currentTrend.Data.Add((datetime, value));
            }

            return trends;
        }

        public List<Event> ReadEvents(List<Variable> variables)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "SELECT * FROM events;";
            var events = new List<Event>();
            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var variable = variables.FirstOrDefault(_ => _.Id == reader.GetInt32(2));
                var ev = new Event() 
                { 
                    Id = reader.GetInt32(0),
                    Type = (EventType)reader.GetInt32(1),
                    Variable = variable,
                    Limit = ValuesParsing.Parse(reader.GetString(3),variable.Type),
                    Comparison = (EventVariableComparison)reader.GetInt32(4),
                    Message = reader.GetString(5)
                };
                events.Add(ev);
            }

            return events;
        }
        
        public bool DeleteEvent(long id)
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"DELETE FROM events WHERE id={id};";
            return command.ExecuteNonQuery()>0;
        }

        public bool InsertEvent(Event ev)
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"INSERT INTO events (type,variable,limit,comparision,message) VALUES ({ev.Type},{ev.Variable.Id},{ev.Limit},{ev.Comparison},{ev.Message});";
            return command.ExecuteNonQuery() > 0;
        }

        public List<EventHistory> ReadOpenedEventsHistory(List<Event> events)
        {
            var listEventHistory = new List<EventHistory>();
            var command = _connection.CreateCommand();
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
                        StartTime = new DateTime(reader.GetInt32(2)),
                        EndTime = new DateTime(reader.GetInt32(3))
                    });
                }
            }

            return listEventHistory;
        }

        public (List<Event>,List<EventHistory>) ReadEventsAndEventsHistory(List<Variable> variables, Variable? variable = null, DateTime? start=null, DateTime? end=null, EventType? type= null, int? count = 0)
        {
            var listEvent = new List<Event>();
            var listEventHistory = new List<EventHistory>();
            var command = _connection.CreateCommand();
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
            if (type!=null && type.HasValue)
                command.CommandText += $" AND {(int)type.Value}=events.type";

            var reader = command.ExecuteReader();

            while(reader.Read())
            {
                var eventId = reader.GetInt32(1);
                Event? ev= listEvent.FirstOrDefault(_ => _.Id == eventId);

                if (ev==null)
                {
                    var eventVariable = variables.FirstOrDefault(_ => _.Id == reader.GetInt32(5));
                    ev = new Event()
                    {
                        Id = eventId,
                        Type = (EventType)reader.GetInt32(4),
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

            return (listEvent, listEventHistory);
        }

        public List<EventHistory> ReadEventsHistory(List<Event> events, Variable? variable = null, DateTime? start = null, DateTime? end = null, EventType? type = null, int? count = 0)
        {
            var listEventHistory = new List<EventHistory>();
            var command = _connection.CreateCommand();
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
            if (type != null && type.HasValue)
                command.CommandText += $" AND {(int)type.Value}=events.type";

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var eventId = reader.GetInt32(1);
                Event? ev = events.FirstOrDefault(_ => _.Id == eventId);

                listEventHistory.Add(new EventHistory()
                {
                    Id = reader.GetInt32(0),
                    Event = ev,
                    StartTime = new DateTime(reader.GetInt32(2)),
                    EndTime = new DateTime(reader.GetInt32(3))
                });
            }

            return listEventHistory;
        }

        public bool WriteEventsHistory(EventHistory eventHistory)
        {
            var command = _connection.CreateCommand();
            command.CommandText = $"INSERT INTO events_history (event, start_time, end_time) VALUES ({eventHistory.Event.Id},{eventHistory.StartTime?.Ticks??0},{eventHistory.EndTime?.Ticks??0});";
            return command.ExecuteNonQuery() > 0;
        }

        public int WriteEventsHistories(List<EventHistory> eventHistories)
        {
            var command = _connection.CreateCommand();
            command.CommandText = "";
            eventHistories.ForEach(eventHistory =>
            command.CommandText += $"INSERT INTO events_history (event, start_time, end_time) VALUES ({eventHistory.Event.Id},{eventHistory.StartTime?.Ticks??0},{eventHistory.EndTime?.Ticks??0});"
            );
            return command.ExecuteNonQuery();
        }

        public bool WriteEndTimeEventHistory(EventHistory eventHistory)
        {
            var command = _connection.CreateCommand();

            if (eventHistory == null
                || !eventHistory.EndTime.HasValue)
            {
                return false;
            }

            command.CommandText = $"UPDATE events_history SET end_time={eventHistory.EndTime.Value.Ticks} WHERE id={eventHistory.Id};"; 
            return command.ExecuteNonQuery() > 0;
        }
    }
}
