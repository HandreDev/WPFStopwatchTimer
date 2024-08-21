using System.Diagnostics;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WPFStopwatchTimer
{
    public class Stopwatch
    {
        public bool On { get; set; }
        public bool Paused { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        public int Millisecond { get; set; }
        private System.Timers.Timer _timer;

        public event Action TimeUpdated = delegate { };

        public Stopwatch()
        {
            _timer = new System.Timers.Timer(10);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            Reset();

            On = false;
            Paused = false;
            Minute = 0;
            Second = 0;
            Millisecond = 0;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Tick();
            TimeUpdated?.Invoke();
        }

        public void Start()
        {
            On = true;
            Paused = false;
            _timer.Start();
        }

        public void Pause()
        {
            Paused = true;
            _timer.Stop();
        }

        public void Reset()
        {
            On = false;
            Paused = false;
            Minute = 0;
            Second = 0;
            Millisecond = 0;
            _timer.Stop();
        }

        private void Tick()
        {
            if (On && !Paused)
            {
                Millisecond += 1;

                if (Millisecond >= 100)
                {
                    Millisecond = 0;
                    Second += 1;
                }

                if (Second >= 60)
                {
                    Second = 0;
                    Minute += 1;
                }
            }
        }
    }

    public class Timer
    {
        public bool On { get; set; }
        public bool Paused { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Second { get; set; }
        private System.Timers.Timer _timer;

        public event Action TimeUpdated = delegate { };

        public Timer()
        {
            _timer = new System.Timers.Timer(1000);
            _timer.Elapsed += Timer_Elapsed;
            _timer.AutoReset = true;
            Reset();

            On = false;
            Paused = false;
            Hour = 0;
            Minute = 0;
            Second = 0;
        }

        private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            Tick();
            TimeUpdated?.Invoke();
        }

        public void Start()
        {
            On = true;
            Paused = false;
            _timer.Start();
        }

        public void Pause()
        {
            Paused = true;
            _timer.Stop();
        }

        public void Reset()
        {
            On = false;
            Paused = false;
            Hour = 0;
            Minute = 0;
            Second = 0;
            _timer.Stop();
        }

        private void Tick()
        {
            if (On && !Paused)
            {
                if (Hour <= 0 & Minute <= 0 & Second <= 0)
                {
                    On = false;
                    Paused = false;
                    _timer.Stop();
                    return;
                }

                if (Second > 0)
                {
                    Second -= 1;
                } else if (Minute > 0)
                {
                    Minute -= 1;
                    Second = 59;
                } else 
                {
                    Hour -= 1;
                    Minute = 59;
                    Second = 59;
                }
            }
        }
    }

    public partial class MainWindow : Window
    {
        private Stopwatch _stopwatch;
        private Timer _timer;

        public MainWindow()
        {
            InitializeComponent();
            _stopwatch = new Stopwatch();
            _stopwatch.TimeUpdated += StopwatchUpdateTimeDisplay;

            _timer = new Timer();
            _timer.TimeUpdated += TimerUpdateTimeDisplay;
        }

        private void btnStartStopwatch_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Start();
        }

        private void btnPauseStopwatch_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Pause();
        }

        private void btnResetStopwatch_Click(object sender, RoutedEventArgs e)
        {
            _stopwatch.Reset();
            StopwatchUpdateTimeDisplay();
        }

        private void StopwatchUpdateTimeDisplay()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                TimeDisplayMin.Text = _stopwatch.Minute.ToString("D2");
                TimeDisplaySec.Text = _stopwatch.Second.ToString("D2");
                TimeDisplayMili.Text = _stopwatch.Millisecond.ToString("D2");
            });
        }

        private void TimerInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                textBox.Dispatcher.BeginInvoke(new Action(() =>
                {
                    textBox.SelectAll();
                }), System.Windows.Threading.DispatcherPriority.Input);
            }
        }

        private void TimerInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
            {
                e.Handled = true;
            }
        }

        private void TimerInput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox && textBox.Text.Length > 2)
            {
                textBox.Text = textBox.Text.Substring(0, 2);
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private void TimerInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (int.TryParse(textBox.Text, out int number))
                {
                    textBox.Text = number.ToString("D2");
                }
                else
                {
                    textBox.Text = "00";
                }
            }
        }

        private void btnStartTimer_Click(object sender, RoutedEventArgs e)
        {
            if (!_timer.On)
            {
                _timer.Hour = Int32.Parse(txtTimeDisplayHr.Text);
                _timer.Minute = Int32.Parse(txtTimeDisplayMin.Text);
                _timer.Second = Int32.Parse(txtTimeDisplaySec.Text);
                txtTimeDisplayHr.IsReadOnly = true;
                txtTimeDisplayMin.IsReadOnly = true;
                txtTimeDisplaySec.IsReadOnly = true;
            }
            _timer.Start();
            btnTimerStart.IsEnabled = false;
        }

        private void btnPauseTimer_Click(object sender, RoutedEventArgs e)
        {
            _timer.Pause();
            btnTimerStart.IsEnabled = true;
        }

        private void btnResetTimer_Click(object sender, RoutedEventArgs e)
        {
            _timer.Reset();
            btnTimerStart.IsEnabled = true;
            txtTimeDisplayHr.IsReadOnly = false;
            txtTimeDisplayMin.IsReadOnly = false;
            txtTimeDisplaySec.IsReadOnly = false;
            TimerUpdateTimeDisplay();
        }

        private void TimerUpdateTimeDisplay()
        {
            
                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!_timer.On)
                    {
                        btnTimerStart.IsEnabled = true;
                        txtTimeDisplayHr.IsReadOnly = false;
                        txtTimeDisplayMin.IsReadOnly = false;
                        txtTimeDisplaySec.IsReadOnly = false;
                    }
                    else
                    {
                        txtTimeDisplayHr.Text = _timer.Hour.ToString("D2");
                        txtTimeDisplayMin.Text = _timer.Minute.ToString("D2");
                        txtTimeDisplaySec.Text = _timer.Second.ToString("D2");
                    }
                });
        }
    }
}