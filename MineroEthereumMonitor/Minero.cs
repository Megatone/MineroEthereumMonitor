using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MineroEthereumMonitor
{
    class Minero
    {

        private int id { get; set; }
        private string nombre { get; set; }
        private int gpus { get; set; }
        private string direccionIP { get; set; }
        private string hashrate { get; set; }
        private string token { get; set; }
        private int timeout { get; set; }
        private string poolServer1 { get; set; }
        private int portPoolServer1 { get; set; }
        private string poolServer2 { get; set; }
        private int portPoolServer2 { get; set; }
        private string account { get; set; }

        public Minero(string _nombre)
        {
            this.nombre = _nombre;
            using (MD5 md5Hash = MD5.Create())
            {
                this.token = GetMd5Hash(md5Hash, string.Concat(Environment.MachineName, Environment.Is64BitOperatingSystem, Environment.Is64BitProcess, Environment.OSVersion, Environment.ProcessorCount, Environment.UserName, Environment.Version));
            }
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
    }
}
