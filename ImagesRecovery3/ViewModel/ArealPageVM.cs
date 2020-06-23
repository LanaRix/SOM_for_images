using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ImagesRecovery3.ViewModel
{
    class ArealPageVM : INotifyPropertyChanged
    {
        public ArealPageVM()
        {
            XLeftTop = 0;
            YLeftTop = 0;
            XRightBottom = 0;
            YRightBottom = 0;
            IsReady = "";
        }

        private String _isReady;
        public String IsReady
        {
            get { return _isReady; }
            set
            {
                _isReady = value;
                OnPropertyChanged("IsReady");
            }
        }

        private UInt32 _xLeftTop;
        public UInt32 XLeftTop
        {
            get { return _xLeftTop; }
            set
            {
                _xLeftTop = value;
                OnPropertyChanged("XLeftTop");
            }
        }

        private UInt32 _yLeftTop;
        public UInt32 YLeftTop
        {
            get { return _yLeftTop; }
            set
            {
                _yLeftTop = value;
                OnPropertyChanged("YLeftTop");
            }
        }

        private UInt32 _xRightBottom;
        public UInt32 XRightBottom
        {
            get { return _xRightBottom; }
            set
            {
                _xRightBottom = value;
                OnPropertyChanged("XRightBottom");
            }
        }

        private UInt32 _yRightBottom;
        public UInt32 YRightBottom
        {
            get { return _yRightBottom; }
            set
            {
                _yRightBottom = value;
                OnPropertyChanged("YRightBottom");
            }
        }

        public ICommand ApplyCommand
        {
            get
            {
                return new RelayCommand(p => Apply());
            }
        }

        private void Apply()
        {
            uint x = XRightBottom - XLeftTop + 1;
            uint y = YRightBottom - YLeftTop + 1;
            int S = (int)(x < y ? y : x);
            //int S = (int)(x < y ? x : y);
            if (S % 2 == 0) S++;

            string path = @"areal.dat";

            if (File.Exists(path))
                File.Delete(path);

            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            writer.Write(1);
            writer.Write(S);
            writer.Write(XLeftTop);
            writer.Write(YLeftTop);
            writer.Write(XRightBottom);
            writer.Write(YRightBottom);
            writer.Close();
            IsReady = "Готово! Закройте текущее окно.";
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
