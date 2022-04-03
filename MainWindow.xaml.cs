using MusikPlayer.FileManager;
using MusikPlayer.Logs;
using MusikPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace MusikPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const double RESIZE_BORDER_OFFSET = 5d;

        public bool IsMaximized => WindowState == WindowState.Maximized;

        private static MainWindow instance = null;

        public MainWindow()
        {
            InitializeComponent();

            this.Closed += OnMainwindowClosed;

            this.StateChanged += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                {
                    //this.RestoreButton.Visibility = Visibility.Visible;
                    //this.MaximizeButton.Visibility = Visibility.Collapsed;
                    this.rootGrid.Margin = new Thickness(RESIZE_BORDER_OFFSET);
                }
                else if (this.WindowState == WindowState.Normal)
                {
                    //this.RestoreButton.Visibility = Visibility.Collapsed;
                    //this.MaximizeButton.Visibility = Visibility.Visible;
                    this.rootGrid.Margin = new Thickness(0);
                }

            };

        }

        private void OnMainwindowClosed(object sender, EventArgs e)
        {
            Logger.Instance.RunLogg($"{nameof(MainWindow)}", $"{nameof(this.OnMainwindowClosed)}", "Schließe Programm");
            MainWindowViewModel.OnProgrammShutDown();

            //jetzt noch alle Threads/tasks killen
            if (Application.Current.MainWindow != null)
            {
                Application.Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                Application.Current.Shutdown();
            }
            this.Close();
            Logger.Instance.RunLogg($"{nameof(MainWindow)}", $"{nameof(this.OnMainwindowClosed)}", "Schließe Programm Ende Erfolgreich");
        }

        public static MainWindow GetInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainWindow();
                }
                return instance;
            }
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void OnMaximize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
        }

        private void OnRestore(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;

                //MoveToCurserPosition();
            }

            DragMove();
        }

        private void OnDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (IsMaximized)
            {
                this.OnRestore(null, null);
            }
            else
                this.OnMaximize(null, null);
        }
    }
}
