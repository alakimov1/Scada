using Modbus.Device;
using Project1.Modbus.Models;
using System.Net;

namespace Project1.Modbus
{
    public class ModbusMasterService
    {
        private List<IModbusDevice> _existingModbusDevices;
        private SerialPortsManager _serialPortsManager;

        public IModbusDevice CreateModbusDevice(ModbusMasterConnectionSettings settings)
        {
            if (_existingModbusDevices == null)
                _existingModbusDevices = new();

            var _existingDevice = _existingModbusDevices.FirstOrDefault(device => device.Id == settings.Id);

            if (_existingDevice != null)
                return _existingDevice;

            IModbusDevice modbusDevice = settings.Type == ConnectionTypeEnum.TCP
            ? CreateTCP(settings)
            : CreateSerial(settings);
            _existingModbusDevices.Add(modbusDevice);

            return modbusDevice;
        }

        public IModbusDevice[] CreateModbusDevices(ModbusMasterConnectionSettings[] settings) =>
            settings.Select(setting => CreateModbusDevice(setting)).ToArray();

        private IModbusDevice CreateTCP(ModbusMasterConnectionSettings settings)
        {
            var client = new System.Net.Sockets.TcpClient();
            var ipAddress = IPAddress.Parse(settings.Address);
            var portParseSuccess = int.TryParse(settings.Port, out var port);
            client.Connect(ipAddress, portParseSuccess ? port : 502);
            var modbusMaster = ModbusIpMaster.CreateIp(client);
            modbusMaster.Transport.Retries = 0;
            modbusMaster.Transport.ReadTimeout = 1500;

            return new ModbusDeviceTCP(modbusMaster, settings);
        }

        private IModbusDevice CreateSerial(ModbusMasterConnectionSettings settings)
        {
            if (_serialPortsManager == null)
                _serialPortsManager = new ();

            var serialPort = _serialPortsManager.CreateAndOpenSerialPort(settings);

            if (serialPort == null)
                return null;

            var modbusMaster = settings.Type == ConnectionTypeEnum.RTU
                ? ModbusSerialMaster.CreateRtu(serialPort)
                : ModbusSerialMaster.CreateAscii(serialPort);

            return new ModbusDeviceSerial(modbusMaster, settings);
        }

        public async Task<ushort[]> ReadRegisters(int connectionId, ushort startAddress, ushort length = 1)
        {
            var existing = _existingModbusDevices.SingleOrDefault(device => device.Id == connectionId);

            if (existing == null)
                return null;

            return await existing.ReadRegistersAsync(startAddress, length);
        }

        public async Task<bool> WriteRegister(int connectionId, ushort startAddress, ushort data) =>
            await WriteRegisters(connectionId, startAddress, new ushort[] { data });

        public async Task<bool> WriteRegisters(int connectionId, ushort startAddress, ushort[] data)
        {
            var existing = _existingModbusDevices.SingleOrDefault(device => device.Id == connectionId);

            if (existing == null)
                return false;

            await existing.WriteRegistersAsync(startAddress, data);
            return true;
        }

        public void CloseConnection(int id)
        {
            var device = _existingModbusDevices.SingleOrDefault(device => device.Id == id);

            if (device == null)
                return;

            _existingModbusDevices.Remove(device);

            var serialPortIsNeed = device.Settings.IsSerial 
                && !_existingModbusDevices.Any(d => d.Settings.Port == device.Settings.Port && d.Settings.IsSerial);

            if (serialPortIsNeed)
                _serialPortsManager.CloseSerialPortByPort(device.Settings.Port);

            ((ModbusMaster)device).Dispose();
        }

        public void CloseConnections(int[] ids)
        {
            foreach(var id in ids)
                CloseConnection(id);
        }

        public void CloseAllConnections() =>
            CloseConnections(_existingModbusDevices.Select(device => device.Id).Distinct().ToArray());
    }
}
