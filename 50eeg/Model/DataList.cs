using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _50eeg.Model
{
    interface IUpdate
    {
        List<Queue<double>> Datalist { get; set; }
        /// <summary>
        /// /上传数据
        /// </summary>
        /// <param name="signals">信号值</param>
        /// <param name="Items">通道数</param>
        void Update(double signals, int Items);
        void Load();
        void Save();
        bool NeedsSave { get; set; }

    }
    public class DataList : IUpdate
    {
        private bool mNeedSave;

        public bool NeedsSave { get { return mNeedSave; } set { mNeedSave = value; } }

        private List<Queue<double>> datalist = new List<Queue<double>>();
        public List<Queue<double>> Datalist
        {
            get
            {
                return datalist;
            }
            set
            {
                datalist = value;
            }
        }

        public void Load()
        {
            Console.WriteLine("Load");
        }

        public void Save()
        {
            Console.WriteLine("Save");
        }

        public void Update(Double signals, int Items)
        {
            while (this.datalist.Count < 50) { this.datalist.Add(new Queue<double>()); }
            Datalist[Items].Enqueue(signals);
        }
        public double DataGet(int channels)
        {
            if (Datalist[channels].Count > 0)
            {
                return Datalist[channels].Dequeue();
            }
            else
            {
                return 0;
            }
            
        }
    }
}
