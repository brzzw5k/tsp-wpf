using tsp_shared;

namespace tsp_thread
{
    class Program
    {
        static bool isRunning = true;
        static Client Client;
        static List<Thread> Threads;
        static CancellationTokenSource CancellationSource = new CancellationTokenSource();
        private static object cycleLock = new();
        private static object progressLock = new();
        private static Cycle best;
        static int concurrencyCount = 0;
        const int ITERATIONS_1 = 1000;
        const int ITERATIONS_2 = 1000;

        static void Main(string[] args)
        {
            Client = new Client(Client.PipeName);
            Threads = new List<Thread>();
            Client.onMessageReceived += OnMessageReceived;
            while (isRunning) { }
        }

        private static void OnMessageReceived(PipeMessage message)
        {
            if (message.PipeMessageType == PipeMessageType.STOP)
            {
                CancellationSource.Cancel();
                Console.WriteLine("Cancelled");
                foreach (var thread in Threads)
                {
                    thread.Interrupt();
                }
                isRunning = false;
            }
            else if (message.PipeMessageType == PipeMessageType.START)
            {
                best = message.Cycle;
                concurrencyCount = message.Value;
                for (int i = 0; i < concurrencyCount; i++)
                {
                    Thread thread = new Thread(() => Work(Client, message.Cycle, i, CancellationSource.Token));
                    thread.Start();
                    Threads.Add(thread);
                }
            }
        }

        static void Work(Client client, Cycle cycle, int id, CancellationToken cancellationToken)
        {
            if (cycle == null)
            {
                Console.WriteLine("No cycle provided");
                return;
            }

            Console.WriteLine("Starting");

            for (int i = 0; i < ITERATIONS_1; i++)
            {
                var cycleA = cycle.ShuffledCopy();
                var cycleB = cycle.ShuffledCopy();
                var cycleAMutated = Transformations.PMXMutate(cycleA, cycleB);
                var cycleBMutated = Transformations.PMXMutate(cycleB, cycleA);

                for (int j = 0; j < ITERATIONS_2; j++)
                {
                    var cycleAThreeOpted = Transformations.ThreeOPT(cycleAMutated);
                    var cycleBThreeOpted = Transformations.ThreeOPT(cycleBMutated);
                    var localBest = cycleAThreeOpted.GetLength() < cycleBThreeOpted.GetLength() ? cycleAThreeOpted : cycleBThreeOpted;
                    lock (cycleLock)
                    {
                        if (best == null || localBest.GetLength() < best.GetLength())
                        {
                            best = localBest;
                            Console.WriteLine($"New best: {best.GetLength()}");
                            _ = client.SendAsync(new PipeMessage() { Cycle = best, PipeMessageType = PipeMessageType.NEW_BEST, ID = id });
                        }
                    }

                }
            }

            Console.WriteLine("Finished");
            _ = client.SendAsync(new PipeMessage() { Cycle = best, PipeMessageType = PipeMessageType.FINISH });
        }
        
    }
}