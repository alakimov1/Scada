using Project1.Database;
using Project1.Modbus;
using Project1.Modbus.Models;
using Project1.Models;

namespace Project1.Processors
{
    public class PLCConnectionProcessor
    {
        private readonly DatabaseWorker _databaseWorker;
        private List<ModbusMasterConnectionSettings> _connectionSettings;
        private readonly ModbusMasterService _modbusService;
        private Dictionary<int, ushort[]> _registersByConnections;
        private Dictionary<int, (ushort Min, ushort Length)> _minAndLengthRegisterAddressByConnections;

        public PLCConnectionProcessor(
            DatabaseWorker databaseWorker)
        {
            _databaseWorker = databaseWorker;
            _modbusService = new ModbusMasterService();
        }

        public async Task Init(Variable[] variables)
        {
            var connectionIds = variables.Select(x => x.Connection).Distinct().ToList();
            _connectionSettings = await _databaseWorker.DatabaseWorkerPLCConnections.ReadPLCConnections();
            var connectionsByIds = _connectionSettings
                .Where(connection => connectionIds.Contains(connection.Id));
            _modbusService.CreateModbusDevices(connectionsByIds.ToArray());

            var minRegisterAddressByConnections = GetMinRegisterAddressByConnection(variables);
            var maxRegisterAddressByConnections = GetMaxRegisterAddressByConnection(variables);
            _minAndLengthRegisterAddressByConnections = minRegisterAddressByConnections.ToDictionary(
                x => x.Key,
                x => (x.Value, (ushort)(maxRegisterAddressByConnections[x.Key] - x.Value + 1)));
        }

        public async Task ReadVariables(List<Variable> variables)
        {
            _registersByConnections = await GetRegistersByConnections();
            
            foreach (var variable in variables)
                GetVariableValueFromRegisters(variable);
        }

        private void GetVariableValueFromRegisters(Variable variable)
        {
            var registers = _registersByConnections[variable.Connection];
            var minAddress = _minAndLengthRegisterAddressByConnections[variable.Connection].Min;

            variable.Value = variable.Type switch
            {
                VariableType.Bool => ConverterService.ToBoolFromRegisters(registers, variable.Address, minAddress),
                VariableType.Byte => ConverterService.ToByteFromRegisters(registers, variable.Address, minAddress),
                VariableType.Word => ConverterService.ToWordFromRegisters(registers, variable.Address, minAddress),
                VariableType.Dword => ConverterService.ToDWordFromRegisters(registers, variable.Address, minAddress),
                VariableType.Real => ConverterService.ToRealFromRegisters(registers, variable.Address, minAddress),
                _ => null
            };
        }

        private async Task<Dictionary<int, ushort[]>> GetRegistersByConnections()
        {
            var registersByConnection = new Dictionary<int, ushort[]>();
            _modbusService.ReconnectLostConnections();

            foreach (var addressByConnection in _minAndLengthRegisterAddressByConnections)
            {
                var registers = await _modbusService.ReadRegisters(
                    addressByConnection.Key, addressByConnection.Value.Min, addressByConnection.Value.Length);

                if (registers is null || registers.Length == 0)
                    continue;

                registersByConnection.Add(addressByConnection.Key, registers);
            }

            return registersByConnection;
        }

        private Dictionary<int, ushort> GetMaxRegisterAddressByConnection(Variable[] variables) =>
            variables.GroupBy(x => x.Connection).ToDictionary(
                x => x.Key, 
                variables => variables.Max(variable => GetMaxAddressRegistrerByVariable(variable)));

        private ushort GetMaxAddressRegistrerByVariable(Variable variable) =>
            (ushort)(variable.Type switch
            {
                VariableType.Real => variable.Address + 1,
                VariableType.Dword => variable.Address + 1,
                VariableType.Bool => variable.Address / 16,
                VariableType.String => variable.Address + ((string)variable.Value).Length,
                _ => variable.Address
            });

        private Dictionary<int, ushort> GetMinRegisterAddressByConnection(Variable[] variables) =>
            variables.GroupBy(x => x.Connection).ToDictionary(
                x => x.Key,
                variables => (ushort)variables.Min(variable =>
                variable.Type == VariableType.Bool
                ? variable.Address / 16
                : variable.Address));


        public async Task WriteVariable(Variable variable)
        {
            var connectionId = variable.Connection;
            var startAddress = _minAndLengthRegisterAddressByConnections[connectionId].Min;

            if (variable.Value == null)
                return;

            if (variable.Type == VariableType.Bool)
                await _modbusService.WriteRegister(
                    connectionId,
                    (ushort)(variable.Address / 16),
                    ConverterService.FromBoolToRegisters(
                        (bool)variable.Value,
                        _registersByConnections[connectionId],
                        variable.Address,
                        startAddress));
            else if (variable.Type == VariableType.Dword)
                await _modbusService.WriteRegisters(
                    connectionId,
                    (ushort)variable.Address,
                    ConverterService.FromValueToRegisters(
                        (int)variable.Value,
                        _registersByConnections[connectionId],
                        variable.Address,
                        startAddress));
            else if (variable.Type == VariableType.Real)
                await _modbusService.WriteRegisters(
                    connectionId,
                    (ushort)variable.Address,
                    ConverterService.FromValueToRegisters(
                        (float)variable.Value,
                        _registersByConnections[connectionId],
                        variable.Address,
                        startAddress));
            else if (variable.Type == VariableType.Byte)
                await _modbusService.WriteRegisters(
                    connectionId,
                    (ushort)variable.Address,
                    ConverterService.FromValueToRegisters(
                        (byte)variable.Value,
                        _registersByConnections[connectionId],
                        variable.Address,
                        startAddress));
            else if (variable.Type == VariableType.Word)
                await _modbusService.WriteRegisters(
                    connectionId,
                    (ushort)variable.Address,
                    ConverterService.FromValueToRegisters(
                        (short)variable.Value,
                        _registersByConnections[connectionId],
                        variable.Address,
                        startAddress));
            else
                return;
        }

        public void CloseConnections() => _modbusService.CloseAllConnections();
    }
}
