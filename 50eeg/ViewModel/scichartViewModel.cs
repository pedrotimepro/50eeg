using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Media;

namespace _50eeg.ViewModel
{
    public class scichartViewModel : ViewModelBase
    {
        private ObservableCollection<EEGChannelViewModel> _channelViewModels;

        private readonly IList<Color> _colors = new[]
        {
            Colors.White, Colors.Yellow, Color.FromArgb(255, 0, 128, 128), Color.FromArgb(255, 176, 196, 222),
            Color.FromArgb(255, 255, 182, 193), Colors.Purple, Color.FromArgb(255, 245, 222, 179),Color.FromArgb(255, 173, 216, 230),
            Color.FromArgb(255, 250, 128, 114), Color.FromArgb(255, 144, 238, 144), Colors.Orange, Color.FromArgb(255, 192, 192, 192),
            Color.FromArgb(255, 255, 99, 71), Color.FromArgb(255, 205, 133, 63), Color.FromArgb(255, 64, 224, 208), Color.FromArgb(255, 244, 164, 96)
        };

        private readonly Random _random = new Random();
        private volatile int _currentSize;

        private const int ChannelCount = 50; // Number of channels to render
        private const int Size = 1000;       // Size of each channel in points (FIFO Buffer)

        private uint _timerInterval = 20; // Interval of the timer to generate data in ms        
        private int _bufferSize = 15;     // Number of points to append to each channel each timer tick

        private Timer _timer;
        private readonly object _syncRoot = new object();

        // X, Y buffers used to buffer data into the Scichart instances in blocks of BufferSize
        private double[] xBuffer;
        private double[] yBuffer;
        private bool _running;
        private bool _isReset;

        private readonly RelayCommand _startCommand;
        private readonly RelayCommand _stopCommand;
        private readonly RelayCommand _resetCommand;

        public scichartViewModel()
        {
            _startCommand = new RelayCommand(Start, () => !IsRunning);
            _stopCommand = new RelayCommand(Stop, () => IsRunning);
            _resetCommand = new RelayCommand(Reset, () => !IsRunning && !IsReset);
        }

        public ObservableCollection<EEGChannelViewModel> ChannelViewModels
        {
            get => _channelViewModels;
            set
            {
                _channelViewModels = value;
                RaisePropertyChanged("ChannelViewModels");
            }
        }

        public RelayCommand StartCommand => _startCommand;
        public RelayCommand StopCommand => _stopCommand;
        public RelayCommand ResetCommand => _resetCommand;

        public int PointCount => _currentSize * ChannelCount;

        public double TimerInterval
        {
            get => _timerInterval;
            set
            {
                _timerInterval = (uint)value;
                RaisePropertyChanged("TimerInterval");
                Stop();
            }
        }

        public double BufferSize
        {
            get => _bufferSize;
            set
            {
                _bufferSize = (int)value;
                RaisePropertyChanged("BufferSize");
                Stop();
            }
        }

        public bool IsReset
        {
            get => _isReset;
            set
            {
                _isReset = value;

                _startCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
                _resetCommand.RaiseCanExecuteChanged();

                RaisePropertyChanged("IsReset");
            }
        }

        public bool IsRunning
        {
            get => _running;
            set
            {
                _running = value;

                _startCommand.RaiseCanExecuteChanged();
                _stopCommand.RaiseCanExecuteChanged();
                _resetCommand.RaiseCanExecuteChanged();

                RaisePropertyChanged("IsRunning");
            }
        }

        private void Start()
        {
            if (_channelViewModels == null || _channelViewModels.Count == 0)
            {
                Reset();
            }

            if (!IsRunning)
            {
                IsRunning = true;
                IsReset = false;
                xBuffer = new double[_bufferSize];
                yBuffer = new double[_bufferSize];
                _timer = new Timer(_timerInterval);
                _timer.Elapsed += OnTick;
                _timer.AutoReset = true;
                _timer.Start();
            }
        }

        private void Stop()
        {
            if (IsRunning)
            {
                _timer.Stop();
                IsRunning = false;
            }
        }

        private void Reset()
        {
            Stop();

            // Initialize N EEGChannelViewModels. Each of these will be represented as a single channel
            // of the EEG on the view. One channel = one SciChartSurface instance
            ChannelViewModels = new ObservableCollection<EEGChannelViewModel>();

            for (int i = 0; i < ChannelCount; i++)
            {
                var channelViewModel = new EEGChannelViewModel(Size, _colors[i % 16]) { ChannelName = "Channel " + i };
                ChannelViewModels.Add(channelViewModel);
            }

            IsReset = true;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Ensure only one timer Tick processed at a time
            lock (_syncRoot)
            {
                foreach (var channel in _channelViewModels)
                {
                    var dataseries = channel.ChannelDataSeries;

                    // Preload previous value with k-1 sample, or 0.0 if the count is zero
                    double xValue = dataseries.Count > 0 ? dataseries.XValues[dataseries.Count - 1] : 0.0;

                    // Add points 10 at a time for efficiency   
                    for (int j = 0; j < BufferSize; j++)
                    {
                        // Generate a new X,Y value in the random walk
                        xValue += 1;
                        double yValue = _random.NextDouble();

                        xBuffer[j] = xValue;
                        yBuffer[j] = yValue;
                    }

                    // Append block of values
                    dataseries.Append(xBuffer, yBuffer);

                    // For reporting current size to GUI
                    _currentSize = dataseries.Count;
                }
            }
        }
    } 
 }
