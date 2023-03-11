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
    public class AsynchronousSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public AsynchronousSocketListener()
        {
        }

        #region 비동기 소켓서버를 시작합니다.
        public static void StartListening(string _sockip, int _sockport)
        {
            // IP정보
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            // IP주소중 첫번째 주소
            IPAddress ipaddr = ipHostInfo.AddressList[0];

            if (_sockport == 0)
            {
                // 기본포트(11000) 셋팅
                _sockport = 11000;
            }

            IPEndPoint ipendp;

            if (_sockip != "")
            {
                // IP주소의 포트매핑하여 Endpoint선언
                ipendp = new IPEndPoint(IPAddress.Parse(_sockip), _sockport);
            }
            else
            {
                // IP주소의 포트매핑하여 Endpoint선언
                ipendp = new IPEndPoint(ipaddr, _sockport);
            }

            // Create TCP/IP Socket
            Socket listener = new Socket(ipaddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // 소켓 바인딩 및 대기
                listener.Bind(ipendp);
                listener.Listen(100);

                while (true)
                {
                    // 대기 쓰레드 초기화
                    allDone.Reset();

                    // Start an asynchronous socket to listen for connection
                    Console.WriteLine("waiting for a connection...");

                    // 비동기 작업을 시작함. 
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    // 요청시까지 쓰레드 대기
                    allDone.WaitOne();
                }

            }
            catch(Exception e)
            {
                Console.WriteLine("Socket Server Listening Error : " + e.ToString());
            }

            Console.WriteLine("\nPress Enter Continue...");
            Console.Read();
        }
        #endregion

        #region 소켓 쓰레드에 대한 비동기 Callback
        public static void AcceptCallback(IAsyncResult ar)
        {
            // 대기쓰레드 재실행
            allDone.Set();

            // 비동기 Callback의 사용자정의 다시 소켓을 정의함.
            Socket listener = (Socket)ar.AsyncState;
            // 들어오는 연결시도를 모두 비동기적으로 받아들이고 원격호스트통신을 진행할 새로운 소켓을 생성
            Socket handler = listener.EndAccept(ar);

            // Client소켓과의 연결을 위한 설정을 연결한다.
            MineSocket state = new MineSocket();

            // Client소켓에 handler소켓을 연결한다.
            state.clSocket = handler;

            // 연결된 소켓에서 데이터를 비동기적으로 수신하기 시작합니다.
            handler.BeginReceive(state.buffer, 0, MineSocket.BufferSize, 0, new AsyncCallback(ReadCallback), state);
        }
        #endregion

        #region 비동기적으로 들어온 데이터의 처리를 Callback합니다.
        public static void ReadCallback(IAsyncResult ar)
        {
            // 내용을 담을 변수
            String content = String.Empty;

            // 비동기상태의 객체에서 상태객체를 연결합니다.
            MineSocket state = (MineSocket)ar.AsyncState;
            Socket handler = state.clSocket;

            // Client소켓에서 데이터를 읽습니다.
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // 들어온데이터마다 모두 StringBuilder에 저장합니다.
                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                content = state.sb.ToString();
                if (content.IndexOf("<EOF>") > -1)
                {
                    // All the data has been read from the client. display it on then console.
                    Console.WriteLine("Read {0} bytes from socket. \n Data : {1}", content.Length, content);
                    SendData(handler, content);
                }
                else
                {
                    handler.BeginReceive(state.buffer, 0, MineSocket.BufferSize, 0, new AsyncCallback(ReadCallback), state);
                }
            }
        }
        #endregion

        private static void SendData(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] bytedata = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device
            handler.BeginSend(bytedata, 0, bytedata.Length, 0, new AsyncCallback(SendCallback), handler);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retreive the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int byteSent = handler.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to client", byteSent);

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine("SendCallBack Error : " + e.ToString());
            }
        }
    }
}
