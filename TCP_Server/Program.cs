using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Server
{
    private static readonly List<TcpClient> clients = new List<TcpClient>();//fiosdfhgioth
    private const int Port = 8888;

    static void Main()
    {
        TcpListener server = new TcpListener(IPAddress.Any, Port);
        server.Start();
        Console.WriteLine($"Server started on port {Port}");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient tcpClient = (TcpClient)obj;
        NetworkStream stream = tcpClient.GetStream();

        byte[] buffer = new byte[1024];
        int bytesRead;

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
                Console.WriteLine($"Received: {message}");

                BroadcastMessage(tcpClient, message);
            }
            catch (Exception)
            {
                break;
            }
        }

        clients.Remove(tcpClient);
        tcpClient.Close();
    }

    static void BroadcastMessage(TcpClient sender, string message)
    {
        byte[] broadcastBuffer = Encoding.ASCII.GetBytes(message);

        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                stream.Write(broadcastBuffer, 0, broadcastBuffer.Length);
            }
        }
    }
}
