using ImageSOM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImagesRecovery3.Model.Algorithm
{
    class ShowMap
    {
        string mapPath;

        public string MapPath { get => mapPath; set => mapPath = value; }

        public void CreateCard()
        {
            ChooseFile file = new ChooseFile();
            //бинарный файл с данными о повреждении
            string path = file.ChooseDatFile();

            Load load = new Load();
            string pathSave = load.SaveMapPath();

            MapRecordAndReading2 reading = new MapRecordAndReading2();

            if (!path.Equals(""))
            {

                NeuronModel2[,] model = reading.Read(path);


                int D = model[0, 0].M.Length;
                int L = Convert.ToInt32(Math.Pow(model[0, 0].M.Length, 0.5));
                int Q = model.GetLength(0);
                int number = 10;

                int f = model.GetLength(0) * L;
                Bitmap b1 = new Bitmap(f, f);
                //Bitmap b1 = new Bitmap(model.GetLength(0), model.GetLength(0));
                ConvertorCC cc = new ConvertorCC();

                for (int x = 0; x < Q; x++)
                    for (int y = 0; y < Q; y++)
                        for (int i = 0; i < L; i++)
                            for (int j = 0; j < L; j++)
                            {
                                number = L * i + j;
                                Color color = Color.FromArgb(model[x, y].M[number].R, model[x, y].M[number].G, model[x, y].M[number].B);
                                b1.SetPixel(i + (x * L), j + (y * L), color);
                            }

                
                b1.Save(pathSave + "\\\\ImageMap.jpg");
                MapPath = pathSave + "\\\\ImageMap.jpg";
            }
        }
    }
}
