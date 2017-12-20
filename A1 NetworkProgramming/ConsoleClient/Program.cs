using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleClient
{
    class Program
    {
        private static Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private const int dedicatedVarbytes = 5;
        private static byte clientID;
        private static byte Xloc;
        private static byte Yloc;

        static void Main(string[] args)
        {
            Console.Title = "ClientConsole";
            LoopConnect();
            SendLoop();
            Console.ReadLine();
        }
        private static void LoopConnect()
        {
            int attempts = 0;
            while (!_clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    _clientSocket.Connect(IPAddress.Loopback, 100);
                }
                catch (SocketException)
                {
                    Console.Clear();
                    Console.WriteLine("Connection attempts: " + attempts.ToString());
                }
            }
            Console.WriteLine("Connected");
            byte[] receivedBuf = new byte[1024];
            int rec = _clientSocket.Receive(receivedBuf);
            byte[] data = new byte[rec];
            Array.Copy(receivedBuf, data, rec);
            Console.WriteLine("Client number: " + data[0].ToString());
            clientID = data[0];
        }
        private static void SendLoop()
        {
            while (true)
            {

                string req;
                while (true)
                {
                    Console.Write("//");
                    req = Console.ReadLine();
                    if (req != string.Empty) break;
                }
                if (req == "x") { Xloc++; }
                if (req == "y") { Yloc++; }
                if (req == "-x") { Xloc--; }
                if (req == "-y") { Yloc--; }
                byte[] variableBytes = new byte[dedicatedVarbytes] { clientID, Xloc, Yloc, 0, 0 };
                byte[] buffer = Append(variableBytes , Encoding.ASCII.GetBytes(req));
                _clientSocket.Send(buffer);
                byte[] receivedBuf = new byte[1024];
                int rec = _clientSocket.Receive(receivedBuf);
                byte[] data = new byte[rec];
                Array.Copy(receivedBuf, data, rec);
                Console.WriteLine(Encoding.ASCII.GetString(data));
            }
        }

        private static byte[] Append(byte[] A, byte[] B)
        {
            byte[] O = new byte[A.Length + B.Length];
            Array.Copy(A, O, A.Length);
            for (int i = 0; i < B.Length; i++)
            {
                O[i + A.Length] = B[i];
            }
            return O;
        }
    }
}
