using Modbus.Device;
using Project1.Modbus.Models;
using System.Net;

namespace Project1.Modbus
{
    public class ModbusDeviceTCP: IModbusDevice
    {
        public ModbusDeviceTCP(ModbusIpMaster master, ModbusMasterConnectionSettings settings)
        {
            _master = master;
            Settings = settings;
            Id = settings.Id;
        }

        private readonly ModbusIpMaster _master;

        public int Id { get; }
        public IModbusMaster Master => _master;
        public ModbusMasterConnectionSettings Settings { get; }
        
        public async Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort length = 1) =>
            await _master.ReadCoilsAsync(startAddress, length);

        public async Task<ushort[]> ReadRegistersAsync(ushort startAddress, ushort length = 1) =>
            await _master.ReadHoldingRegistersAsync(startAddress, length);

        public async Task WriteCoilsAsync(ushort startAddress, bool[] data)
        {
            if (data == null || data.Length == 0)
                return;

            if (data.Length == 1)
                await _master.WriteSingleCoilAsync(startAddress, data[0]);

            await _master.WriteMultipleCoilsAsync(startAddress, data);
        }

        public async Task WriteRegistersAsync(ushort startAddress, ushort[] data)
        {
            if (data == null || data.Length == 0)
                return;

            if (data.Length == 1)
                await _master.WriteSingleRegisterAsync(startAddress, data[0]);

            await _master.WriteMultipleRegistersAsync(startAddress, data);
        }
    }
}
