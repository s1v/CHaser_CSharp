using System.Net;
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

        ConnectSHaserServer();
    }

    /// <summary>
    /// CHaserサーバーに接続する
    /// </summary>
    private void ConnectSHaserServer()
    {
        //接続
        socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socketClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

        //クライアント情報送信
        SendToCHaserServer($"{name}\r\n");
    }

    /// <summary>
    /// CHaserサーバーへメッセージを送信する
    /// </summary>
    /// <param name="sendMessage">送信するメッセージ</param>
    private void SendToCHaserServer(string sendMessage)
    {
        socketClient.Send(Encoding.UTF8.GetBytes(sendMessage));
    }
}