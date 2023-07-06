using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private string ip { get; init; }
    private int port { get; init; }
    private string name { get; init; }

    private Socket socketClient;

    public Client(string ip, int port, string name)
    {
        this.ip = ip;
        this.port = port;
        this.name = name;

        ConnectCHaserServer();
    }

    /// <summary>
    /// ターンのはじめに実⾏する。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public string GetReady() { return Order(OrderCode.GetReady); }

    /// <summary>
    /// 上に移動します。
    /// </summary>
    /// <returns></returns>
    public string WalkUp() { return Order(OrderCode.WalkUp); }

    /// <summary>
    /// 下に移動します。
    /// </summary>
    /// <returns></returns>
    public string WalkDown() { return Order(OrderCode.WalkDown); }

    /// <summary>
    /// 左に移動します。
    /// </summary>
    /// <returns></returns>
    public string WalkLeft() { return Order(OrderCode.WalkLeft); }

    /// <summary>
    /// 右に移動します。
    /// </summary>
    /// <returns></returns>
    public string WalkRight() { return Order(OrderCode.WalkRight); }

    /// <summary>
    /// 正⽅形状に上9マスの情報を取得します。
    /// </summary>
    /// <returns>上9マスの情報(正⽅形状)</returns>
    public string LookUp() { return Order(OrderCode.LookUp); }

    /// <summary>
    /// 正⽅形状に下9マスの情報を取得します。
    /// </summary>
    /// <returns>下9マスの情報(正⽅形状)</returns>
    public string LookDown() { return Order(OrderCode.LookDown); }

    /// <summary>
    /// 正⽅形状に左9マスの情報を取得します。
    /// </summary>
    /// <returns>左9マスの情報(正⽅形状)</returns>
    public string LookLeft() { return Order(OrderCode.LookLeft); }

    /// <summary>
    /// 正⽅形状に右9マスの情報を取得します。
    /// </summary>
    /// <returns>右9マスの情報(正⽅形状)</returns>
    public string LookRight() { return Order(OrderCode.LookRight); }

    /// <summary>
    /// 直線状に上9マスの情報を取得します。
    /// </summary>
    /// <returns>上9マスの情報(直線状)</returns>
    public string SearchUp() { return Order(OrderCode.SearchUp); }

    /// <summary>
    /// 直線状に下9マスの情報を取得します。
    /// </summary>
    /// <returns>下9マスの情報(直線状)</returns>
    public string SearchDown() { return Order(OrderCode.SearchDown); }

    /// <summary>
    /// 直線状に左9マスの情報を取得します。
    /// </summary>
    /// <returns>左9マスの情報(直線状)</returns>
    public string SearchLeft() { return Order(OrderCode.SearchLeft); }

    /// <summary>
    /// 直線状に右9マスの情報を取得します。
    /// </summary>
    /// <returns>右9マスの情報(直線状)</returns>
    public string SearchRight() { return Order(OrderCode.SearchRight); }

    /// <summary>
    /// 上にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public string PutUp() { return Order(OrderCode.PutUp); }

    /// <summary>
    /// 下にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public string PutDown() { return Order(OrderCode.PutDown); }

    /// <summary>
    /// 左にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public string PutLeft() { return Order(OrderCode.PutLeft); }

    /// <summary>
    /// 右にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public string PutRight() { return Order(OrderCode.PutRight); }

    /// <summary>
    /// CHaserサーバーに接続する
    /// </summary>
    private void ConnectCHaserServer()
    {
        try
        {
            //接続
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

            //クライアント情報送信
            Send($"{name}\r\n");

            Console.WriteLine("Connected.");
        }
        catch (SocketException e)
        {
            //接続失敗時
            Console.WriteLine("Failed to connect CHaser server.\n(Press \"Enter\" to retry)");
            Console.ReadLine();
            ConnectCHaserServer();
        }
    }

    /// <summary>
    /// CHaserサーバーへ文字列を送信する
    /// </summary>
    /// <param name="sendString">送信する文字列</param>
    private void Send(string sendString)
    {
        socketClient.Send(Encoding.UTF8.GetBytes(sendString));
    }

    /// <summary>
    /// CHaserサーバーから文字列を受信する
    /// </summary>
    /// <returns>受信した文字列</returns>
    private string Receive()
    {
        byte[] data = new byte[4096];
        socketClient.Receive(data, data.Length, SocketFlags.None);
        Array.Reverse(data);
        return Encoding.UTF8.GetString(data.Take(11).ToArray());
    }

    private string Order(string orderString)
    {
        if (orderString == OrderCode.GetReady)
        {
            //接続確認
            if (!Receive().Contains(ControlCode.TurnStart))
            {
                //ダメだった場合
                Console.WriteLine("Connection failed.");
            }
        }

        Send($"{orderString}\r\n");
        string response = Receive();

        if (orderString != OrderCode.GetReady)
        {
            Send($"{ControlCode.TurnEnd}\r\n");
        }

        switch (response[0])
        {
            case GameStatus.Progress:
                return response.Substring(1, 10);

            case GameStatus.Finished:
                throw new Exception("Game Set!!");

            default:
                throw new Exception("Response Error.");
        }
    }
}