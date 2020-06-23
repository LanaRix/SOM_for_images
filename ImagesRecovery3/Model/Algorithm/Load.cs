using ImagesRecovery3.View;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Forms;

using ImageSOM;
using ImagesRecovery3.View.Forms;

namespace ImagesRecovery3.Model.Algorithm
{
    class Load
    {
        string FileName;
        ImageModel model;
        int S; //размер блока
        int DamagePixelsCount; //число поврежденных пикселей

        public string FileName1 { get => FileName; set => FileName = value; }
        public ImageModel Model { get => model; set => model = value; }
        public int S1 { get => S; set => S = value; }
        public int DamagePixelsCount1 { get => DamagePixelsCount; set => DamagePixelsCount = value; }

        public Load() 
        {
            model = null;
            S1 = -1;
            DamagePixelsCount1 = -1;
        }

        public int CurclesCount()
        {
            Form1 form1 = new Form1();
            form1.ShowDialog();

            return form1.T;
        }

        public void LoadImageToImageModel(string FileName)
        {
            this.FileName = FileName;
            try
            {
                Bitmap bitmap = new Bitmap(FileName);
                Model = new ImageModel();
                Model.Height = bitmap.Height;
                Model.Width = bitmap.Width;
                Model.Palette = bitmap.GetPixel(0, 0).A;
                Pixel [,] pixels = new Pixel [Model.Width, Model.Height];
                for (int x=0; x<Model.Width; x++)
                    for (int y=0; y<Model.Height; y++)
                    {
                        pixels[x, y] = new Pixel();
                        pixels[x, y].R = bitmap.GetPixel(x, y).R;
                        pixels[x, y].G = bitmap.GetPixel(x, y).G;
                        pixels[x, y].B = bitmap.GetPixel(x, y).B;
                        pixels[x, y].Damage = false;
                    }
                Model.Pixels = pixels;
            }
            catch
            {
                System.Windows.MessageBox.Show("Не получилось загрузить это изображение!",
                                         "Ошибка",
                                         MessageBoxButton.OK,
                                         MessageBoxImage.Error);
            }
        }

        public string SaveMapPath()
        {
            //выбор места сохранения
            System.Windows.Forms.FolderBrowserDialog DirDialog = new System.Windows.Forms.FolderBrowserDialog();
            DirDialog.Description = "Выбор директории";
            DirDialog.SelectedPath = @"C:\";
            string path = "";

            if (DirDialog.ShowDialog() == DialogResult.OK)
            {
                return DirDialog.SelectedPath;
            }
            return path;
        }

        public void LoadDamageToImageModel()
        {
            if (Model==null)
            {
                MessageBoxResult result = System.Windows.MessageBox.Show("Изображение не выбрано! Выбрать ранее созданный файл?",
                                          "Ошибка",
                                          MessageBoxButton.YesNo,
                                          MessageBoxImage.Error);
                if (result == MessageBoxResult.Yes)
                {
                    LoadReadyFile();
                }
            }
            else
            {
                //вызываю окно
                Window1 window = new Window1(Model.Width, Model.Height);
                window.ShowDialog();

                string path = @"areal.dat";
                try
                {
                    BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
                    int uk = reader.ReadInt32();

                    S1 = reader.ReadInt32();
                    DamagePixelsCount1 = 0;

                    DamageImage damageImage = new DamageImage();
                    switch (uk)
                    {
                        case 1:
                            int x1 = reader.ReadInt32();
                            int y1 = reader.ReadInt32();
                            int x2 = reader.ReadInt32();
                            int y2 = reader.ReadInt32();
                            DamagePixelsCount1 = (x2 - x1 + 1) * (y2 - y1 + 1);
                            reader.Close();
                            Model = damageImage.DamageArial(Model, x1, y1, x2, y2);
                            break;
                        case 2:
                            int count = reader.ReadInt32();
                            DamagePixelsCount1 = count;
                            reader.Close();
                            Model = damageImage.DamageRandomPixels(Model, count);
                            break;
                    }
                    LoadDamageImageToDrive();
                }
                catch
                {}
            }
        }


        public void LoadRecoveryImageToDrive(ImageModel image, string FileName)
        {
            ConvertorCC cc = new ConvertorCC();
            Bitmap b1 = new Bitmap(FileName);
            for (int i = 0; i < image.Width; i++)
                for (int j = 0; j < image.Height; j++)
                {
                    Color color = Color.FromArgb(image.Pixels[i, j].R, image.Pixels[i, j].G, image.Pixels[i, j].B);
                    b1.SetPixel(i, j, color);
                }
            //выбор места сохранения
            System.Windows.Forms.FolderBrowserDialog DirDialog = new System.Windows.Forms.FolderBrowserDialog();
            DirDialog.Description = "Выбор директории";
            DirDialog.SelectedPath = @"C:\";
            string path = "";
            int d = FileName.Length - 1;

            for (int i=FileName.Length-1; i>0;i--)
            {
                if (FileName[i]=='.')
                {
                    path = FileName.Substring(0, i) + "Recovere"+ FileName.Substring(i);
                }
            }
            b1.Save(path);
        }


        private void LoadDamageImageToDrive()
        {
            //получение картинки с поломкой
            try
            {
                Bitmap b1 = new Bitmap(FileName);
                for (int i = 0; i < Model.Width; i++)
                    for (int j = 0; j < Model.Height; j++)
                    {
                        if (Model.Pixels[i, j].Damage)
                        {
                            b1.SetPixel(i, j, Color.Black);
                        }
                    }

                //выбор места сохранения
                System.Windows.Forms.FolderBrowserDialog DirDialog = new System.Windows.Forms.FolderBrowserDialog();
                DirDialog.Description = "Выбор директории";
                DirDialog.SelectedPath = @"C:\";
                string path = "";

                if (DirDialog.ShowDialog() == DialogResult.OK)
                {
                    path = DirDialog.SelectedPath;
                }
                if (!path.Equals(""))
                {
                    //запишем само изображение для просмотра
                    b1.Save(path + "\\\\damageImage.jpg");

                    //запишем двоичный файл с информацией
                    BinaryWriter writer = new BinaryWriter(File.Open(path + "\\\\damageImage.dat", FileMode.OpenOrCreate));
                    writer.Write(path + "\\\\damageImage.jpg");
                    //Запишем размер блока
                    writer.Write(S1);
                    for (int i = 0; i < Model.Width; i++)
                        for (int j = 0; j < Model.Height; j++)
                        {
                            if (Model.Pixels[i, j].Damage)
                            {
                                writer.Write(i);
                                writer.Write(j);
                            }
                        }
                    writer.Close();

                    FileName = path + "\\\\damageImage.jpg";
                }
            }
            catch
            {}
        }

        public void LoadReadyFile()
        {
            ChooseFile chooseFile = new ChooseFile();

            //бинарный файл с данными о повреждении
            string path = chooseFile.ChooseDatFile();

            try
            {
                BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
                FileName = reader.ReadString();
                S1 = reader.ReadInt32();
                LoadImageToImageModel(FileName);
                DamagePixelsCount1 = 0;
                while (reader.PeekChar() != -1)
                {
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    DamagePixelsCount1++;
                    Model.Pixels[x, y].Damage = true;
                }
                reader.Close();
            }
            catch { }
        }
    }
}
