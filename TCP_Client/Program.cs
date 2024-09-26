using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Client
{
    private const int Port = 8888; // създава константна целочислена променлива относно порта със стойност 8888
    private const string ServerIp = "127.0.0.1"; // създава константна променлива тип низ, съдържаща 4-те октети на примерен логически IP адрес

    static void Main()
    {
        TcpClient client = new TcpClient(ServerIp, Port); // създава обект client от клас TcpClient с двете променливи - IP aдрес и номер на порта
        Console.WriteLine("Connected to server. Start chatting!");

        NetworkStream stream = client.GetStream();

        Thread receiveThread = new Thread(ReceiveMessages);
        receiveThread.Start(stream);

        while (true)
        {
            string message = Console.ReadLine();
            byte[] buffer = Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }

    static void ReceiveMessages(object obj)
    {
        NetworkStream stream = (NetworkStream)obj;
        byte[] buffer = new byte[1024];
        int bytesRead; // инициализира целочислена променлива отностно прочетените байтове

        while (true)
        {
            try
            {
                bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    break;
                }

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
