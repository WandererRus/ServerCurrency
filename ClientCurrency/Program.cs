using System.Net.Sockets;
using System.Net;
using System.Text;
namespace ClientCurrency
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Добро пожаловать в приложение \"Курс валют\".");
                IPEndPoint serverPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 80);
                Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);

                clientSocket.Connect(serverPoint);

                while (true)
                {
                    if (clientSocket.Connected)
                    {
                        Console.WriteLine("Введите валюту для конвертации.");
                        Console.WriteLine("Валюта вводится двумя словами через пробел из списка доступных валют." +
                            " Первой указываем валюту которую хотим конвертировать.");
                        Console.WriteLine("1. EUR");
                        Console.WriteLine("2. USD");
                        Console.WriteLine("3. GBP");
                        string message = Console.ReadLine();
                        if (message != null && message != "")
                        {
                            if (message.Contains(' '))
                            {
                                string[] values = message.Split(" ");
                                values[0] = values[0].Trim();
                                values[1] = values[1].Trim();
                                clientSocket.Send(Encoding.UTF8.GetBytes(values[0] + " " + values[1] + " "));
                                byte[] buffer = new byte[1024];
                                clientSocket.Receive(buffer);
                                Console.WriteLine($"В {values[0]} содержится {Encoding.UTF8.GetString(buffer).TrimEnd()} {values[1]}");
                            }
                            else
                            {
                                Console.WriteLine("Программы не обнаружила пробела в запросе. Просим соблюдать инструкции.");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Программа не смогла установить соединение.");
                        clientSocket.Close();
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
        }
    }
}