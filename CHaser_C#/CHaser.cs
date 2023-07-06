public static class CHaser
{
    public static void Run(Client client)
    {
        //ここにコードを書きます。
        while (true)
        {
            string value = client.GetReady();
            value = client.SearchLeft();

            value = client.GetReady();

            if (value[7] != 2)
            {
                value = client.WalkDown();
            }
            else
            {
                value = client.PutUp();
            }

            value = client.GetReady();
            value = client.LookUp();

            value = client.GetReady();
            value = client.PutRight();
        }
    }
}