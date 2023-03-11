using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MineFramework
{
    class MineSocket : AsynchronousSocketListener
    {
        // Client Socket
        public Socket clSocket = null;

        // Size of Receive Buffer
        public const int BufferSize = 1024;

        // ReceiveBuffer
        public byte[] buffer = new byte[BufferSize];

        // Receive Data String
        public StringBuilder sb = new StringBuilder();

        public int MineSocketStart()
        {
            StartListening();
            return 0;
        }
    }
}
