using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerCurrency
{
    internal class Program
    {
        static List<Socket> clientSockets = new List<Socket>();
        static void Main(string[] args)
        {
            IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            serverSocket.Bind(serverPoint);
            serverSocket.Listen(1000);

            serverSocket.BeginAccept(AcceptConnectionCallback, serverSocket);


        }

        static void AcceptConnectionCallback(IAsyncResult result)
        {
            if (result.AsyncState != null)
            {
                byte[] data = new byte[1024];
                Socket server = (Socket)result.AsyncState;
                Socket client = server.EndAccept(result);
                clientSockets.Add(client);
                client.BeginReceive(data, 0, data.Length, SocketFlags.None, ClientReciveMessageCallback, new ClientMessage(client, data));
                server.BeginAccept(AcceptConnectionCallback, server);
            }
; }

        static void ClientReciveMessageCallback(IAsyncResult result)
        {
            if (result.AsyncState != null)
            {
                ClientMessage clientMessage = (ClientMessage)result.AsyncState;
                Socket client = clientMessage.GetClient();
                byte[] data = clientMessage.GetData();
                client.EndReceive(result);
                string message = Encoding.UTF8.GetString(data);

                /*отправка данных клиенту*/
                /*проверку на закрытие соединения*/
                client.BeginReceive(data, 0, data.Length, SocketFlags.None, ClientReciveMessageCallback, new ClientMessage(client, data));
            }
        }
    }
    class ClientMessage
    {
        Socket _client;
        byte[] _message = new byte[1024];
        public ClientMessage(Socket client, byte[] message)
        {
            _client = client;
            _message = message;
        }
        public Socket GetClient()
        {
            return _client;
        }
        public byte[] GetData()
        {
            return _message;
        }
    }

    class CurrencyList
    {
        double EUR = 82.3736;
        double USD = 77.2422;
        double GBP = 93.7720;

        public double USDToEUR()
        {
            return USD / EUR;
        }
        public double USDToGBP()
        {
            return USD / GBP;
        }
        public double GBPToUSD()
        {
            return GBP / USD;
        }
        public double GBPToEUR()
        {
            return GBP / EUR;
        }
        public double EURToGBP()
        {
            return EUR / GBP;
        }
        public double EURToUSD()
        {
            return EUR / USD;
        }
    }
}