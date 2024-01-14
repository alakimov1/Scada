using Modbus.Device;
using Project1.Modbus.Models;

namespace Project1.Modbus
{
    public interface IModbusDevice
    {
        public int Id { get; }
        public IModbusMaster Master { get; }
        public ModbusMasterConnectionSettings Settings { get; }

        public Task<bool[]> ReadCoilsAsync(ushort startAddress, ushort length = 1);
        public Task<ushort[]> ReadRegistersAsync(ushort startAddress, ushort length = 1);
        public Task WriteCoilsAsync(ushort startAddress, bool[] data);
        public Task WriteRegistersAsync(ushort startAddress, ushort[] data);
    }
}
