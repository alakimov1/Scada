using Microsoft.Win32;
using System.Net;

namespace Project1.Modbus
{
    public static class ConverterService
    {
        public static bool ToBoolFromRegisters(ushort[] registers, int address, ushort startAddress)
        {
            var registerAddress = address / 16 - startAddress;
            var bitAddress = address - registerAddress;
            var register = registers[registerAddress];

            return (register & (ushort)(1 << bitAddress)) != 0;
        }

        public static byte ToByteFromRegisters(ushort[] registers, int address, ushort startAddress) =>
            (byte)(registers[address - startAddress] % 256);

        public static short ToWordFromRegisters(ushort[] registers, int address, ushort startAddress) =>
            BitConverter.ToInt16(new Span<byte>(GetBytesFromRegister(registers[address - startAddress])));

        public static int ToDWordFromRegisters(ushort[] registers, int address, ushort startAddress) =>
            BitConverter.ToInt32(new Span<byte>(
                GetBytesFromRegister(registers[address - startAddress], registers[address - startAddress + 1])));

        public static float ToRealFromRegisters(ushort[] registers, int address, ushort startAddress) =>
            BitConverter.ToSingle(new Span<byte>(
                GetBytesFromRegister(registers[address - startAddress], registers[address - startAddress + 1])));

        private static byte[] GetBytesFromRegister(ushort register) =>
            BitConverter.GetBytes(register);

        private static byte[] GetBytesFromRegister(ushort register1, ushort register2)
        {
            var register1Bytes = GetBytesFromRegister(register1);
            var register2Bytes = GetBytesFromRegister(register2);
            return new byte[] { register1Bytes[0], register1Bytes[1], register2Bytes[0], register2Bytes[1] };
        }

        public static ushort FromBoolToRegisters(bool value, ushort[] registers, int address, ushort startAddress)
        {
            var registerAddress = address / 16 - startAddress;
            var bitAddress = address - registerAddress;
            var register = registers[registerAddress];
            var valueBitOperator = (ushort)(1 << bitAddress);

            return (ushort)(value ? register | valueBitOperator : register & ~valueBitOperator);
        }

        public static ushort[] FromValueToRegisters(uint value, ushort[] registers, int address, ushort startAddress) =>
            GetRegistersFromBytes(BitConverter.GetBytes(value));

        public static ushort[] FromValueToRegisters(float value, ushort[] registers, int address, ushort startAddress) =>
            GetRegistersFromBytes(BitConverter.GetBytes(value));

        public static ushort[] FromValueToRegisters(byte value, ushort[] registers, int address, ushort startAddress) =>
            GetRegistersFromBytes(new byte[] { value });

        private static ushort GetRegisterFromBytes(byte byte1, byte byte2) =>
            BitConverter.ToUInt16(new Span<byte>(new byte[] { byte1, byte2 }));

        private static ushort[] GetRegistersFromBytes(byte[] bytes) 
        {
            var result = new ushort[bytes.Length / 2 + bytes.Length % 2];
            var i = 0;

            while(i < bytes.Length)
            {
                result[i / 2] =  i + 1 == bytes.Length
                    ? GetRegisterFromBytes(bytes[i], 0)
                    : GetRegisterFromBytes(bytes[i], bytes[i + 1]);
                i += 2;
            }

            return result;
        }
    }
}
