using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ImagesRecovery3.ViewModel
{
    class RandomPageVM : INotifyPropertyChanged
    {
        public RandomPageVM()
        {
            Count = 1;
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


        private UInt32 _count;
        public UInt32 Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged("Count");
            }
        }
        public UInt32 getCount()
        {
            return Count;
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
            int S = (int)Math.Pow(Count, 0.5);
            if (S % 2 == 0) S++;

            string path = @"areal.dat";

            if (File.Exists(path))
                File.Delete(path);

            BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.OpenOrCreate));
            writer.Write(2);
            writer.Write(S);
            writer.Write(Count);
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
