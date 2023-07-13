public static class Client
{
    public static void Run(Connector connector)
    {
        //ここにコードを書きます。
        while (true)
        {
            connector.GetReady();
            connector.SearchUp();

            connector.GetReady();
            connector.SearchLeft();

            connector.GetReady();
            connector.SearchDown();

            connector.GetReady();
            connector.SearchRight();

            connector.GetReady();
            connector.LookUp();

            connector.GetReady();
            connector.LookLeft();

            connector.GetReady();
            connector.LookDown();

            connector.GetReady();
            connector.LookRight();
        }
    }
}