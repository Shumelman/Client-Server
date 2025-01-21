using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace MultiThreadServer
{
    class ExampleTcpListener
    {
        static void Main(string [] args)
        {
            TcpListener server = null;
            try
            {
                int MaxThreadsCount = Environment.ProcessorCount * 4;
                //Установка максимального кол-тво рабочих потоков
                ThreadPool.SetMaxThreads(MaxThreadsCount, MaxThreadsCount);
                //минимальное кол-во рабочих потоков 
                ThreadPool.SetMinThreads(2, 2);

                //Установка порта для TcpListener = 9595
                Int32 port = 9595;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                int counter = 0;
                server = new TcpListener(localAddr, port);
                Console.WriteLine("Конфигурация многопоточного сервера: ");
                Console.WriteLine(" IP-адрес  :  127.0.0.1");
                Console.WriteLine(" Порт    : " + port.ToString());
                Console.WriteLine(" Потоки   : " + MaxThreadsCount.ToString());
                Console.WriteLine("\nСервер запущен\n");
                //Запус и начало прослушивания клиента 
                server.Start();
                //Принимаем клиента в бесконечном цикле 
                while (true)
                {
                    Console.Write("\nОжидание соединения... ");
                    ThreadPool.QueueUserWorkItem(ClientProcessing, server.AcceptTcpClient());
                    counter++;
                    Console.Write("\nСоединение номер" + counter.ToString() + "!");

                }
            }
            catch(SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                //Остановка сервера 
                server.Stop();
            }

            Console.WriteLine("\bНажмите Enter...");
            Console.Read();
        }

        static void ClientProcessing(object client_obj)
        {
            //буфер для принимаемых данных
            Byte[] bytes = new byte[256];
            String data = null;
            TcpClient client = client_obj as TcpClient;

            data = null;

            NetworkStream stream = client.GetStream();

            int i;
            //Принимаем данные от клиента в цикле, пока не дойдет до конца
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                data = data.ToUpper();

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                stream.Write(msg, 0, msg.Length);

            }

            client.Close();
        }
    }
}