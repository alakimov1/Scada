using System.IO.Ports;

namespace Project1.Modbus.Models
{
    public class ModbusMasterConnectionSettings
    {
        public ModbusMasterConnectionSettings(
            int id,
            ConnectionTypeEnum type,
            string address,
            string port,
            int speed = 9600,
            int dataBits = 8,
            Parity parity = Parity.None,
            StopBits stopBits = StopBits.One,
            int timeout = 1500,
            int retries = 3) 
        {
            Id = id;
            Type = type;
            Address = address;
            Port = port;
            Speed = speed;
            DataBits = dataBits;
            Parity = parity;
            StopBits = stopBits;
            Timeout = timeout;
            Retries = retries;
        }

        public int Id { get; }
        public ConnectionTypeEnum Type { get; }
        public string Address { get; }
        public string Port { get; }
        public int Speed { get; }
        public int DataBits { get; }
        public Parity Parity { get; }
        public StopBits StopBits { get; }
        public int Timeout { get; }
        public int Retries { get; }

        public bool IsSerial => Type == ConnectionTypeEnum.RTU || Type == ConnectionTypeEnum.ASCII;
    }
}
