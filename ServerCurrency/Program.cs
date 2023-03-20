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
                string[] currencys = message.Split(' ');
                string answer = CurrencyList.GetValue(currencys[0], currencys[1]).ToString();
                byte[] senddata = Encoding.UTF8.GetBytes(answer);
                client.BeginSend(senddata, 0, senddata.Length, SocketFlags.None, ClientSendMessageCallback, client);
                /*проверку на закрытие соединения*/
                client.BeginReceive(data, 0, data.Length, SocketFlags.None, ClientReciveMessageCallback, new ClientMessage(client, data));
            }
        }

        static void ClientSendMessageCallback(IAsyncResult result)
        {
            if (result.AsyncState != null) 
            {
                Socket client = (Socket)result.AsyncState;
                client.EndSend(result);
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

    static class CurrencyList
    {
        static double EUR = 82.3736;
        static double USD = 77.2422;
        static double GBP = 93.7720;

        static public double GetValue(string first, string second)
        {
            double result = 0;
            switch (first) 
            {
                case "EUR": if (second == "USD") { result = EURToUSD(); } else { result = EURToGBP(); } break;
                case "USD": if (second == "EUR") { result = USDToEUR(); } else { result = USDToGBP(); } break;
                case "GBP": if (second == "EUR") { result = GBPToEUR(); } else { result = GBPToUSD(); } break;
            }
            return result;
        }

        static double USDToEUR()
        {
            return USD / EUR;
        }
        static double USDToGBP()
        {
            return USD / GBP;
        }
        static double GBPToUSD()
        {
            return GBP / USD;
        }
        static double GBPToEUR()
        {
            return GBP / EUR;
        }
        static double EURToGBP()
        {
            return EUR / GBP;
        }
        static double EURToUSD()
        {
            return EUR / USD;
        }
    }
}