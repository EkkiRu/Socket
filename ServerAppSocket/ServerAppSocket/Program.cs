using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ServerAppSocket
{
    class Program
    {
        static async void Main(string[] args)
        {
            // аддресс интранет сети, адрес локальный и адрес localhost
            // 1 тип вдреса узнаём через командную строку и команду ipconfig

            var ipAddress = IPAddress.Parse("127.0.0.1");

            //создыем сокет для ip сети с транспортным протоколом TCP, работающий через потоки (постоянные соединения)
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                //привязываем наш соккет к конечной точке для будущего прослушивания 
                socket.Bind(new IPEndPoint(ipAddress, 62227));
                //начинаем прослушивать конечную  точку на входяшщие соединение при этом может держать в ожидании 10 следующий соединений
                socket.Listen(10);

                Console.WriteLine("Сервер запущен...");
                //бесконечно принимаем входящие соединения и считываем из них данные
                while (true)
                {
                    Console.WriteLine("Ждем входящих соединений...");
                    var incomeSocket = await socket.AcceptAsync();
                    //в зависимости что мы хотим сделать будет меняться 1...<
                    var buffer = new byte[256];
                    //работает как read у Stream
                    do
                    {
                        SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                        socketAsyncEventArgs.SetBuffer(buffer);
                        incomeSocket.ReceiveAsync(socketAsyncEventArgs);
                        socketAsyncEventArgs.Dispose();

                        Console.WriteLine($"Новое сообщение: {Encoding.UTF8.GetString(buffer)}");
                    }
                    while (incomeSocket.Available > 0);
                    incomeSocket.Shutdown(SocketShutdown.Both);
                    incomeSocket.Close();
                    Console.WriteLine("Закрытие соединения...");
                    //>...2
                }

                //socket.Close();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }
    }
}
