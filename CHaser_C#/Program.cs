using System.Text.RegularExpressions;

string? ip = null;
string? port = null;
string? name = null;

if (args.Length > 0)
{
    //コマンドライン引数からセット
    ip = args.SingleOrDefault(arg => arg.Contains("ip:"))?.Replace("ip:", "");
    port = args.SingleOrDefault(arg => arg.Contains("port:"))?.Replace("port:", "");
    name = args.SingleOrDefault(arg => arg.Contains("name:"))?.Replace("name:", "");
}

if (ip is null)
{
    //コマンドライン引数からセットしていない場合
    do
    {
        //正しく入力していない間繰り返す
        Console.WriteLine("サーバーIPアドレス: ");
        ip = Console.ReadLine();
    } while (String.IsNullOrEmpty(ip) || !Regex.IsMatch(ip, @"[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}.[0-9]{1,3}"));
}

if (port is null)
{
    //コマンドライン引数からセットしていない場合
    do
    {
        //正しく入力していない間繰り返す
        Console.WriteLine("ポート番号: ");
        port = Console.ReadLine();
    } while (String.IsNullOrEmpty(port));
}

if (name is null)
{
    //コマンドライン引数からセットしていない場合
    do
    {
        //正しく入力していない間繰り返す
        Console.WriteLine("表示名: ");
        name = Console.ReadLine();
    } while (String.IsNullOrEmpty(name));
}

//CHaserクライアントを初期化し、プログラムを実行
new CHaser(new Client(ip, int.Parse(port), name)).Run();