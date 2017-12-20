using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp1
{
    class Client
    {
        public Socket socket;
        public byte ID;
        public string name;
        public byte xLoc;
        public byte yLoc;

        public Client(Socket socket, byte ID, string name, byte xLoc, byte yLoc)
        {
            this.socket = socket;
            this.ID     = ID;
            this.name   = name;
            this.xLoc   = xLoc;
            this.yLoc   = yLoc;
        }
        public byte[] ToBytes()
        {
            return new byte[] {
                ID,
                xLoc,
                yLoc
            };
        }
        public void ReceiveUpdate(byte[] data)
        {
            if (data[0] == ID)
            {
                xLoc = data[1];
                yLoc = data[2];
            }
        }
    }
}
