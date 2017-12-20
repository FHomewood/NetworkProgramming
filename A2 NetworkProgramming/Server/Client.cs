using System.Net.Sockets;

namespace Server
{
    internal class Client
    {
        public Socket socket;
        public byte ID;
        public byte xLoc;
        public byte yLoc;

        public Client(Socket socket, byte ID, byte xLoc, byte yLoc)
        {
            this.socket = socket;
            this.ID = ID;
            this.xLoc = xLoc;
            this.yLoc = yLoc;
        }
    }
}