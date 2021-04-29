using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chain
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length is not (4 or 3))
                throw new ArgumentException("Invalid arguments count");
            
            var listeningPort = int.Parse(args[0]);
            var nextHost = args[1];
            var nextPort = int.Parse(args[2]);
            var isInitiator = args.Length == 4 && args[3] == "true";
            try
            {
                Start(listeningPort, nextHost, nextPort, isInitiator);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.ReadKey();
            }
        }

        private static void Start(int listeningPort, string nextHost, int nextPort, bool isInitiator)
        {
            var listenIpAddress = IPAddress.Any;
            var localEp = new IPEndPoint(listenIpAddress, listeningPort);
            var listener = new Socket(
                listenIpAddress.AddressFamily, 
                SocketType.Stream, 
                ProtocolType.Tcp);
            
            var nextIpAddress = nextHost == "localhost" ? IPAddress.Loopback : IPAddress.Parse(nextHost);
            var sender = new Socket(
                nextIpAddress.AddressFamily, 
                SocketType.Stream, 
                ProtocolType.Tcp);
            
            listener.Bind(localEp);
            listener.Listen(10);
            
            var x = int.Parse(Console.ReadLine() ?? throw new InvalidOperationException("Wrong input"));
            var remoteEp = new IPEndPoint(nextIpAddress, nextPort);
            ConnectSender(sender, remoteEp);
            var receiver = listener.Accept();
            if (isInitiator)
            {
                sender.SendInt(x);
                x = receiver.ReceiveInt();
                Console.WriteLine(x);
                sender.SendInt(x);
            }
            else
            {
                var y = receiver.ReceiveInt();
                sender.SendInt(Math.Max(x, y));
                y = receiver.ReceiveInt();
                Console.WriteLine(y);
                sender.SendInt(y);
            }

            receiver.Shutdown(SocketShutdown.Both);
            receiver.Close();
            
            sender.Shutdown(SocketShutdown.Both);
            sender.Close();
        }

        private static void ConnectSender(Socket socket, EndPoint endPoint)
        {
            while (true)
                try
                {
                    socket.Connect(endPoint);
                    break;
                }
                catch
                {
                    Thread.Sleep(1000);
                }
        }
    }
}