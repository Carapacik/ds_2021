using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    internal static class Program
    {
        private static void StartListening(int port)
        {
            var history = new List<string>();
            var ipAddress = IPAddress.Any;
            var localEndPoint = new IPEndPoint(ipAddress, port);
            var listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                listener.Bind(localEndPoint);
                listener.Listen(10);
                while (true)
                {
                    var handler = listener.Accept();

                    var buf = new byte[2048];
                    var bytesRec = handler.Receive(buf);
                    var data = Encoding.UTF8.GetString(buf, 0, bytesRec);
                    history.Add(data);
                    Console.WriteLine($"Message received: {data}");

                    var jsonMsg = JsonSerializer.Serialize(history);
                    var msg = Encoding.UTF8.GetBytes(jsonMsg);
                    handler.Send(msg);

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length != 1) throw new Exception("Invalid count of arguments");
            StartListening(int.Parse(args[0]));
        }
    }
}