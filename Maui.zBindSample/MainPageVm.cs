using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Maui.zBindSample
{
    public class MainPageVm : INotifyPropertyChanged
    {
        private long _count;
        public event PropertyChangedEventHandler PropertyChanged;

        public MainPageVm()
        {
            var timer = Application.Current.Dispatcher.CreateTimer();
            timer.Interval = TimeSpan.FromMilliseconds(17);
            timer.Tick += (s, e) => Application.Current.Dispatcher.Dispatch(() => Count++);
            timer.Start();
        }

        public long Count
        {
            get => _count;
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged();
                }
            }
        }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
