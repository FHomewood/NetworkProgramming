using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1
{
    class Program
    {
        private static byte[] _buffer = new byte[1024];
        private static byte clientNo = 0;
        private const int dedicatedVarbytes = 5;
        private static List<Client> _clientSockets = new List<Client>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        static void Main(string[] args)
        {
            Console.Title = "Server";
            SetupServer();
            Console.Read();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up server...");
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            _serverSocket.Listen(1);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private static void AcceptCallback(IAsyncResult AR)
        {
            Socket socket = _serverSocket.EndAccept(AR);
            _clientSockets.Add(new Client(socket, clientNo, "Client " + clientNo.ToString(), 0, 0));
            Console.WriteLine("Client {0} Connected", clientNo);
            byte[] data = new byte[] { (byte)clientNo };
            clientNo++;
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private static void ReceiveCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            int received = 0;
            received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(_buffer, dataBuf, received);
            string text = Encoding.ASCII.GetString(SubArray(dataBuf, 5, dataBuf.Length));
            foreach (Client client in _clientSockets)
            {
                if (dataBuf[0] == client.ID)
                {
                    Console.WriteLine("{0}: {1}", client.name, text);
                    client.xLoc = dataBuf[1];
                    client.yLoc = dataBuf[2];
                }
            }
            string response = "invalid request";
            //command list//
            if (text.ToLower() == "get time")
            {
                response = DateTime.Now.ToLongDateString();
            }
            if (text.Length > 14 && text.ToLower().Substring(0,15) == "set clientname ")
            {
                foreach (Client client in _clientSockets)
                {
                    if (dataBuf[0] == client.ID)
                    {
                        client.name = text.Substring(15);
                    }
                }
                response = "Client name set to " + text.Substring(15);
            }
            foreach (Client client in _clientSockets)
            {
                response += "\r\n" + client.name + " is at (" + client.xLoc.ToString() + "," + client.yLoc.ToString() + ")";
            }
            //send response//
            byte[] data = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
        }
        private static void SendCallback(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }

        private static byte[] SubArray(byte[] array, int startVal, int endval)
        {
            byte[] O = new byte[endval - startVal];
            for (int i = startVal; i < endval; i++)
            {
                O[i - startVal] = array[i];
            }


            return O;
        }
    }
}
