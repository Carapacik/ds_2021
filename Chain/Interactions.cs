using System;
using System.Net.Sockets;

namespace Chain
{
    public static class Interactions
    {
        public static int ReceiveInt(this Socket socket)
        {
            var buf = new byte[sizeof(int)];
            socket.Receive(buf);
            return BitConverter.ToInt32(buf);
        }

        public static void SendInt(this Socket socket, int number)
        {
            socket.Send(BitConverter.GetBytes(number));
        }
    }
}