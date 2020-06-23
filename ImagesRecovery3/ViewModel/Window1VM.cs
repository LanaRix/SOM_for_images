using ImagesRecovery3.View.Pages;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace ImagesRecovery3.ViewModel
{
    class Window1VM : INotifyPropertyChanged
    {
        private Page ArealPage, RandomPage;

        public Window1VM(int width, int height)
        {
            TitleName = "В изображении " + Convert.ToString(width * height) + " пикселей. (" + Convert.ToString(width) + "x" + Convert.ToString(height) + ")";
            ArealPage = new ArealPage();
            RandomPage = new RandomPage();
            CurrentPage = ArealPage;
            FrameOpacity = 1;
        }

        private string _titleName;
        public string TitleName
        {
            get { return _titleName; }
            set
            {
                _titleName = value;
                OnPropertyChanged("TitleName");
            }
        }

        private double _frameOpacity;
        public double FrameOpacity
        {
            get { return _frameOpacity; }
            set
            {
                _frameOpacity = value;
                OnPropertyChanged("FrameOpacity");
            }
        }

        private Page _currentPage;
        public Page CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged("CurrentPage");
            }
        }

        public ICommand RamdomButtonCommand
        {
            get
            {
                //return new RelayCommand(p => CurrentPage = RandomPage);
                return new RelayCommand(p => SlowOpacity(RandomPage));
            }
        }
        public ICommand ArealButtonCommand
        {
            get
            {
                //return new RelayCommand(p => CurrentPage = ArealPage);
                return new RelayCommand(p => SlowOpacity(ArealPage));
            }
        }

        private async void SlowOpacity(Page page)
        {
            await Task.Factory.StartNew(() =>
            {
                for (double i = 1.0; i > 0.0; i -= 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(50);
                }
                CurrentPage = page;
                for (double i = 0.0; i <= 1.0; i += 0.1)
                {
                    FrameOpacity = i;
                    Thread.Sleep(50);
                }
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
