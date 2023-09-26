using Microsoft.Data.Sqlite;
using Project1.Models;

namespace Project1.Database
{
    public class DatabaseWorkerEvents
    {
        private string _connectionString;

        public DatabaseWorkerEvents(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Event>> ReadEvents(List<Variable> variables, List<EventType> eventTypes)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM events;";
            var events = new List<Event>();
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var variable = variables.FirstOrDefault(_ => _.Id == reader.GetInt32(2));
                var eventTypeId = reader.GetInt32(1);
                var eventType = eventTypes.FirstOrDefault(_ => _.Id == eventTypeId);

                if (variable != null)
                {
                    var ev = new Event()
                    {
                        Id = reader.GetInt32(0),
                        Type = eventType,
                        Variable = variable,
                        Limit = ValuesParsing.Parse(reader.GetString(3), variable.Type),
                        Comparison = (EventVariableComparison)reader.GetInt32(4),
                        Message = reader.GetString(5)
                    };
                    events.Add(ev);
                }
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
                var ev = events.FirstOrDefault(_ => _.Id == reader.GetInt64(1));

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

        public async Task<(List<Event>, List<EventHistory>)> ReadEventsAndEventsHistory(
            List<Variable> variables, 
            List<EventType> eventTypes, 
            Variable? variable = null, 
            DateTime? start = null, 
            DateTime? end = null, 
            int? eventTypeId = null, 
            int? count = 0)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();
            var listEvent = new List<Event>();
            var listEventHistory = new List<EventHistory>();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT ";

            if (count != null
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
            if (eventTypeId != null)
                command.CommandText += $" AND {eventTypeId}=events.type";

            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var eventId = reader.GetInt32(1);
                Event? ev = listEvent.FirstOrDefault(_ => _.Id == eventId);

                if (ev == null)
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

        public async Task<List<EventHistory>> ReadEventsHistory(
            List<Event> events, 
            Variable? variable = null, 
            DateTime? start = null, 
            DateTime? end = null, 
            List<int>? types = null, 
            int? count = 0)
        {
            var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync();

            var listEventHistory = new List<EventHistory>();
            var command = connection.CreateCommand();
            command.CommandText = "SELECT ";

            if (count != null
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
                && types.Count > 0)
            {
                command.CommandText += $" AND events.type IN ({string.Join(',', types)})";
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
            command.CommandText = $"INSERT INTO events_history (event, start_time, end_time) VALUES ({eventHistory.Event.Id},{eventHistory.StartTime?.Ticks ?? 0},{eventHistory.EndTime?.Ticks ?? 0});";
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
            command.CommandText += $"INSERT INTO events_history (event, start_time, end_time) VALUES ({eventHistory.Event.Id},{eventHistory.StartTime?.Ticks ?? 0},{eventHistory.EndTime?.Ticks ?? 0});"
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
