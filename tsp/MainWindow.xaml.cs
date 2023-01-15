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

namespace tsp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double minX;
        private double minY;
        private double maxX;
        private double maxY;
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
                SelectedFile.Text = dialog.FileName;
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
    }

    public enum ComputeMethod
    {
        THREAD,
        TASK
    }
}
