using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    class Program
    {
        //  Socket Variables  //
        private static byte[] _buffer = new byte[1024];
        private static byte clientNo = 0;
        private const int recConstBytes = 5;
        private const int senConstBytes = 0;
        private const int bytesPerClient = 3;
        private static List<Client> _clientSockets = new List<Client>();
        private static Socket _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //  Socket Variables  //

        //  Server Variables  //

        //  Server Variables  //

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
            _clientSockets.Add(new Client(socket, clientNo, 0, 0));
            Console.WriteLine("Client {0} Connected", clientNo);
            byte[] data = new byte[] { (byte)clientNo };
            clientNo++;
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallback), socket);
            socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), socket);
            _serverSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }
        private static void ReceiveCallback(IAsyncResult AR)
        {
            //  Receive Client Buffer  //
            Socket socket = (Socket)AR.AsyncState;
            int received = 0;
            received = socket.EndReceive(AR);
            byte[] dataBuf = new byte[received];
            Array.Copy(_buffer, dataBuf, received);

            //--------------------------//
            //  Manage Received Buffer  //
            //--------------------------//


            foreach (Client client in _clientSockets)
            {
                if (dataBuf[0] == client.ID)
                {
                    client.xLoc = dataBuf[1];
                    client.yLoc = dataBuf[2];
                }
            }
            //-------------------------//
            //  Construct Send Buffer  //
            //-------------------------//
            byte[] data = new byte[bytesPerClient * clientNo + senConstBytes];
            //  Construct ConstBytes Here  //

            //  Construct ClientBytes Here  //
            foreach (Client client in _clientSockets)
            {
                data[bytesPerClient * client.ID + senConstBytes] = client.ID;
                data[bytesPerClient * client.ID + senConstBytes + 1] = client.xLoc;
                data[bytesPerClient * client.ID + senConstBytes + 2] = client.yLoc;
            }
            //  Send Buffer  //
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
