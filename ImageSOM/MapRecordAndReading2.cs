using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSOM
{
    public class MapRecordAndReading2
    {
        int iter;

        public int Iter { get => iter; set => iter = value; }

        public void Record(NeuronModel2[,] model, string path, int T)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(path + "\\\\map" + Convert.ToString(T) + ".dat", FileMode.OpenOrCreate));

            writer.Write(T);
            writer.Write(model.GetLength(0));
            writer.Write(model[0, 0].M.Length);

            for (int i = 0; i < model.GetLength(0); i++)
                for (int j = 0; j < model.GetLength(0); j++)
                    for (int d = 0; d < model[i, j].M.Length; d++)
                    {
                        writer.Write(model[i, j].M[d].R);
                        writer.Write(model[i, j].M[d].G);
                        writer.Write(model[i, j].M[d].B);
                    }
            writer.Close();
        }

        public void Record2(NeuronModel2[,] model, string path, int T)
        {
            BinaryWriter writer = new BinaryWriter(File.Open(path + "\\\\map"+Convert.ToString(T)+".dat", FileMode.OpenOrCreate));

            writer.Write(T);
            writer.Write(model.GetLength(0));
            writer.Write(model[0, 0].M.Length);

            for (int i = 0; i < model.GetLength(0); i++)
                for (int j = 0; j < model.GetLength(0); j++)
                    for (int d = 0; d < model[i, j].M.Length; d++)
                    {
                        writer.Write(model[i, j].M[d].R);
                        writer.Write(model[i, j].M[d].G);
                        writer.Write(model[i, j].M[d].B);
                    }
            writer.Close();
        }

        public NeuronModel2[,] Read(string path)
        {
            BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open));
            iter = reader.ReadInt32();
            int L = reader.ReadInt32();
            NeuronModel2[,] model = new NeuronModel2[L, L];
            int N = reader.ReadInt32();

            for (int i = 0; i < L; i++)
                for (int j = 0; j < L; j++)
                {
                    model[i, j] = new NeuronModel2();
                    RGB[] M = new RGB[N];
                    for (int d = 0; d < N; d++)
                    {
                        M[d]= new RGB();
                        M[d].R = reader.ReadInt32();
                        M[d].G = reader.ReadInt32();
                        M[d].B = reader.ReadInt32();
                    }
                    model[i, j].M = M;
                }

            reader.Close();
            return model;
        }
    }
}
