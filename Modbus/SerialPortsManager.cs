using Project1.Modbus.Models;
using System.IO.Ports;

namespace Project1.Modbus
{
    public class SerialPortsManager
    {
        private Dictionary<string, SerialPort>? _existingSerialPorts;

        public SerialPort? CreateAndOpenSerialPort(ModbusMasterConnectionSettings settings)
        {
            if (settings == null)
                return null;

            if (_existingSerialPorts == null)
                _existingSerialPorts = new ();

            if (_existingSerialPorts.ContainsKey(settings.Port))
                return _existingSerialPorts[settings.Port];

            var serialPort = new SerialPort();
            serialPort.PortName = settings.Port;
            serialPort.BaudRate = settings.Speed;
            serialPort.DataBits = settings.DataBits;
            serialPort.Parity = settings.Parity;
            serialPort.StopBits = settings.StopBits;
            serialPort.Open();

            _existingSerialPorts.Add(settings.Port, serialPort);

            return serialPort;
        }

        public void CloseSerialPortByPort(string port)
        {
            if (_existingSerialPorts != null && _existingSerialPorts.ContainsKey(port))
            {
                _existingSerialPorts[port].Close();
                _existingSerialPorts[port].Dispose();
                _existingSerialPorts.Remove(port);
            }
        }

        public void CloseSerialPortsByPorts(string[] ports)
        {
            foreach (var port in ports)
                CloseSerialPortByPort(port);
        }
        
        public void CloseAllSerialPorts()
        {
            if (_existingSerialPorts == null || !_existingSerialPorts.Any())
                return;

            foreach (var port in _existingSerialPorts.Keys)
                CloseSerialPortByPort(port);
        }
    }
}
