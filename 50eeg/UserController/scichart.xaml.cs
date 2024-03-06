using _50eeg.ViewModel;
using SciChart.Examples.ExternalDependencies.Data;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace _50eeg.UserController
{
    /// <summary>
    /// scichart.xaml 的交互逻辑
    /// </summary>
    public partial class scichart : UserControl
    {
        public scichart()
        {
            InitializeComponent();
            _stopWatch = Stopwatch.StartNew();
            _fpsAverage = new MovingAverage(5);

            this.Loaded += OnLoaded;
            this.Unloaded += OnUnloaded;
            this.DataContext = new scichartViewModel();
        }
        private Stopwatch _stopWatch;
        private double _lastFrameTime;
        private MovingAverage _fpsAverage;

        private void OnUnloaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }


        ///<summary>
        /// Purely for stats reporting (FPS). Not needed for SciChart rendering
        ///</summary>
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            if (StopButton.IsChecked != true && ResetButton.IsChecked != true)
            {
                // Compute the render time
                double frameTime = _stopWatch.ElapsedMilliseconds;
                double delta = frameTime - _lastFrameTime;
                double fps = 1000.0 / delta;

                // Push the fps to the movingaverage, we want to average the FPS to get a more reliable reading
                _fpsAverage.Push(fps);

                // Render the fps to the screen
                fpsCounter.Text = double.IsNaN(_fpsAverage.Current) ? "-" : string.Format("{0:0}", _fpsAverage.Current);

                // Render the total point count (all series) to the screen
                var eegExampleViewModel = (DataContext as scichartViewModel);
                pointCount.Text = eegExampleViewModel != null ? eegExampleViewModel.PointCount.ToString() : "Na";

                _lastFrameTime = frameTime;
            }
        }
    }
}
