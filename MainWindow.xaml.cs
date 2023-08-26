using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PomoSprint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        public enum WindowType
        {
            Break,
            Pomo,
            Complete
        }

        private bool isPomoStarted = false;
        private int numOfPomos = 0;
        private Thread? thread;
        private bool isFoucsTime = true;
        private MediaPlayer mediaPlayer = new MediaPlayer();
        private readonly Uri Bell_ring = new(System.IO.Path.GetDirectoryName(Environment.ProcessPath) + "\\Res\\Bell.wav");
        private static readonly Uri Breakicon = new(System.IO.Path.GetDirectoryName(Environment.ProcessPath) + "\\Res\\Break.png");
        private static readonly Uri Focusedicon = new(System.IO.Path.GetDirectoryName(Environment.ProcessPath) + "\\Res\\Work.png");
        private static readonly Uri Completedicon = new(System.IO.Path.GetDirectoryName(Environment.ProcessPath) + "\\Res\\Completed.png");

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new Speed_n();
        }

        public class Speed_n : INotifyPropertyChanged
        {
            private string Sec = "00";         //00
            private string Min = "25";        //25

            public string min
            {
                get { return Min; }
                set
                {
                    Min = value;
                    NotifyPropertyChanged(nameof(min));

                }
            }
            public string sec
            {
                get { return Sec; }
                set
                {
                    Sec = value;
                    NotifyPropertyChanged(nameof(sec));
                }
            }
            public event PropertyChangedEventHandler? PropertyChanged;

            protected virtual void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
            {
                var handler = PropertyChanged;
                if (handler != null)
                    handler(this, new PropertyChangedEventArgs(propertyName));
            }

        }

        public void StartPomo()
        {
            while (isPomoStarted)
            {
                Thread.Sleep(1000);
                if (Application.Current == null)
                {
                    return;
                }
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    update_GUI();
                }));


            }

        }


        private void update_GUI()
        {
            int Minu = Convert.ToInt16(Time_Min.Text);
            int Secs = Convert.ToInt16(Time_Sec.Text);
            if (Secs == 0)
            {
                if (Minu == 0)
                {
                    if (isFoucsTime)
                    {
                        Pomo_fin();
                        return;
                    }
                    else
                    {
                        Break_fin();
                        return;
                    }

                }
                else
                {
                    Minu = Minu - 1;
                    Secs = 59;
                }
            }
            else
            {
                Secs = Secs - 1;
            }
            Time_Min.Text = Minu.ToString("D2");
            Time_Sec.Text = Secs.ToString("D2");
            return;
        }
        private void Pomo_fin()
        {
            numOfPomos++;
            if (numOfPomos == 4)
            {

                TakeLongBreak();
                return;

            }
            else
            {
                TakeBreak();
            }

        }

        private void Break_fin()
        {
            isPomoStarted = false;
            if (thread != null)
            {
                thread.Join();
            }
            Time_Min.Text = "25";       //25
            Time_Sec.Text = "01";
            mediaPlayer.Open(Bell_ring);
            mediaPlayer.Play();
            PomoWindow pm = new PomoWindow(WindowType.Pomo);
            pm.ShowDialog();
            Thread.Sleep(1000);
            thread = new Thread(StartPomo);
            thread.Start();
            isFoucsTime = true;
            isPomoStarted = true;

        }

        private void TakeLongBreak()
        {
            isPomoStarted = false;
            numOfPomos = 0;
            if (thread != null)
            {
                thread.Join();
            }
            Time_Min.Text = "15";        //15
            Time_Sec.Text = "01";
            mediaPlayer.Open(Bell_ring);
            mediaPlayer.Play();
            PomoWindow cp = new PomoWindow(WindowType.Complete);
            cp.ShowDialog();
            Thread.Sleep(1000);
            thread = new Thread(StartPomo);
            thread.Start();
            isFoucsTime = false;
            isPomoStarted = true;
        }

        private void TakeBreak()
        {
            isPomoStarted = false;
            if (thread != null)
            {
                thread.Join();
            }
            Time_Min.Text = "05";          //05
            Time_Sec.Text = "01";
            mediaPlayer.Open(Bell_ring);
            mediaPlayer.Play();
            PomoWindow bw = new PomoWindow(WindowType.Break);
            bw.ShowDialog();
            Thread.Sleep(1000);
            thread = new Thread(StartPomo);
            thread.Start();
            isFoucsTime = false;
            isPomoStarted = true;

        }

        private void Window_move(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }




        private void Start_Timer(object sender, MouseButtonEventArgs e)
        {
            if (!isPomoStarted)
            {
                isPomoStarted = true;
                thread = new Thread(StartPomo);
                thread.Start();
                return;
            }


        }


        private void window_close(object sender, RoutedEventArgs e)
        {
            isPomoStarted = false;
            if (thread != null)
            {
                Thread.Sleep(1000);
                thread.Join();
            }
            this.Close();
        }

        private void Rest_Pomo(object sender, RoutedEventArgs e)
        {
            isPomoStarted = false;
            if (thread != null)
            {
                thread.Join();
                Time_Min.Text = "25";
                Time_Sec.Text = "01";
            }
            else
            {
                Time_Min.Text = "25";
                Time_Sec.Text = "00";
            }

            numOfPomos = 0;
        }



        public class PomoWindow : Window
        {
            public PomoWindow(WindowType _windowType)
            {
                this.Background = System.Windows.Media.Brushes.White;
                this.Topmost = true;
                this.Width = 500;
                this.Height = 300;
                this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                this.AllowsTransparency = true;
                this.WindowStyle = WindowStyle.None;
                StackPanel gd = new StackPanel();
                gd.Orientation = Orientation.Vertical;
                this.AddChild(gd);
                Image img = new Image();
                img.Width = 500;
                img.Height = 220;
                img.VerticalAlignment = VerticalAlignment.Center;
                img.HorizontalAlignment = HorizontalAlignment.Center;
                Label label = new Label();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Center;
                label.Foreground = System.Windows.Media.Brushes.Black;
                label.FontSize = 12;
                switch (_windowType)
                {
                    case WindowType.Break:
                        img.Source = new BitmapImage(Breakicon);
                        label.Content = "Good Job! You have completed a Pomodoro, You deserve a break.";
                        break;
                    case WindowType.Pomo:
                        img.Source = new BitmapImage(Focusedicon);
                        label.Content = "Let's get back to work! Avoid distractions and focus on work.";
                        break;
                    case WindowType.Complete:
                        img.Source = new BitmapImage(Completedicon);
                        label.Content = "Congratulation ! You have completed 4 pomorodo sessions, Take a long break champ.";
                        break;
                }
                gd.Children.Add(img);
                gd.Children.Add(label);
                Button button = new Button();
                button.Width = 50;
                button.Height = 25;
                button.VerticalAlignment = VerticalAlignment.Center;
                button.HorizontalAlignment = HorizontalAlignment.Center;
                button.Background = System.Windows.Media.Brushes.Green;
                button.Content = "OK";
                button.Margin = new Thickness(16);
                button.Click += PomoWindow_close;
                gd.Children.Add(button);
            }

            private void PomoWindow_close(object sender, RoutedEventArgs e)
            {
                this.Close();
            }
        }



    }
}
