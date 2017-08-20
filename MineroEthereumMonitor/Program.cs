using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Quobject.Collections.Immutable;
using Quobject.EngineIoClientDotNet.Client.Transports;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Diagnostics;
using System.Threading;

namespace MineroEthereumMonitor
{
    class Program
    {

        private static string urlServidor = string.Empty;
        private static Minero minero;
        private static string rutaInicio = AppDomain.CurrentDomain.BaseDirectory;
 
        static void Main(string[] args)
        {
          
            Console.ForegroundColor = ConsoleColor.Green;
            string help = "Parametros Requqeridos: " + Environment.NewLine + Environment.NewLine + "\t -s <URL SERVIDOR>" + Environment.NewLine + "\t -n <NOMBRE MINERO>" + Environment.NewLine + Environment.NewLine + "Ejemplo : " + Environment.NewLine + Environment.NewLine + "\t MineroEthereumMonitor.exe -s http://localhost:8000 -n Minero1" + Environment.NewLine + "\t MineroEthereumMonitor.exe -s http://192.168.1.12:8080 -n Minero7";

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
        static Socket socket;       
        private static void iniciarSocket()
        {
            string bar = "############################################################################################################" + Environment.NewLine;

            IO.Options options = new IO.Options();
            options.Transports = ImmutableList.Create(new string[] { WebSocket.NAME, Polling.NAME });
            socket = IO.Socket(urlServidor, options);
            socket.On(Socket.EVENT_CONNECT, () =>
            {               
                    Console.WriteLine(bar);
                    Console.WriteLine("Conexion establecida con el servidor => " + urlServidor);
                    Console.WriteLine("Sincronizando Información con el servidor...");
                    socket.Emit("inicializarMinero", JObject.Parse(JsonConvert.SerializeObject(minero)));               

            });

            socket.On(Socket.EVENT_CONNECT_ERROR, () =>
            {
                Console.WriteLine("ERROR CONECTION");
            });

            socket.On(Socket.EVENT_CONNECT_TIMEOUT, () =>
            {
                Console.WriteLine("TIMEOUT CONECTION");
            });

            socket.On("ConfiguracionMinero", (data) =>
            {
                if (!minero.minando)
                {
                    minero = JsonConvert.DeserializeObject<Minero>(data.ToString());
                    Console.WriteLine("Configuración de Minado actualizada" + Environment.NewLine);
                    Console.WriteLine(bar);
                    Console.WriteLine("CONFIGURACIÓN :");
                    Console.WriteLine("PoolSever 1 => " + minero.poolServer1);
                    Console.WriteLine("Puerto PoolSever 1 => " + minero.portPoolServer1);
                    Console.WriteLine("PoolSever 2 => " + minero.poolServer2);
                    Console.WriteLine("Puerto PoolSever 2 => " + minero.portPoolServer2);
                    Console.WriteLine("Intervalo de Reporte al Panel de Administracion => " + minero.timeout + " ms" + Environment.NewLine);
                    Console.WriteLine(bar);

                    minar();
                }
            });

            socket.On(Socket.EVENT_DISCONNECT, () => {
                Console.WriteLine("PETO EL SOCKET");
             
            });

            socket.On(Socket.EVENT_RECONNECTING, () => {
                Console.WriteLine("PETO EL SOCKET");
            
            });
            socket.On(Socket.EVENT_RECONNECT, () => {
                Console.WriteLine("PETO EL SOCKET");

            });

            socket.On(Socket.EVENT_RECONNECT_ATTEMPT, () => {
                Console.WriteLine("PETO EL SOCKET");

            });
            socket.On(Socket.EVENT_RECONNECT_ERROR, () => {
                Console.WriteLine("PETO EL SOCKET");

            });
            socket.On(Socket.EVENT_RECONNECT_FAILED, () => {
                Console.WriteLine("PETO EL SOCKET");
                iniciarSocket();

            });
            socket.On(Socket.EVENT_ERROR, () => {
                Console.WriteLine("PETO EL SOCKET");

            });
        }



        private static void minar()
        {
            matarProcesoMinado();
            Process process = new Process();
            process.StartInfo.FileName = (string)string.Concat(rutaInicio, @"/ethminer.exe");
            process.StartInfo.Arguments = (string)string.Concat(" --farm-recheck ", minero.timeout, " -G -S ", minero.poolServer1, ":", minero.portPoolServer1, " -FS ", minero.portPoolServer2, ":", minero.portPoolServer2, " -O ", minero.account, ".", minero.nombre);
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            process.Start();
            minero.minando = true;
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

        }

        private static void matarProcesoMinado()
        {
            foreach (var process in Process.GetProcessesByName("ethminer"))
            {
                process.Kill();
            }
        }

        private static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            try
            {
                Console.WriteLine(outLine.Data);
                if (outLine.Data.Contains("MH/s"))
                {
                    minero.hashrate = outLine.Data.Split(':')[3].Split('M')[0].Trim();                   
                    socket.Emit("ReporteHashrate", JObject.Parse(JsonConvert.SerializeObject(minero)));
                }
            }
            catch (Exception ex)
            {
            }
        }

    }
}
