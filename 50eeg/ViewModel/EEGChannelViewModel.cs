using GalaSoft.MvvmLight;
using SciChart.Charting.Model.DataSeries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace _50eeg.ViewModel
{
    public class EEGChannelViewModel : ViewModelBase
    {
        private Color _color;
        private IXyDataSeries<double, double> _channelDataSeries;

        public EEGChannelViewModel(int size, Color color)
        {
            Stroke = color;

            // Add an empty First In First Out series. When the data reaches capacity (int size) then old samples
            // will be pushed out of the series and new appended to the end. This gives the appearance of 
            // a scrolling chart window
            ChannelDataSeries = new XyDataSeries<double, double> { FifoCapacity = size };

            // Pre-fill with NaN up to size. This stops the stretching effect when Fifo series are filled with AutoRange
            for (int i = 0; i < size; i++)
            {
                ChannelDataSeries.Append(i, double.NaN);
            }
        }

        public string ChannelName { get; set; }

        public Color Stroke
        {
            get => _color;
            set
            {
                _color = value;
                RaisePropertyChanged("Stroke");
            }
        }

        public IXyDataSeries<double, double> ChannelDataSeries
        {
            get => _channelDataSeries;
            set
            {
                _channelDataSeries = value;
                RaisePropertyChanged("ChannelDataSeries");
            }
        }

        public void Reset()
        {
            _channelDataSeries.Clear();
        }
    }
}
