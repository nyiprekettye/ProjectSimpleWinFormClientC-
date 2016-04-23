using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OwnFormClient
{
    public class Connect
    {
        public Socket client = null;
        public IPEndPoint endPoint;
        private string ip = "127.0.0.1";
        private int port = 1234;

        private const int BUFFER_SIZE = 4096;
        private static byte[] buffer = new byte[BUFFER_SIZE]; //buffer size is limited to BUFFER_SIZE per message

        // The response from the remote device.
        private static String response = String.Empty;

        public class StateObject
        {
            // Client socket.
            public Socket workSocket = null;
            // Size of receive buffer.
            public const int BufferSize = 256;
            // Receive buffer.
            public byte[] buffer = new byte[BufferSize];
            // Received data string.
            public StringBuilder sb = new StringBuilder();
        }

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
                new ManualResetEvent(false);     
        private static ManualResetEvent sendDone =
               new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
            new ManualResetEvent(false);

        public Connect()
        {
            client = new Socket(
             AddressFamily.InterNetwork,
             SocketType.Stream,
             ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse(ip), port);            
        }

        internal void Disconnect()
        {
            if (client.Connected)
            {
                //client.Disconnect(true);
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Console.WriteLine("Lecsatlakoztam a szerverről!");
            }
           
        }

        public void Init(string ip, string port)
        {
            this.ip = ip;
            this.port = Convert.ToInt16(port);
            // Create a TCP/IP socket. 
            client = new Socket(
             AddressFamily.InterNetwork,
             SocketType.Stream,
             ProtocolType.Tcp);
            endPoint = new IPEndPoint(IPAddress.Parse(this.ip), this.port);           
        }

        internal void Start()
        {
            Console.WriteLine("Kezdem a kapcsolódást!");
            StartClient();
        }

        private void StartClient()
        {
            // Connect to a remote device.
            try
            {
                // Establish the remote endpoint for the socket.
                // The name of the 
                // remote device is "host.contoso.com".
                // IPHostEntry ipHostInfo = Dns.Resolve("host.contoso.com");
                //IPAddress ipAddress = "127.0.0.1";
                //IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 1234);

                // Create a TCP/IP socket.
                //client = new Socket(AddressFamily.InterNetwork,
                //   SocketType.Stream, ProtocolType.Tcp);

                // Connect to the remote endpoint.                
                client.BeginConnect(endPoint,
                    new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();




                // Send test data to the remote device.
                // StartSend("ez egy tesztüzenet<EOF>");

                // Receive the response from the remote device.
                //  String getLetter=StarReceive();
                //  receiveDone.Reset();
                // Receive(client);
                // receiveDone.WaitOne();
                // Write the response to the console.
                //Console.WriteLine("A szervertől kapott válasz: {0}", response);
                /*
                // Release the socket.
                client.Shutdown(SocketShutdown.Both);
                client.Close();
                Console.ReadKey();*/
             

                 client.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), client);
           
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        public void StartSend(string s)
        {
            // Send test data to the remote device.
            sendDone.Reset();
            Send(client, s);
            sendDone.WaitOne();
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("A kliens kapcsolódott a következő címre {0}",
                    client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void receiveCallback(IAsyncResult result)
        {
            System.Net.Sockets.Socket socket = null;
            try
            {
                socket = (System.Net.Sockets.Socket)result.AsyncState;
                if (socket.Connected)
                {
                    int received = socket.EndReceive(result);
                    if (received > 0)
                    {
                        
                        byte[] data = new byte[received];
                        Buffer.BlockCopy(buffer, 0, data, 0, data.Length); //copy the data from your buffer
                                                                           //DO ANYTHING THAT YOU WANT WITH data, IT IS THE RECEIVED PACKET!
                                                                           //Notice that your data is not string! It is actually byte[]
                                                                           //For now I will just print it out
                        Console.WriteLine("Server Üzenete: " + Encoding.UTF8.GetString(data));
                        socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), socket);
                    }                    
                    else
                    { //completely fails!
                        Console.WriteLine("receiveCallback is failed!");                        
                        socket.Close();
                    }
                }
            }
            catch (Exception e)
            { // this exception will happen when "this" is be disposed...
                Console.WriteLine("receiveCallback is failed! " + e.ToString());
            }
        }      

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.UTF8.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}
