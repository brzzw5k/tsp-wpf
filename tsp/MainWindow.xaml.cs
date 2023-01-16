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
        private Cycle Cycle;
        private ComputeMethod ComputeMethod = ComputeMethod.TASK;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = false;
            StopButton.IsEnabled = true;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            StartButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if(dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                Cycle = new Cycle(path);
                DrawCycle();
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
    }

    public enum ComputeMethod
    {
        THREAD,
        TASK
    }
}
