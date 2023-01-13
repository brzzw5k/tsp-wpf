using H.Pipes;
using H.Pipes.Args;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tsp_shared
{
    public class Client
    {
        public Action<MyMessage> OnMessageReceived;

        private PipeClient<MyMessage> client;
        public Client(string pipeName)
        {
            Task.Run(async () => await StartClient(pipeName));
        }

        public async Task StartClient(string pipeName)
        {
            await using var clientTemp = new PipeClient<MyMessage>(pipeName);
            this.client = clientTemp;
            clientTemp.MessageReceived += MessageReceivedFromServer;

            await clientTemp.ConnectAsync();
            Console.WriteLine("Connected");
            await Task.Delay(Timeout.InfiniteTimeSpan);
        }

        private void MessageReceivedFromServer(object sender, ConnectionMessageEventArgs<MyMessage> args)
        {
            OnMessageReceived.Invoke(args.Message);
        }

        public void SendMessage(MyMessage message)
        {
            client.WriteAsync(message);
        }
    }
}
