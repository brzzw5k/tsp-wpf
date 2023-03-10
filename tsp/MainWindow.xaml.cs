using H.Pipes;
using H.Pipes.Args;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;
using tsp_shared;

namespace tsp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PipeServer<PipeMessage> PipeServer;
        private Cycle Cycle;
        private ComputeMethod ComputeMethod = ComputeMethod.TASK;

        private string TASK_CLIENT_EXE_PATH = "C:\\Users\\jbrzozowski\\PG\\tsp-wpf\\tsp-task\\bin\\Debug\\net6.0\\tsp-task.exe";
        private string THREAD_CLIENT_EXE_PATH = "C:\\Users\\jbrzozowski\\PG\\tsp-wpf\\tsp-thread\\bin\\Debug\\net6.0\\tsp-thread.exe";
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(async () => await StartServerAsync());
        }

        private async Task StartServerAsync()
        {
            await using var server = new PipeServer<PipeMessage>(Client.PipeName);
            PipeServer = server;
            server.ClientConnected += async (o, args) =>
            {
                Console.WriteLine($"Client {args.Connection.PipeName} is now connected!");
                await Application.Current.Dispatcher.Invoke(() => args.Connection.WriteAsync(new PipeMessage { Cycle = Cycle, Value = int.Parse(ConcurrencyTextBox.Text), PipeMessageType = PipeMessageType.START }));
            };
            server.ClientDisconnected += (o, args) =>
            {
                Console.WriteLine($"Client {args.Connection.PipeName} disconnected");
            };
            server.MessageReceived += (sender, args) => OnMessageReceived(sender, args);

            server.ExceptionOccurred += (o, args) => OnExceptionOccurred(args.Exception);

            await server.StartAsync();

            await Task.Delay(Timeout.InfiniteTimeSpan);
        }

        private void OnMessageReceived(object sender, ConnectionMessageEventArgs<PipeMessage> args)
        {
            if (args.Message.PipeMessageType == PipeMessageType.PROGRESS)
            {
                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = args.Message.Value;
                });
            }
            else if (args.Message.PipeMessageType == PipeMessageType.NEW_BEST)
            {
                Dispatcher.Invoke(() =>
                {
                    var currentCycleLength = Cycle.GetLength();
                    var newCycleLength = args.Message.Cycle.GetLength();
                    if (currentCycleLength > newCycleLength)
                    {
                        Cycle = args.Message.Cycle;
                        BestSolutionTextBox.Text = newCycleLength.ToString();
                        Log.Items.Add($"Solution: {newCycleLength} by {ComputeMethod.GetName(ComputeMethod)} {args.Message.ID}");
                        Application.Current.Dispatcher.Invoke(() => DrawCycle());
                    }
                });
            }
            else if (args.Message.PipeMessageType == PipeMessageType.FINISH)
            {
                Dispatcher.Invoke(() =>
                {
                    ProgressBar.Value = 0;
                    Application.Current.Dispatcher.Invoke(() => DrawCycle());
                });
            }
        }

        private void OnExceptionOccurred(Exception exception)
        {
            throw new NotImplementedException();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            ExitButton.IsEnabled = false;

            if (ComputeMethod == ComputeMethod.TASK)
            {
                Process.Start(TASK_CLIENT_EXE_PATH);
            }
            else if (ComputeMethod == ComputeMethod.THREAD)
            {
                Process.Start(THREAD_CLIENT_EXE_PATH);
            }

            _ = StopAfterTimeoutAsync();

        }

        private async Task StopAfterTimeoutAsync()
        {
            var ms = TimeoutTextBox.Text == "" ? 0 : int.Parse(TimeoutTextBox.Text);
            await Task.Delay(ms * 1000);
            _ = Application.Current.Dispatcher.Invoke(async () => await PipeServer.WriteAsync(new PipeMessage { PipeMessageType = PipeMessageType.STOP }));

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ExitButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            _ = Application.Current.Dispatcher.Invoke(async () => await PipeServer.WriteAsync(new PipeMessage { PipeMessageType = PipeMessageType.STOP }));

            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            ExitButton.IsEnabled = true;

        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                Cycle = new Cycle(path);
                Cycle = Cycle.ShuffledCopy();
                BestSolutionTextBox.Text = Cycle.GetLength().ToString();
                Log.Items.Clear();
                Application.Current.Dispatcher.Invoke(() => DrawCycle());
            }
        }

        private void TaskRadio_Click(object sender, RoutedEventArgs e)
        {
            ComputeMethod = ComputeMethod.TASK;
        }

        private void ThreadRadio_Click(object sender, RoutedEventArgs e)
        {
            ComputeMethod = ComputeMethod.THREAD;
        }

        private void DrawCycle()
        {
            CycleCanvas.Children.Clear();
            var nodes = Cycle.GetNormalizedNodes((float)CycleCanvas.ActualWidth, (float)CycleCanvas.ActualHeight);

            for (int i = 0; i < nodes.Count - 1; i++)
            {
                DrawLine(nodes[i], nodes[i + 1]);
            }
            DrawLine(nodes[^1], nodes[0]);

            for (int i = 0; i < nodes.Count; i++)
            {
                DrawNode(nodes[i]);
            }
        }

        private void DrawLine(Node node1, Node node2)
        {
            var line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;
            line.X1 = node1.Position.X;
            line.Y1 = node1.Position.Y;
            line.X2 = node2.Position.X;
            line.Y2 = node2.Position.Y;
            CycleCanvas.Children.Add(line);
        }

        private void DrawNode(Node node)
        {
            var ellipse = new Ellipse();
            ellipse.Fill = Brushes.Red;
            ellipse.Width = 10;
            ellipse.Height = 10;
            ellipse.Margin = new Thickness(node.Position.X - 5, node.Position.Y - 5, 0, 0);
            CycleCanvas.Children.Add(ellipse);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Cycle != null)
            {
                Application.Current.Dispatcher.Invoke(() => DrawCycle());
            }
        }
    }

    public enum ComputeMethod
    {
        THREAD,
        TASK
    }
}
