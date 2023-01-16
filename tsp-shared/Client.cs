using H.Pipes;
using H.Pipes.Args;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace tsp_shared
{
    public class Client
    {
        public Action<PipeMessage> onMessageReceived;
        private PipeClient<PipeMessage> PipeClient;
        public const string PipeName = "tsp_pipe";

        public Client(string pipeName)
        {
            Task.Run(async () => await StartClientAsync(pipeName));
        }

        public async Task StartClientAsync(string pipeName)
        {
            await using var pipeClientTemp = new PipeClient<PipeMessage>(pipeName);
            PipeClient = pipeClientTemp;
            pipeClientTemp.MessageReceived += (o, args) => OnMessageReceived(o, args);
            pipeClientTemp.Disconnected += (o, args) => Console.WriteLine("Disconnected from server");
            pipeClientTemp.Connected += (o, args) => Console.WriteLine("Connected to server");
            pipeClientTemp.ExceptionOccurred += (o, args) => OnExceptionOccurred(args.Exception);

            await pipeClientTemp.ConnectAsync();
            await Task.Delay(Timeout.InfiniteTimeSpan);
        }

        private void OnMessageReceived(object o, ConnectionMessageEventArgs<PipeMessage> args)
        {
            onMessageReceived.Invoke(args.Message);
        }

        private void OnExceptionOccurred(Exception exception)
        {
            Console.WriteLine(exception.Message);
        }

    }
}
