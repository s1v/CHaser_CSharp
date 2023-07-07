public static class CHaser
{
    public static void Run(Client client)
    {
        //ここにコードを書きます。
        while (true)
        {
            client.GetReady();
            client.SearchUp();

            client.GetReady();
            client.SearchLeft();

            client.GetReady();
            client.SearchDown();

            client.GetReady();
            client.SearchRight();

            client.GetReady();
            client.LookUp();

            client.GetReady();
            client.LookLeft();

            client.GetReady();
            client.LookDown();

            client.GetReady();
            client.LookRight();
        }
    }
}