using System;
using System.Collections.Generic; // за колекции от информация
using System.Net; // за мрежовите услуги и протоколите 
using System.Net.Sockets; // за мрежовите услуги
using System.Text; // за кодирането и конвертирането
using System.Threading; // за управление на нишките

class Server
{    
    // създаване на статичен списък, съхраняващ TCP клиентите
    private static readonly List<TcpClient> clients = new List<TcpClient>();
    // създаване на константна целочислена променлива относно номера на порта за връзка със сървъра
    private const int Port = 8888;

    static void Main()
    {
        // създанане на TcpListener - слуша за връзки на всички IP адреси на порта
        TcpListener server = new TcpListener(IPAddress.Any, Port);
        // стартиране на сървъра
        server.Start();
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {   
            // приемане на връзка от клиент
            TcpClient client = server.AcceptTcpClient();
            // добавяне на клиент в списъка
            clients.Add(client);
            // създаване на нова нишка 
            Thread clientThread = new Thread(HandleClient);
            // стартиране на нишката и предаване на параметъра 
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        // преобразуване/кастване на обекта към TcpClient
        TcpClient tcpClient = (TcpClient)obj;
        // задаване на обекта stream за четене и/или запис на данни към клиента
        NetworkStream stream = tcpClient.GetStream();

        // създаване на буферен масив от байтове с дължина 1024
        byte[] buffer = new byte[1024];
        // инициализиране на целочислена променлива, съхраняваща броя прочетени байтове
        int bytesRead;

        while (true)
        {
            try
            {
                // четене на данни от клиента в буфера
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

                // преобразуване на байтовете в низ чрез ASCII кодиране
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");

                // изпрщане на съобщението до всички останали клиенти
                BroadcastMessage(tcpClient, message);
            }
            catch (Exception)
            {
                break;
            }
        }

        // премахване на клиента от списъка с клиенти 
        clients.Remove(tcpClient);
        // затваряне на връзката с клиента
        tcpClient.Close();
    }

    static void BroadcastMessage(TcpClient sender, string message)
    {
        // преобразуване на съобщението в масив от байтове чрез ASCII кодиране
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);

        // за всички клиенти в списъка
        foreach (TcpClient client in clients)
        {    
            // дали текущият е изпращач на съобщението
            if (client != sender)
            {
                // получаване на stream на клиента и изпращане на съобщението
                NetworkStream stream = client.GetStream();
                // записване на масива от байтове
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
    }
}
