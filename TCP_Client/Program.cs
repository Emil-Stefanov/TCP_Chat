using System;
using System.Net.Sockets; // за мрежовите услуги
using System.Text; // за кодирането и конвертирането
using System.Threading; // за управление на нишките

class Client
{
    // създаване на константна целочислена променлива относно номера на порта за връзка със сървъра
    private const int Port = 8888;
    // създаване на константна променлива тип низ за IP адрес на сървъра
    private const string ServerIp = "127.0.0.1"; 

    static void Main()
    {
        // създаване на  обект client от клас TcpClient с двете променливи - IP aдрес на сървъра и номер на порта
        TcpClient client = new TcpClient(ServerIp, Port);
        Console.WriteLine("Connected to server. Start chatting!");

        // задаване на обекта stream за четене и/или запис на данни към и от сървъра
        NetworkStream stream = client.GetStream();

        // създаване на нова нишка за получавнане на съобщения от сървъра 
        Thread receiveThread = new Thread(ReceiveMessages);
        // предаване на потока към нишката за получаване на съобщения 
        receiveThread.Start(stream);

        while (true)
        {
            string message = Console.ReadLine();
            // преобразуване на съобщението в масив от байтове чрез ASCII кодиране
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            // записване на масива от байтове в потока към сървъра
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    // дефиниране на метод за получаване на съобщения от сървъра
    static void ReceiveMessages(object obj)
    {
        // преобразуване/кастване  на обекта към тип NetwotkStream
        NetworkStream stream = (NetworkStream)obj;
        // създаване на буферен масив от байтове с дължина 1024
        byte[] buffer = new byte[1024];
        // инициализиране на целочислена променлива, съхраняваща броя прочетени байтове
        int bytesRead; 

        // прави проверки за наличието на изключения (exceptions)
        while (true)
        {
            try
            {
                //четене на данни в буфера
                bytesRead = stream.Read(buffer, 0, buffer.Length); 
                if (bytesRead == 0)
                {
                    break;
                }
                
                // преобразуване на байтовете в низ чрез ASCII кодиране
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Console.WriteLine(message);
            }
            catch (Exception)
            {
                break;
            }
        }
    }
}
