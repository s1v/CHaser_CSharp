using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;

public class Client
{
    public string ip { get; init; }
    public int port { get; init; }
    public string name { get; init; }

    private Socket socketClient;

    public Client(string ip, int port, string name)
    {
        this.ip = ip;
        this.port = port;
        this.name = name;

        ConnectCHaserServer();
    }

    /// <summary>
    /// CHaserサーバーに接続する
    /// </summary>
    private void ConnectCHaserServer()
    {
        //接続
        socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socketClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

        //クライアント情報送信
        Send($"{name}\r\n");
    }

    /// <summary>
    /// CHaserサーバーへ文字列を送信する
    /// </summary>
    /// <param name="sendString">送信する文字列</param>
    private void Send(string sendString)
    {
        socketClient.Send(Encoding.UTF8.GetBytes(sendString));
    }

    private string Receive()
    {
        byte[] data = new byte[4096];
        socketClient.Receive(data, data.Length, SocketFlags.None);
        Array.Reverse(data);
        return Encoding.UTF8.GetString(data.Take(11).ToArray());
    }

    private void Order(string orderString, bool isReady)
    {
        if (isReady)
        {
            //接続確認
            if (!Receive().Contains("@"))
            {
                //ダメだった場合
                Console.WriteLine("Connection Failed.");
            }

            Send($"{orderString}\r\n");
            Receive();
        }
    }
}