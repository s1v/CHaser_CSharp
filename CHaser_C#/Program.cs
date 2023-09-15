using CHaserConnector.exception;
using System.Text.RegularExpressions;

string? ip = null;
string? port = null;
string? name = null;

if (args.Length > 0)
{
    //コマンドライン引数からセット
    ip = args.FirstOrDefault(arg => arg.Contains("ip:"))?.Replace("ip:", "");
    port = args.FirstOrDefault(arg => arg.Contains("port:"))?.Replace("port:", "");
    name = args.FirstOrDefault(arg => arg.Contains("name:"))?.Replace("name:", "");
}

if (ip is null || !Regex.IsMatch(ip, @"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}"))
{
    //コマンドライン引数からセットしていないもしくは正しくない場合
    do
    {
        //正しく入力していない間繰り返す
        Console.Write("サーバーIPアドレス: ");
        ip = Console.ReadLine();
    } while (String.IsNullOrEmpty(ip) || !Regex.IsMatch(ip, @"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}"));
}
else Console.WriteLine($"サーバーIPアドレス: {ip}");

if (port is null)
{
    //コマンドライン引数からセットしていない場合
    do
    {
        //正しく入力していない間繰り返す
        Console.Write("ポート番号: ");
        port = Console.ReadLine();
    } while (String.IsNullOrEmpty(port));
}
else Console.WriteLine($"ポート番号: {port}");

if (name is null)
{
    //コマンドライン引数からセットしていない場合
    do
    {
        //正しく入力していない間繰り返す
        Console.Write("表示名: ");
        name = Console.ReadLine();
    } while (String.IsNullOrEmpty(name));
}
else Console.WriteLine($"表示名: {name}\n");

//コネクターの生成
Connector connector = new Connector(ip, int.Parse(port), name);

while (true) //接続に成功するまでリトライする
{
    try
    {
        connector.Connect(); //サーバーへ接続
        break; //成功したら脱ループ
    }
    catch (ConnectException)
    {
        //リトライ表示
        Console.WriteLine("(Press \"Enter\" to retry)");
        Console.ReadLine();
    }
}

try
{
    Client client = new Client(connector); //Clientの生成
    client.Run(); //Clientの実行
}
catch (CHaserConnectorException e)
{
    //意図したExceptionの場合
    Console.WriteLine(e.Message);
    Console.WriteLine("(Press \"Enter\" to exit)");
    Console.ReadLine();
}
catch (Exception e)
{
    //意図せぬExceptionの場合
    Console.WriteLine(
        $"Occurred critical error.\n" +
        $"Plz report to GitHub Issues: https://github.com/s1v/CHaser_Connector/issues\n" +
        $"****************************\n" +
        $"ErrorMessage:\n" +
        $"{e.Message}\n" +
        $"StackTrace:\n" +
        $"{e.StackTrace}\n" +
        $"****************************\n" +
        $"(Press \"Enter\" to exit)"
    );
    Console.ReadLine();
}
finally
{
    connector.Dispose();
}
