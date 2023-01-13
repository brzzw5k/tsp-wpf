using System;
using tsp;
using H.Pipes;
using System.Threading.Tasks;
using System.Threading;
using H.Pipes.Args;
using System.Diagnostics;
using tsp_shared;
using System.Windows.Documents;
using System.Collections.Generic;
using Microsoft.VisualBasic;

namespace tsp_task
{
    internal class Program
    {
        static Cycle best = null;
        static object cycleLock = new object();

        static int progress = 0;
        static object progressLock = new object();

        static Client client;

        const int FIRST_ITERATIONS = 1000;
        const int SECOND_ITERATIONS = 1000;
        static int concurrency;
        static double lastProgressSent = 0;
        static CancellationTokenSource cancellationSource = new CancellationTokenSource();
        static void Main(string[] args)
        {
            client = new Client("tsp-task");

            client.OnMessageReceived += (MyMessage message) =>
            {
                if(message.Type == MessageType.CANCEL)
                {
                    cancellationSource.Cancel();
                    Console.WriteLine("Cancelled");
                }
                else if (message.Type == MessageType.START)
                {
                    Console.WriteLine($"Running {message.Concurrency} tasks");

                    concurrency = message.Concurrency;
                    for (int i = 0; i < message.Concurrency; i++)
                    {
                        try
                        {
                            Task.Run(() => DoWork(message, client, cancellationSource.Token));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Cancelled");
                        }
                    }
                }
            };

            while (true) { }
        }
        
        static void DoWork(MyMessage message, Client client, CancellationToken token)
        {
            Console.WriteLine("Starting calculations...");

            for (int i = 0; i < FIRST_ITERATIONS; i++)
            {
                var a = message.Cycle.GetShuffledCopy();
                var b = message.Cycle.GetShuffledCopy();

                var mutatedA = Transformations.PMXMutate(a, b);
                var mutatedB = Transformations.PMXMutate(b, a);


                for (int j = 0; j < SECOND_ITERATIONS; j++)
                {
                    if(token.IsCancellationRequested)
                    {
                        token.ThrowIfCancellationRequested();
                    }

                    //iteration++;
                    var optedA = Transformations.ThreeOpt(mutatedA);
                    var optedB = Transformations.ThreeOpt(mutatedB.GetCopy());

                    var localBest = optedA.CalculateTotalDistance() < optedB.CalculateTotalDistance() ? optedA : optedB;

                    if (j % 100 == 0)
                    {
                        //client.SendMessage(new MyMessage { Type = MessageType.PROGRESS, Progress = (double)iteration / totalIterations });
                        UpdateProgress(100);
                    }

                    lock (cycleLock)
                    {
                        if (best == null || localBest.CalculateTotalDistance() < best.CalculateTotalDistance())
                        {
                            best = localBest.GetCopy();
                            client.SendMessage(new MyMessage { Type = MessageType.BEST_SOLUTION, Cycle = best });
                        }
                    }
                }
            }

            Console.WriteLine("Finished calculations");
        }

        static void UpdateProgress(int amount)
        {
            lock (progressLock)
            {
                progress+=amount;
                var progressComputed = (double)progress / (FIRST_ITERATIONS * SECOND_ITERATIONS * concurrency);

                if (progressComputed - lastProgressSent > 0.01)
                {
                    client.SendMessage(new MyMessage { Type = MessageType.PROGRESS, Progress = progressComputed });
                    lastProgressSent = progressComputed;
                }

                if (progressComputed == 1)
                {
                    Environment.Exit(0);
                }
            }
        }
    }
}
