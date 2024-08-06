using System.Net;
using System.Net.Sockets;
using System.Text;

class Client
{

    static Socket mySocket;
    static async Task Main(string[] args)
    {
        await InitSocket();
        _ = ReceiveMessagesAsync();
            while (true)
            {
                string message = Console.ReadLine();
                _ = SendMessageAsync(message);
            }
    }

    private static async Task InitSocket()
    {
        // יצירת אובייקט סוקט ללקוח
        mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // הגדרת כתובת IP ופורט של השרת
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 12345);

        // חיבור לשרת
        await Task.Factory.FromAsync(mySocket.BeginConnect, mySocket.EndConnect, serverEndPoint, null);
        Console.WriteLine("Connected to server.");
    }

    private static async Task SendMessageAsync(string message)
    {
        byte[] messageBytes = Encoding.ASCII.GetBytes(message);
        await Task.Factory.FromAsync(
            mySocket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, null, mySocket),
            mySocket.EndSend);

    }

    static async Task ReceiveMessagesAsync()
    {
        while (true)
        {
            byte[] buffer = new byte[1024];

            int bytesRead = await GetMessage(mySocket, buffer);

            if (bytesRead == 0) break; // השרת סגר את החיבור

            string receivedMessage = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine("Received: " + receivedMessage);
        }

        mySocket.Shutdown(SocketShutdown.Both);
        mySocket.Close();
    }

    private static Task<int> GetMessage(Socket mySocket, byte[] buffer)
    {
        return Task.Factory.FromAsync<int>(
                mySocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, null, mySocket),
                mySocket.EndReceive);
    }
}