using H.Pipes;
using H.Pipes.Args;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Numerics;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using tsp_shared;

namespace tsp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PipeServer<MyMessage> _pipe;
        private double minX;
        private double minY;
        private double maxX;
        private double maxY;
        private Cycle cycle;
        private Cycle best = new Cycle(new List<Vertex>());
        private ComputeMethod chosenMethod = ComputeMethod.TASK;
        public MainWindow()
        {
            InitializeComponent();
            Task.Run(async () => await Server());

            InitializeNewSet("C:\\Users\\jbrzozowski\\Downloads\\7th-semester-main\\7th-semester-main\\programowanie-aplikacji-lokalnych\\wi29.tsp");
        }

        private void InitializeNewSet(string path)
        {
            BestSolution.Text = "0";
            var vertexes = Reader.GetVertexesFromFile(path);
            cycle = new Cycle(vertexes);
            minX = cycle.GetMinX();
            minY = cycle.GetMinY();
            maxX = cycle.GetMaxX();
            maxY = cycle.GetMaxY();
            DrawCycle(cycle);
            best = cycle; 
        }

        public async Task Server()
        {
            _pipe = new PipeServer<MyMessage>("tsp-task");
            _pipe.MessageReceived += MessageReceivedFromClient;
            _pipe.ClientConnected += (a, b) =>
            {
                Application.Current.Dispatcher.Invoke(() => _pipe.WriteAsync(new MyMessage { Type = MessageType.START, Cycle = cycle, Concurrency = int.Parse(ConcurrencyInput.Text) }));
            };

            await _pipe.StartAsync();
            await Task.Delay(Timeout.InfiniteTimeSpan);
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if (chosenMethod == ComputeMethod.TASK)
            {

                Process.Start("C:\\Users\\jbrzozowski\\PG\\tsp-wpf\\tsp-task\\bin\\Debug\\net6.0-windows\\tsp-task.exe");
            }
            else if (chosenMethod == ComputeMethod.THREAD)
            {
                Process.Start("C:\\Users\\jbrzozowski\\PG\\tsp-wpf\\tsp-thread\\bin\\Debug\\net6.0-windows\\tsp-thread.exe");
            }


            DelayedStop();
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private async Task DelayedStop()
        {
            var miliseconds = DurationInput.Text == "" ? 0 : int.Parse(DurationInput.Text);
            await Task.Delay(miliseconds * 1000);
            Application.Current.Dispatcher.Invoke(() => _pipe.WriteAsync(new MyMessage { Type = MessageType.CANCEL }));
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() => _pipe.WriteAsync(new MyMessage { Type = MessageType.CANCEL }));
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if(dialog.ShowDialog() == true)
            {
                ChosenFile.Text = dialog.FileName;
                InitializeNewSet(dialog.FileName);
            }
        }

        private void MessageReceivedFromClient(object sender, ConnectionMessageEventArgs<MyMessage> args)
        {
            var message = args.Message;
            
            if(message.Type == MessageType.BEST_SOLUTION)
            {
                Application.Current.Dispatcher.Invoke(() => {DrawCycle(args.Message.Cycle);});
                Application.Current.Dispatcher.Invoke(() => BestSolution.Text = args.Message.Cycle.CalculateTotalDistance().ToString("0.00"));
                Application.Current.Dispatcher.Invoke(() => { myList.ItemsSource = args.Message.Cycle.Vertexes; });
            }
            else if (message.Type == MessageType.PROGRESS)
            {
                Application.Current.Dispatcher.Invoke(() => ProgressBar.Value = message.Progress * 100);
                if (message.Progress > 0.98)
                {
                    Application.Current.Dispatcher.Invoke(() => StartButton.IsEnabled = true);
                }
            }
        }

        public Cycle CreateRandomCycle()
        {
            var random = new Random();
            var vertexes = new List<Vertex>();

            for (int i = 0; i < 10; i++)
            {
                var x = random.Next(0, 400);
                var y = random.Next(0, 400);
                vertexes.Add(new Vertex(0, new Vector2(x, y)));
            }

            return new Cycle(vertexes);
        }

        public void DrawCycle(Cycle cycle)
        {
            MyCanvas.Children.Clear();

            for (int i = 0; i < cycle.Length; i++)
            {
                var currentVertex = cycle.Vertexes[i];
                var nextVertex = cycle.Vertexes[(i + 1) % cycle.Length];

                DrawLine(currentVertex.Point, nextVertex.Point);
            }

            foreach(var vertex in cycle.Vertexes)
            {
                DrawPoint(vertex.Point);
            }
        }

        private void DrawLine(Vector2 pointA, Vector2 pointB)
        {
            var line = new Line();
            line.Stroke = Brushes.Black;
            line.StrokeThickness = 2;

            line.X1 = ClampXToScreen(pointA.X);
            line.Y1 = ClampYToScreen(pointA.Y);

            line.X2 = ClampXToScreen(pointB.X);
            line.Y2 = ClampYToScreen(pointB.Y);

            MyCanvas.Children.Add(line);
        }

        private void DrawPoint(Vector2 point)
        {
            var ellipse = new Ellipse();
            var radius = 8;
            ellipse.Fill = Brushes.Red;
            ellipse.Width = radius;
            ellipse.Height = radius;
            var x = ClampXToScreen(point.X) - (radius / 2);
            var y = ClampYToScreen(point.Y) - (radius / 2);

            ellipse.Margin = new Thickness(x, y, 0, 0);
            MyCanvas.Children.Add(ellipse);
        }

        private double ClampXToScreen(double x)
        {
            var val = Utilities.StrangeClamp(x, minX, maxX, 0, MyCanvas.ActualWidth);
            ;
            return val;
        }
        
        private double ClampYToScreen(double y)
        {
            return Utilities.StrangeClamp(y, minY, maxY, 0, MyCanvas.ActualHeight);
        }

        private void ThreadsRadio_Click(object sender, RoutedEventArgs e)
        {
            chosenMethod = ComputeMethod.THREAD;
        }

        private void TasksRadio_Click(object sender, RoutedEventArgs e)
        {
            chosenMethod = ComputeMethod.TASK;
        }
    }

    public enum ComputeMethod
    {
        THREAD,
        TASK
    }
}
