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
    public class MineSocket : AsynchronousSocketListener
    {
        // Server Socket
        public Socket clSocket = null;

        // Server Socket IP
        public string _sockip = string.Empty;

        // Server Socket Port
        public int _sockport = 0;

        // Size of Receive Buffer
        public const int BufferSize = 1024;

        // ReceiveBuffer
        public byte[] buffer = new byte[BufferSize];

        // Receive Data String
        public StringBuilder sb = new StringBuilder();

        public int MineSocketStart()
        {
            StartListening(_sockip, _sockport);
            return 0;
        }

        public string sockip
        {
            get { return _sockip; }
            set { _sockip = value; }
        }

        public int sockport
        {
            get { return _sockport; }
            set { _sockport = value; }
        }
    }
}
