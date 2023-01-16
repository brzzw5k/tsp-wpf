using tsp_shared;

namespace tsp_task
{
    class Program
    {
        static Client Client;
        static CancellationTokenSource CancellationSource = new CancellationTokenSource();

        static void Main(string[] args)
        {
            Client = new Client(Client.PipeName);
            Client.onMessageReceived = OnMessageReceived;
            while (true) { }
        }

        private static void OnMessageReceived(PipeMessage message)
        {
            if (message.PipeMessageType == PipeMessageType.STOP)
            {
                CancellationSource.Cancel();
                Console.WriteLine("Cancelled");
            }
            else if (message.PipeMessageType == PipeMessageType.START)
            {
                try
                {
                    Task.Run(() => Work(Client, message, CancellationSource.Token));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                
            }
        }

        static void Work(Client client, PipeMessage message, CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting");
        }
    }
}