using System.Net;
using System.Net.Sockets;
using System.Text;

public class Client
{
    private string ip { get; init; }
    private int port { get; init; }
    private string name { get; init; }

    private Socket socketClient;
    private int currentTurn = 1;

    public Client(string ip, int port, string name)
    {
        this.ip = ip;
        this.port = port;
        this.name = name;

        Connect(); //サーバー接続
    }

    /// <summary>
    /// 周囲の情報を取得します。必ずターンのはじめに実⾏必要があります。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public FieldObject[] GetReady() => Order(OrderName.GetReady);

    /// <summary>
    /// 上に移動します。
    /// </summary>
    /// <returns></returns>
    public FieldObject[] WalkUp() => Order(OrderName.WalkUp);

    /// <summary>
    /// 下に移動します。
    /// </summary>
    /// <returns></returns>
    public FieldObject[] WalkDown() => Order(OrderName.WalkDown);

    /// <summary>
    /// 左に移動します。
    /// </summary>
    /// <returns></returns>
    public FieldObject[] WalkLeft() => Order(OrderName.WalkLeft);

    /// <summary>
    /// 右に移動します。
    /// </summary>
    /// <returns></returns>
    public FieldObject[] WalkRight() => Order(OrderName.WalkRight);

    /// <summary>
    /// 正⽅形状に上9マスの情報を取得します。
    /// </summary>
    /// <returns>上9マスの情報(正⽅形状)</returns>
    public FieldObject[] LookUp() => Order(OrderName.LookUp);

    /// <summary>
    /// 正⽅形状に下9マスの情報を取得します。
    /// </summary>
    /// <returns>下9マスの情報(正⽅形状)</returns>
    public FieldObject[] LookDown() => Order(OrderName.LookDown);

    /// <summary>
    /// 正⽅形状に左9マスの情報を取得します。
    /// </summary>
    /// <returns>左9マスの情報(正⽅形状)</returns>
    public FieldObject[] LookLeft() => Order(OrderName.LookLeft);

    /// <summary>
    /// 正⽅形状に右9マスの情報を取得します。
    /// </summary>
    /// <returns>右9マスの情報(正⽅形状)</returns>
    public FieldObject[] LookRight() => Order(OrderName.LookRight);

    /// <summary>
    /// 直線状に上9マスの情報を取得します。
    /// </summary>
    /// <returns>上9マスの情報(直線状)</returns>
    public FieldObject[] SearchUp() => Order(OrderName.SearchUp);

    /// <summary>
    /// 直線状に下9マスの情報を取得します。
    /// </summary>
    /// <returns>下9マスの情報(直線状)</returns>
    public FieldObject[] SearchDown() => Order(OrderName.SearchDown);

    /// <summary>
    /// 直線状に左9マスの情報を取得します。
    /// </summary>
    /// <returns>左9マスの情報(直線状)</returns>
    public FieldObject[] SearchLeft() => Order(OrderName.SearchLeft);

    /// <summary>
    /// 直線状に右9マスの情報を取得します。
    /// </summary>
    /// <returns>右9マスの情報(直線状)</returns>
    public FieldObject[] SearchRight() => Order(OrderName.SearchRight);

    /// <summary>
    /// 上にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public FieldObject[] PutUp() => Order(OrderName.PutUp);

    /// <summary>
    /// 下にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public FieldObject[] PutDown() => Order(OrderName.PutDown);

    /// <summary>
    /// 左にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public FieldObject[] PutLeft() => Order(OrderName.PutLeft);

    /// <summary>
    /// 右にブロックを置きます。
    /// </summary>
    /// <returns>周囲9マスの情報</returns>
    public FieldObject[] PutRight() => Order(OrderName.PutRight);

    /// <summary>
    /// CHaserサーバーに接続する
    /// </summary>
    private void Connect()
    {
        try
        {
            //接続
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketClient.Connect(new IPEndPoint(IPAddress.Parse(ip), port));

            //クライアント情報送信
            Send($"{name}\r\n");

            Console.WriteLine(
                "Connection completed.\n" +
                "Plz wait game start...\n"
            );
        }
        catch (SocketException)
        {
            //接続失敗時
            Console.WriteLine(
                "Failed to connect CHaser server.\n" +
                "(Press \"Enter\" to retry)"
            );
            Console.ReadLine();
            Connect(); //リトライ(再帰)
        }
    }

    /// <summary>
    /// サーバー接続を閉じる
    /// </summary>
    private void Close()
    {
        socketClient.Shutdown(SocketShutdown.Both);
        socketClient.Close();
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
    private char[] Receive()
    {
        try
        {
            byte[] bytes = new byte[4096];
            int response = socketClient.Receive(bytes);
            return Encoding.UTF8.GetString(bytes, 0, response).ToCharArray();
        }
        catch (SocketException)
        {
            //接続失敗時
            throw new CHaserClientException(
                "Connection terminated!\n" +
                "(Press \"Enter\" to exit)"
            );
        }
    }

    /// <summary>
    /// CHaserサーバーと通信する
    /// </summary>
    /// <param name="orderString">送信する命令</param>
    /// <returns>受信した情報</returns>
    /// <exception cref="Exception"></exception>
    private FieldObject[] Order(OrderName order)
    {
        try
        {
            if (order == OrderName.GetReady)
            {
                if (!(Receive()[0] == ControlCode.TurnStart)) //接続確認
                {
                    //ダメだった場合
                    throw new CHaserClientException(
                        "Connection terminated!\n" +
                        "(Press \"Enter\" to exit)"
                    );
                }
            }

            //サーバーへ命令送信
            Send($"{((OrderCode)order).ToString()}\r\n");
            //命令のコンソール出力
            if (++currentTurn % 2 == 0) Console.WriteLine("************************************\n");
            Console.WriteLine($"Turn{currentTurn / 2}: {order.ToString()}");

            //サーバーから情報受信
            FieldObject[] response = Receive().Take(10).Select(chara => (FieldObject)(int.Parse(chara.ToString()))).ToArray();

            if (order != OrderName.GetReady)
            {
                Send($"{ControlCode.TurnEnd}\r\n");
            }

            switch ((GameStatus)response[0])
            {
                case GameStatus.Progress:
                    response = response.Skip(1).Take(9).ToArray();
                    WriteFieldInfo(response, order);
                    return response;

                case GameStatus.Finished:
                    throw new CHaserClientException(
                        "Game Set!!\n" +
                        "(Press \"Enter\" to exit)"
                    );

                default:
                    throw new Exception("Response Error.");
            }
        }
        catch
        {
            Close(); //接続は必ず閉じる
            throw; //Exceptionを受け流す
        }
    }

    private void WriteFieldInfo(FieldObject[] infos, OrderName order)
    {
        int i = 0;
        foreach (FieldObject info in infos)
        {
            switch (info)
            {
                case FieldObject.none:
                    Console.Write("[　]");
                    break;
                case FieldObject.block:
                    Console.Write("[□]");
                    break;
                case FieldObject.item:
                    Console.Write("[☆]");
                    break;
                case FieldObject.chara:
                    Console.Write("[×]");
                    break;
            }
            if (++i % 3 == 0 && order is not (OrderName.SearchDown or OrderName.SearchUp or OrderName.SearchLeft or OrderName.SearchRight))
            {
                Console.WriteLine();
            }
            else if (i % 9 == 0) Console.WriteLine();
        }
        Console.WriteLine();
    }
}