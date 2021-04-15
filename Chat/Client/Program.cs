using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    internal static class Program
    {
        private static void StartClient(string ip, int port, string message)
        {
            try
            {
                var ipAddress = ip == "localhost" ? IPAddress.Loopback : IPAddress.Parse(ip);
                var remoteEp = new IPEndPoint(ipAddress, port);
                var sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream,
                    ProtocolType.Tcp);

                try
                {
                    sender.Connect(remoteEp);
                    Console.WriteLine("Remote socket address: {0}",
                        sender.RemoteEndPoint);

                    sender.Send(Encoding.UTF8.GetBytes(message));

                    var buf = new byte[1024];
                    var bytesRec = sender.Receive(buf);
                    var data = Encoding.UTF8.GetString(buf, 0, bytesRec);

                    var history = JsonSerializer.Deserialize<List<string>>(data);
                    if (history != null)
                        foreach (var msg in history)
                            Console.WriteLine(msg);

                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane);
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Main(string[] args)
        {
            if (args.Length != 3) throw new Exception("Invalid count of arguments");
            StartClient(args[0], int.Parse(args[1]), args[2]);
        }
    }
}