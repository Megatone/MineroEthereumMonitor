using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.SocketIoClientDotNet.Client;
using System;

namespace MineroEthereumMonitor
{
    class Program
    {

        private static string urlServidor = string.Empty;
        private static Minero minero;

        static void Main(string[] args)
        {
            string help = "Parametros Requqeridos: " + Environment.NewLine + Environment.NewLine + "\t -s <URL SERVIDOR>" + Environment.NewLine + "\t -n <NOMBRE MINERO>" + Environment.NewLine + Environment.NewLine + "Ejemplo : " + Environment.NewLine + Environment.NewLine + "\t MineroEthereumMonitor.exe -s http://localhost:8000 -n Minero1" + Environment.NewLine +  "\t MineroEthereumMonitor.exe -s http://192.168.1.12:8080 -n Minero7";
           
            string banner = @" _______  _                               _______       _                                        
(_______)(_)                             (_______) _   | |                                       
 _  _  _  _  ____   _____   ____  ___     _____  _| |_ | |__   _____   ____  _____  _   _  ____  
| ||_|| || ||  _ \ | ___ | / ___)/ _ \   |  ___)(_   _)|  _ \ | ___ | / ___)| ___ || | | ||    \ 
| |   | || || | | || ____|| |   | |_| |  | |_____ | |_ | | | || ____|| |    | ____|| |_| || | | |
|_|   |_||_||_| |_||_____)|_|    \___/   |_______) \__)|_| |_||_____)|_|    |_____)|____/ |_|_|_|" + Environment.NewLine + Environment.NewLine;

            Console.WriteLine(banner);

            if (args.Length == 4)
            {
                if (args[0] == "-s" && args[2] == "-n")
                {
                    urlServidor = args[1];
                    minero = new Minero(args[3]);
                    iniciarSocket();
                }
                else
                {
                    
                    Console.WriteLine(help);
                }
            }
            else
            {
               
                Console.WriteLine(help);
            }
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        private static void iniciarSocket() {
            string bar = "###########################################################################################################" + Environment.NewLine;
            var socket = IO.Socket(urlServidor );
          
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine(bar);
                Console.WriteLine("Conexion establecida con el servidor => " + urlServidor );
                Console.WriteLine("Sincronizando Información con el servidor...");
                socket.Emit("inicializarMinero", JObject.Parse(JsonConvert.SerializeObject(minero)));

            });

            socket.On(Socket.EVENT_CONNECT_ERROR, () =>{
                Console.WriteLine("ERROR CONECTION");
            });

            socket.On(Socket.EVENT_CONNECT_TIMEOUT, () => {
                Console.WriteLine("TIMEOUT CONECTION");
            });
        }
    }
}
