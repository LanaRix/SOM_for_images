using ImagesRecovery3.Model.Algorithm;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ImageSOM;
using System;
using System.Threading;
using ImagesRecovery3.View.Forms;

namespace ImagesRecovery3.ViewModel
{
    class MainWindowVM : INotifyPropertyChanged
    {
        ChooseFile chooseFile = new ChooseFile();
        Load load = new Load();
        ImageModel ImageModel;
        NeuronModel[,] neurons;
        NeuronModel2[,] neurons2;
        public MainWindowVM()
        {
            FileName = "Выберете изображение";
            ImageModel = null;
            neurons = null;
        }

     

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }

        //кнопка "Выберете файл"
        public RelayCommand ChooseFileCommand
        {
            get
            {
                return new RelayCommand(p => 
                {
                    FileName = chooseFile.ChooseImage();
                    load.LoadImageToImageModel(FileName);
                    FileName = load.FileName1;
                });
            }
        }

        //кнопка повредить изображение
        public RelayCommand BreakImageCommand
        {
            get
            {
                return new RelayCommand(p =>
                {
                    load.LoadDamageToImageModel();
                    FileName = load.FileName1;
                });
            }
        }

        //конпка загрузить ранее созданный файл
        public RelayCommand ChooseReadyCommand
        {
            get
            {
                return new RelayCommand(p => 
                {
                    load.LoadReadyFile();
                    FileName = load.FileName1;
                });
            }
        }
        public RelayCommand TeachNeronsCommand
        {
            get
            {
                return new RelayCommand(p => 
                {
                    if (load.Model != null)
                    {
                        //SOM2 som = new SOM2(load.Model, load.S1, load.DamagePixelsCount1);
                        MapRecordAndReading2 mapRecord = new MapRecordAndReading2();
                        string path = load.SaveMapPath();

                        new Thread(() =>
                        {
                            int T = load.CurclesCount()+10;
                            SOM3 som = new SOM3(load.Model, load.S1, load.DamagePixelsCount1, T);
                            som.Create10Points();
                            neurons2 = som.CreateCard();

                            if (!path.Equals(""))
                                mapRecord.Record(neurons2, path, T);

                        }).Start();
                    }
                    else
                    {
                        MapRecordAndReading2 mapRecord = new MapRecordAndReading2();
                        string path = load.SaveMapPath();

                        int T = load.CurclesCount();

                        //загружаем карту
                        MapRecordAndReading2 mapReading = new MapRecordAndReading2();
                        ChooseFile file = new ChooseFile();
                        neurons2 = mapReading.Read(file.ChooseDatFile());

                        int t = mapReading.Iter;


                        //загружаем сломанное изображение
                        load.LoadReadyFile();
                        FileName = load.FileName1; //получаю путь к jpg


                        new Thread(() =>
                        {
                           
                            SOM3 som = new SOM3(load.Model, load.S1, load.DamagePixelsCount1, t, T);
                            neurons2 = som.ContinueCreateCard(neurons2);

                            if (!path.Equals(""))
                                mapRecord.Record2(neurons2, path, t+T);

                        }).Start();
                    }
                });
            }
        }

        public RelayCommand RecoveryCommand
        {
            get
            {
                return new RelayCommand(p =>
                {
                    //загружаем карту, если ее нет
                    if (neurons2 == null)
                    {
                        MapRecordAndReading2 mapReading = new MapRecordAndReading2();
                        ChooseFile file = new ChooseFile();
                        neurons2 = mapReading.Read(file.ChooseDatFile());
                    }
                    //загружаем сломанное изображение, если его нет
                    if (load.Model == null)
                    {
                        load.LoadReadyFile();
                        FileName = load.FileName1; //получаю путь к jpg
                    }

                    new Thread(() => {
                        SOM3 som = new SOM3(load.Model, load.S1, load.DamagePixelsCount1);
                        som.Recovery(neurons2);
                        ImageModel = som.Image;
                        load.LoadRecoveryImageToDrive(ImageModel, FileName);
                    }).Start();

                    
                });
            } 
        }

        public RelayCommand ShowMapCommand
        {
            get 
            {
                return new RelayCommand(p =>
                {
                    ShowMap showMap = new ShowMap();
                    showMap.CreateCard();
                    if (!showMap.MapPath.Equals(""))
                        FileName = showMap.MapPath;
                });
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
