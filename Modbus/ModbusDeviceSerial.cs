using Modbus.Device;
using Project1.Modbus.Models;

namespace Project1.Modbus
{
    public class ModbusDeviceSerial: IModbusDevice
    {
        public ModbusDeviceSerial(IModbusSerialMaster master, ModbusMasterConnectionSettings settings) 
        {
            _master = master;
            Settings = settings;
            Id = settings.Id;
        }

        private readonly IModbusSerialMaster _master;

        public int Id { get; }
        public ModbusMasterConnectionSettings Settings { get; }
        public IModbusMaster Master => _master;
        private byte _address => byte.Parse(Settings.Address);

        public async Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort length = 1) => 
            await _master.ReadCoilsAsync(_address, startAddress, length);

        public async Task<ushort[]> ReadRegistersAsync(ushort startAddress, ushort length = 1) =>
            await _master.ReadHoldingRegistersAsync(_address, startAddress, length);

        public async Task WriteCoilsAsync(ushort startAddress, bool[] data)
        {
            if (data == null || data.Length == 0)
                return;

            if (data.Length == 1)
                await _master.WriteSingleCoilAsync(_address, startAddress, data[0]);

            await _master.WriteMultipleCoilsAsync(_address, startAddress, data);
        }

        public async Task WriteRegistersAsync(ushort startAddress, ushort[] data)
        {
            if (data == null || data.Length == 0)
                return;

            if (data.Length == 1)
                await _master.WriteSingleRegisterAsync(_address, startAddress, data[0]);

            await _master.WriteMultipleRegistersAsync(_address, startAddress, data);
        }
    }
}
