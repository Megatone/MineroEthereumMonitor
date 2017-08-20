using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MineroEthereumMonitor
{
    public class Minero
    {

        public int id { get; set; }
        public string nombre { get; set; }
        public int gpus { get; set; }
        public string direccionIP { get; set; }
        public string hashrate { get; set; }
        public string token { get; set; }
        public int timeout { get; set; }
        public string poolServer1 { get; set; }
        public int portPoolServer1 { get; set; }
        public string poolServer2 { get; set; }
        public int portPoolServer2 { get; set; }
        public string account { get; set; }
        public bool minando { get; set; }

        public Minero(string _nombre)
        {
            this.id = 0;
            this.nombre = _nombre;
            this.gpus = 0;
            this.direccionIP = GetLocalIPv4(NetworkInterfaceType.Ethernet); 
            this.hashrate = "0";
            
            using (MD5 md5Hash = MD5.Create())
            {
                this.token = GetMd5Hash(md5Hash, string.Concat(Environment.MachineName, Environment.Is64BitOperatingSystem, Environment.Is64BitProcess, Environment.OSVersion, Environment.ProcessorCount, Environment.UserName, Environment.Version));
            }

            this.timeout = 0;
            this.poolServer1 = string.Empty;
            this.portPoolServer1 = 0;
            this.poolServer2 = string.Empty;
            this.portPoolServer2 = 0;
            this.account = string.Empty;
        }

        private string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        private string GetLocalIPv4(NetworkInterfaceType _type)
        {
            string output = "";
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.NetworkInterfaceType == _type && item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                        {
                            output = ip.Address.ToString();
                        }
                    }
                }
            }
            return output;
        }
    }
}
