using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSOM
{
    public class SOM3
    {
        NeuronModel2[,] neurons;
        int CardSize;
        int S;
        int N;
        ImageModel image;
        int[,] TenBlocks;
        int[,] TenBMU;
        int DamagePixelsCount;
        int T;
        int t;

        public SOM3(ImageModel image, int S, int DamagePixelsCount)
        {
            this.Image = image;
            this.S = S;
            if (this.S % 2 == 0) this.S++;
            N = this.S * this.S;
            this.DamagePixelsCount = DamagePixelsCount;
        }

        public SOM3(ImageModel image, int S, int DamagePixelsCount, int t, int T)
        {
            this.S = S;
            N = S*S;
            this.Image = image;

            this.t = t;
            this.T = t+T;
            this.DamagePixelsCount = DamagePixelsCount;
        }
        public SOM3(ImageModel image, int S, int DamagePixelsCount, int T)
        {
            this.S = S + S / 2;
            if (this.S % 2 == 0) this.S++;
            N = this.S * this.S;
            Initialization initialization = new Initialization();
            CardSize = initialization.CardLengthCalculation(image.Height, image.Width, DamagePixelsCount, this.S);
            neurons = initialization.CardInitialization(CardSize, N);
            //this.S = S;
            this.Image = image;
            this.DamagePixelsCount = DamagePixelsCount;

            //10 BMU
            //массив 10х2
            TenBMU = initialization.BMU(CardSize);
            this.T = T;
        }

        public ImageModel Image { get => image; set => image = value; }

        public void Create10Points()
        {
            First10 first10 = new First10();

            //10 блоков, несущие максимальные кол=во информации (главные точки блоков)
            //массив 10х2
            TenBlocks = first10.P10MAX(Image, S);
            int[,] bestPixel = new int[3, 2];

            for (int i = 1; i <= 10; i++)
            {
                bestPixel[0, 0] = TenBlocks[i - 1, 0]; bestPixel[0, 1] = TenBlocks[i - 1, 1];
                bestPixel[1, 0] = TenBlocks[i + 10 - 1, 0]; bestPixel[1, 1] = TenBlocks[i + 10 - 1, 1];
                bestPixel[2, 0] = TenBlocks[i + 20 - 1, 0]; bestPixel[2, 1] = TenBlocks[i + 20 - 1, 1];

                first10.MapErrorCalculation(
                    neurons,
                    Image,
                    bestPixel,
                    CardSize,
                    new int[] { TenBMU[i - 1, 0], TenBMU[i - 1, 1] },
                    i,
                    S, N);
            }
        }

        public NeuronModel2[,] CreateCard()
        {
            CreateMap createMap = new CreateMap();


            t = 11;
            do
            {
                //создание вектора данных
                int[,] vector = createMap.ChoosePixel(S, Image, N);

                //поиск BMU
                int[,] BMU = createMap.BMU(neurons, vector, Image, N);

                //пересчет координат модели
                createMap.MapErrorCalculation(neurons, Image, vector, CardSize, BMU, t, S, N);
                t++;
            }
            while (t < T);

            return neurons;
        }

        public NeuronModel2[,] ContinueCreateCard(NeuronModel2[,] neurons)
        {
            CreateMap createMap = new CreateMap();

            CardSize = neurons.GetLength(0);
            do
            {
                //создание вектора данных
                int[,] vector = createMap.ChoosePixel(S, Image, N);

                //поиск BMU
                int[,] BMU = createMap.BMU(neurons, vector, Image, N);

                //пересчет координат модели
                createMap.MapErrorCalculation(neurons, Image, vector, CardSize, BMU, t, S, N);
                t++;
            }
            while (t<T);

            return neurons;
        }


        //Функция восстановления изображения
        public void Recovery(NeuronModel2[,] neurons)
        {
            ImageRecovery recovery = new ImageRecovery();

            //пока не воостановлены все поврежденные пиксели
            while (DamagePixelsCount > 0)
            {
                //получаю вектор с повреждением
                int[,] vector = recovery.CreateVectorWithDamage(Image, S, N);

                //находу ближайший нейрон
                int[,] BMU = recovery.SearchBestNeuron(vector, Image, neurons);


                //восстанавливаю изображение
                DamagePixelsCount = recovery.Recovery(vector, BMU, Image, neurons, DamagePixelsCount);
            }

         }

        class Initialization
        {
            //вычисление стороны квадрата карты
            public int CardLengthCalculation(int ImageHeight, int ImageWidth, int DamagePixelsCount, int S)
            {
                int CleanAreal = ((ImageHeight * ImageWidth) / 2 - DamagePixelsCount - S * S);
                int lenght = Convert.ToInt32(Math.Pow(0.5 * CleanAreal, 0.5));

                if (Math.Pow(lenght, 2) == CleanAreal)
                    return lenght--;

                return lenght;
            }

            //задание карте начальных значений
            public NeuronModel2[,] CardInitialization(int lenght, int N)
            {
                NeuronModel2[,] neurons = new NeuronModel2[lenght, lenght];
                for (int x = 0; x < lenght; x++)
                    for (int y = 0; y < lenght; y++)
                    {
                        neurons[x, y] = new NeuronModel2();

                        //задание вектора значений писеля
                        RGB[] M = new RGB[N];
                        for (int i = 0; i < N; i++)
                        {
                            M[i] = new RGB();
                            M[i].B = 0;
                            M[i].G = 0;
                            M[i].R = 0;
                        }
                        neurons[x, y].M = M;
                    }
                return neurons;
            }
            public int[,] BMU(int CardSize)
            {
                int r = (CardSize / 6) * 2;
                if (r == 0) r++;

                int[,] coordin = new int[10, 2];

                coordin[9, 0] = CardSize / 2;
                coordin[9, 1] = CardSize / 2;

                for (int i = 0; i < 9; i++)
                {
                    int alfa = 40 * i;
                    coordin[i, 0] = Convert.ToInt32(r * Math.Cos(alfa * Math.PI / 180)) + coordin[9, 0];
                    coordin[i, 1] = Convert.ToInt32(r * Math.Sin(alfa * Math.PI / 180)) + coordin[9, 1];
                }
                return coordin;
            }
        }

        class First10
        {
            public int[,] P10MAX(ImageModel model, int S)
            {
                bool damage;
                double symmaR, symmaG, symmaB;
                double[,] max = new double[30, 3];

                //задаем отрицательные значения - блоки еще не выбраны
                for (int k = 0; k < 30; k++)
                {
                    max[k, 0] = -1;
                    max[k, 1] = -1;
                    max[k, 2] = -1;
                }

                //пока не прошли по всему изображению
                for (int x = S / 2; x < model.Width - S / 2 - 1; x++)
                    for (int y = S / 2; y < model.Height - S / 2 - 1; y++)
                    {
                        symmaR = 0;
                        symmaG = 0;
                        symmaB = 0;
                        damage = false;

                        //если текущая точка не повреждена
                        if (model.Pixels[x, y].Damage == false)
                            for (int i = x - S / 2; i <= S / 2 + x; i++)
                                for (int j = y - S / 2; j <= S / 2 + y; j++)
                                {
                                    //если блок не поврежден
                                    if (damage == false)
                                    {
                                        //если текущая точка оказалась поврежденной
                                        if (model.Pixels[i, j].Damage == true)
                                            damage = true;

                                        //а если не поврежденной
                                        else if (i != x && j != y)
                                        {
                                            symmaR += Math.Pow(Math.Abs(model.Pixels[i, j].R - model.Pixels[x, y].R), 2);
                                            symmaG += Math.Pow(Math.Abs(model.Pixels[i, j].G - model.Pixels[x, y].G), 2);
                                            symmaB += Math.Pow(Math.Abs(model.Pixels[i, j].B - model.Pixels[x, y].B), 2);
                                        }
                                    }
                                }

                        //как только закончили считать информацию от блока
                        //проверяем, является ли он одним из 10 несущих макс количество информации
                        if (damage == false)
                        {
                            //R
                            for (int k = 0; k < 10; k++)
                            {
                                //если значение еще не найдено
                                if (max[k, 0] == -1)
                                {
                                    max[k, 0] = x;
                                    max[k, 1] = y;
                                    max[k, 2] = symmaR;
                                    break;
                                }

                                else if (symmaR > max[k, 2])
                                {
                                    for (int j = 9; j > k; j--)
                                    {
                                        max[j, 0] = max[j - 1, 0];
                                        max[j, 1] = max[j - 1, 1];
                                        max[j, 2] = max[j - 1, 2];
                                    }
                                    max[k, 0] = x;
                                    max[k, 1] = y;
                                    max[k, 2] = symmaR;
                                    break;
                                }
                            }

                            //G
                            for (int k = 10; k < 20; k++)
                            {
                                //если значение еще не найдено
                                if (max[k, 0] == -1)
                                {
                                    max[k, 0] = x;
                                    max[k, 1] = y;
                                    max[k, 2] = symmaG;
                                    break;
                                }

                                else if (symmaR > max[k, 2])
                                {
                                    for (int j = 19; j > k; j--)
                                    {
                                        max[j, 0] = max[j - 1, 0];
                                        max[j, 1] = max[j - 1, 1];
                                        max[j, 2] = max[j - 1, 2];
                                    }
                                    max[k, 0] = x;
                                    max[k, 1] = y;
                                    max[k, 2] = symmaG;
                                    break;
                                }
                            }

                            //B
                            for (int k = 20; k < 30; k++)
                            {
                                //если значение еще не найдено
                                if (max[k, 0] == -1)
                                {
                                    max[k, 0] = x;
                                    max[k, 1] = y;
                                    max[k, 2] = symmaB;
                                    break;
                                }

                                else if (symmaR > max[k, 2])
                                {
                                    for (int j = 29; j > k; j--)
                                    {
                                        max[j, 0] = max[j - 1, 0];
                                        max[j, 1] = max[j - 1, 1];
                                        max[j, 2] = max[j - 1, 2];
                                    }
                                    max[k, 0] = x;
                                    max[k, 1] = y;
                                    max[k, 2] = symmaB;
                                    break;
                                }
                            }
                        }

                    }

                int[,] coordin = new int[30, 2];
                for (int i = 0; i < 30; i++)
                {
                    coordin[i, 0] = (int)max[i, 0];
                    coordin[i, 1] = (int)max[i, 1];
                }
                return coordin;
            }

           
            /// <summary>
            /// Расстояние между нейронами на карте
            /// </summary>
            /// <param name="BMU">Координаты MBU</param>
            /// <param name="neuron">Координаты второго нейрона</param>
            /// <param name="Q">Размерность карты</param>
            double d(int[] BMU, int[] neuron, int Q)
            {
                double r = -1;
                for (int i = -Q; i <= Q; i += Q)
                    for (int j = -Q; j <= Q; j += Q)
                    {
                        double r1 = Math.Pow(Math.Pow(BMU[0] - neuron[0] + i, 2) + Math.Pow(BMU[1] - neuron[1] + j, 2), 0.5);

                        if (r == -1 || r1 < r)
                            r = r1;
                    }
                return r;
            }

            /// <summary>
            /// определяем меру соседства нейронов
            /// </summary>
            /// <param name="CardSize">Размерность карты</param>
            /// <param name="t">№ итерации</param>
            /// <param name="BMU">Координаты MBU</param>
            /// <param name="neuron">Координаты второго нейрона</param>
            double H(int CardSize, int t, int[] BMU, int[] neuron)
            {
                double a = t < 10 ? 1 : 1 / Math.Pow(t - 9, 0.2);
                double q = 5 * Math.Pow(Math.Pow(CardSize, 2), 0.5) / Math.Pow(t, 0.5);
                return a * Math.Exp((-1) * d(BMU, neuron, CardSize) / (2 * q));
            }

            /// <summary>
            /// Нейроны, иправленне с учетом ошибки карты
            /// </summary>
            /// <param name="neurons">нейроны</param>
            /// <param name="image">изображение</param>
            /// <param name="bestPixel">координаты х и y выбранного пикселя</param>
            /// <param name="CardSize">Размер карты</param>
            /// <param name="BMU">Координаты MBU (x и y)</param>
            /// <param name="t">№ итерации</param>
            /// <param name="S">Размерность блока (длина вектора)</param>
            /// <returns></returns>
            public void MapErrorCalculation(NeuronModel2[,] neurons, ImageModel image, int[,] bestPixel, int CardSize, int[] BMU, int t, int S, int N)
            {
                //пока не прошли по всем нейронам карты
                for (int x = 0; x < CardSize; x++)
                    for (int y = 0; y < CardSize; y++)
                    {
                        //вычисляем расстояние от нейрона (x;y) до BMU
                        double h = H(CardSize, t, BMU, new int[] { x, y });

                        //пока не прошли по всему блоку
                        for (int n = 0; n < N; n++)
                        {

                            //получаю кординаты пикселя из блока
                            int iR = bestPixel[0,0] - S / 2 + ((n + 1) % S);
                            int jR = bestPixel[0,1] - S / 2 + ((n + 1) / S);

                            int iG = bestPixel[1,0] - S / 2 + ((n + 1) % S);
                            int jG = bestPixel[1,1] - S / 2 + ((n + 1) / S);

                            int iB = bestPixel[2, 0] - S / 2 + ((n + 1) % S);
                            int jB = bestPixel[2, 1] - S / 2 + ((n + 1) / S);

                            //пересчитываю значение координаты нейрона
                            neurons[x, y].M[n].B += Convert.ToInt32(h * (image.Pixels[iR, jR].B - neurons[x, y].M[n].B));
                            neurons[x, y].M[n].G += Convert.ToInt32(h * (image.Pixels[iG, jG].G - neurons[x, y].M[n].G));
                            neurons[x, y].M[n].R += Convert.ToInt32(h * (image.Pixels[iB, jB].R - neurons[x, y].M[n].R));
                        }
                    }
            }

        }

        class CreateMap
        {
            /// <summary>
            /// Выбираем случайный вектор из набора входных значений. Возвращаем вектор координат из блока
            /// </summary>
            /// <param name="S">Размер блока</param>
            /// <param name="model">Изображение</param>
            /// <returns>Возврашает координаты точек вектора</returns>
            public int[,] ChoosePixel(int S, ImageModel model, int N)
            {
                int[,] coordin = new int[N, 2];
                int count = 0;
                RandomCalculations random = new RandomCalculations();

                do
                {
                    int x = random.getRandom(S / 2, model.Width - S / 2 - 1);
                    int y = random.getRandom(S / 2, model.Height - S / 2 - 1);

                    for (int d = 0; d < N; d++)
                    {
                        coordin[d, 0] = x - S / 2 + ((d + 1) % S);
                        coordin[d, 1] = y - S / 2 + ((d + 1) / S);

                        if (model.Pixels[coordin[d, 0], coordin[d, 1]].Damage)
                            break;
                        if (d == N - 1) return coordin;
                    }
                    coordin = new int[N, 2];
                    count++;
                    if (count == 2 * model.Width * model.Height) return null;
                }
                while (true);
            }

            /// <summary>
            /// Находим MBU для этого вектора данных. Возращаем координаты.
            /// </summary>
            /// <param name="neuron">Карта</param>
            /// <param name="pixel">Пиксель вектора</param>
            /// <param name="image"></param>
            /// <param name="S"></param>
            /// <returns></returns>
            public int[,] BMU(NeuronModel2[,] neuron, int[,] vector, ImageModel image, int N)
            {
                double distanceR, distanceG, distanceB;
                double MinDistanceR = -1, MinDistanceG = -1, MinDistanceB = -1;
                List<int> coordinR = new List<int>();
                List<int> coordinG = new List<int>();
                List<int> coordinB = new List<int>();

                for (int x = 0; x < neuron.GetLength(0); x++)
                    for (int y = 0; y < neuron.GetLength(1); y++)
                    {
                        distanceR = 0.0; distanceG = 0.0; distanceB = 0.0;
                        //считаю среднеквадратичное отклонение
                        for (int d = 0; d < N; d++)
                        {
                            int i = vector[d, 0];
                            int j = vector[d, 1];

                            distanceR += Math.Pow(neuron[x, y].M[d].R - image.Pixels[i, j].R, 2);
                            distanceG += Math.Pow(neuron[x, y].M[d].G - image.Pixels[i, j].G, 2);
                            distanceB += Math.Pow(neuron[x, y].M[d].B - image.Pixels[i, j].B, 2);
                        }
                        distanceR = Math.Pow(distanceR, 0.5);
                        distanceG = Math.Pow(distanceG, 0.5);
                        distanceB = Math.Pow(distanceB, 0.5);

                        //проверяю, является ли текущее среднеквадратичное отклонение минимальным
                        if (MinDistanceR == -1 || distanceR <= MinDistanceR)
                        {
                            //если одна из минимальных
                            if (distanceR == MinDistanceR)
                            {
                                coordinR.Add(x);
                                coordinR.Add(y);
                            }
                            //если новая минимальная
                            else
                            {
                                MinDistanceR = distanceR;
                                coordinR = new List<int>();
                                coordinR.Add(x);
                                coordinR.Add(y);
                            }
                        }
                        if (MinDistanceG == -1 || distanceG <= MinDistanceG)
                        {
                            //если одна из минимальных
                            if (distanceG == MinDistanceG)
                            {
                                coordinG.Add(x);
                                coordinG.Add(y);
                            }
                            //если новая минимальная
                            else
                            {
                                MinDistanceG = distanceG;
                                coordinG = new List<int>();
                                coordinG.Add(x);
                                coordinG.Add(y);
                            }
                        }
                        if (MinDistanceB == -1 || distanceB <= MinDistanceB)
                        {
                            //если одна из минимальных
                            if (distanceB == MinDistanceB)
                            {
                                coordinB.Add(x);
                                coordinB.Add(y);
                            }
                            //если новая минимальная
                            else
                            {
                                MinDistanceB = distanceB;
                                coordinB = new List<int>();
                                coordinB.Add(x);
                                coordinB.Add(y);
                            }
                        }
                    }

                int[,] coordin = new int[3, 2];

                if (coordinR.Count>2)
                {
                    RandomCalculations random = new RandomCalculations();
                    int number = random.getRandom(0, coordinR.Count);
                    if (number%2==0)
                    {
                        coordin[0, 0] = coordinR[number];
                        coordin[0, 1] = coordinR[number+1];
                    }
                    else
                    {
                        coordin[0, 0] = coordinR[number - 1];
                        coordin[0, 1] = coordinR[number];
                    }
                }
                else
                {
                    coordin[0, 0] = coordinR[0];
                    coordin[0, 1] = coordinR[1];
                }


                if (coordinG.Count > 2)
                {
                    RandomCalculations random = new RandomCalculations();
                    int number = random.getRandom(0, coordinG.Count);
                    if (number % 2 == 0)
                    {
                        coordin[1, 0] = coordinG[number];
                        coordin[1, 1] = coordinG[number + 1];
                    }
                    else
                    {
                        coordin[1, 0] = coordinG[number - 1];
                        coordin[1, 1] = coordinG[number];
                    }
                }
                else
                {
                    coordin[1, 0] = coordinG[0];
                    coordin[1, 1] = coordinG[1];
                }

                if (coordinB.Count > 2)
                {
                    RandomCalculations random = new RandomCalculations();
                    int number = random.getRandom(0, coordinB.Count);
                    if (number % 2 == 0)
                    {
                        coordin[2, 0] = coordinB[number];
                        coordin[2, 1] = coordinB[number + 1];
                    }
                    else
                    {
                        coordin[2, 0] = coordinB[number - 1];
                        coordin[2, 1] = coordinB[number];
                    }
                }
                else
                {
                    coordin[2, 0] = coordinB[0];
                    coordin[2, 1] = coordinB[1];
                }
                return coordin;
            }

            /// <summary>
            /// Расстояние между нейронами на карте
            /// </summary>
            /// <param name="BMU">Координаты MBU</param>
            /// <param name="neuron">Координаты второго нейрона</param>
            /// <param name="Q">Размерность карты</param>
            double d(int[] BMU, int[] neuron, int Q)
            {
                double r = -1;
                for (int i = -Q; i <= Q; i += Q)
                    for (int j = -Q; j <= Q; j += Q)
                    {
                        double r1 = Math.Pow(Math.Pow(BMU[0] - neuron[0] + i, 2) + Math.Pow(BMU[1] - neuron[1] + j, 2), 0.5);

                        if (r == -1 || r1 < r)
                            r = r1;
                    }
                return r;
            }

            /// <summary>
            /// определяем меру соседства нейронов
            /// </summary>
            /// <param name="CardSize">Размерность карты</param>
            /// <param name="t">№ итерации</param>
            /// <param name="BMU">Координаты MBU</param>
            /// <param name="neuron">Координаты второго нейрона</param>
            double H(int CardSize, int t, int[] BMU, int[] neuron)
            {
                double a = 1 / Math.Pow(t - 9, 0.2);
                double q = 5 * Math.Pow(Math.Pow(CardSize, 2), 0.5) / Math.Pow(t, 0.5);
                return a * Math.Exp((-1) * d(BMU, neuron, CardSize) / (2 * q));
            }

            /// <summary>
            /// Нейроны, иправленне с учетом ошибки карты
            /// </summary>
            /// <param name="neurons">нейроны</param>
            /// <param name="image">изображение</param>
            /// <param name="Vector">Вектор из набора данных</param>
            /// <param name="CardSize">Размер карты</param>
            /// <param name="BMU">Координаты MBU (x и y)</param>
            /// <param name="t">№ итерации</param>
            /// <param name="S">Размерность блока (длина вектора)</param>
            /// <returns></returns>
            public void MapErrorCalculation(NeuronModel2[,] neurons, ImageModel image, int[,] Vector, int CardSize, int[,] BMU, int t, int S, int N)
            {
                //пока не прошли по всем нейронам карты
                for (int x = 0; x < CardSize; x++)
                    for (int y = 0; y < CardSize; y++)
                    {
                        //вычисляем расстояние от нейрона (x;y) до BMU
                        double hR = H(CardSize, t, new int[] { BMU[0, 0], BMU[0, 1] }, new int[] { x, y });
                        double hG = H(CardSize, t, new int[] { BMU[1, 0], BMU[1, 1] }, new int[] { x, y });
                        double hB = H(CardSize, t, new int[] { BMU[2, 0], BMU[2, 1] }, new int[] { x, y });

                        //пока не прошли по всему блоку
                        for (int d = 0; d < N; d++)
                        {
                            //получаю кординаты пикселя из блока
                            int i = Vector[d, 0];
                            int j = Vector[d, 1];

                            neurons[x, y].M[d].R += Convert.ToInt32(hR * (image.Pixels[i, j].R - neurons[x, y].M[d].R));
                            neurons[x, y].M[d].G += Convert.ToInt32(hG * (image.Pixels[i, j].G - neurons[x, y].M[d].G));
                            neurons[x, y].M[d].B += Convert.ToInt32(hB * (image.Pixels[i, j].B - neurons[x, y].M[d].B));
                        }
                    }
            }
        }

        class ImageRecovery
        {
            /// <summary>
            /// Находит вектор с минимальным числом повреждений
            /// </summary>
            /// <param name="image">изображение</param>
            /// <param name="S">блина блока</param>
            /// <param name="N">длина вектора</param>
            /// <returns>Возвращает набор координат точек вектора</returns>
            public int[,] CreateVectorWithDamage(ImageModel image, int S, int N)
            {
                int uk = N + 1;
                int[,] coordinBest = new int[N, 2];
                int[,] coordin = new int[N, 2];

                for (int x = S / 2; x < image.Width - S / 2 - 1; x++)
                    for (int y = S / 2; y < image.Height - S / 2 - 1; y++)
                    {
                        int CountDamagePixelsInCurrentVector = 0;
                        for (int d = 0; d < N; d++)
                        {
                            coordin[d, 0] = x - S / 2 + ((d + 1) % S);
                            coordin[d, 1] = y - S / 2 + ((d + 1) / S);
                            if (image.Pixels[coordin[d, 0], coordin[d, 1]].Damage == true)
                                CountDamagePixelsInCurrentVector++;
                        }

                        //если есть вектор с меньшим числом повреждений
                        if (CountDamagePixelsInCurrentVector < uk && CountDamagePixelsInCurrentVector > 0)
                        {
                            coordinBest = coordin;
                            coordin = new int[N, 2];
                            uk = CountDamagePixelsInCurrentVector;
                        }

                        //если нашли вектор с 1 поврежденным пикселем
                        if (uk == 1)
                            return coordinBest;
                    }

                return coordinBest;
            }

            /// <summary>
            /// Находит Ближайший нейрон (BMU)
            /// </summary>
            /// <param name="vectorWithDamage">вектор данных с повреждениями</param>
            /// <param name="image">изображение</param>
            /// <param name="neurons">нейроны</param>
            /// <returns>Координаты BMU нейрона</returns>
            public int[,] SearchBestNeuron(int[,] vectorWithDamage, ImageModel image, NeuronModel2[,] neurons)
            {
                int[,] BMU = new int[3, 2];
                double DR, DminR = -1;
                double DG, DminG = -1;
                double DB, DminB = -1;

                for (int i = 0; i < neurons.GetLength(0); i++)
                    for (int j = 0; j < neurons.GetLength(1); j++)
                    {
                        DR = 0; DG = 0; DB = 0;
                        for (int d = 0; d < vectorWithDamage.GetLength(0); d++)
                            //если пиксель не поврежден и не восстановлен
                            if (image.Pixels[vectorWithDamage[d, 0], vectorWithDamage[d, 1]].Damage == false && image.Pixels[vectorWithDamage[d, 0], vectorWithDamage[d, 1]].Recovery==false)
                            {
                                DR += Math.Pow(neurons[i, j].M[d].R - image.Pixels[vectorWithDamage[d, 0], vectorWithDamage[d, 1]].R, 2);
                                DG += Math.Pow(neurons[i, j].M[d].G - image.Pixels[vectorWithDamage[d, 0], vectorWithDamage[d, 1]].G, 2);
                                DB += Math.Pow(neurons[i, j].M[d].B - image.Pixels[vectorWithDamage[d, 0], vectorWithDamage[d, 1]].B, 2);
                            }

                        if (DminR == -1 || DR < DminR)
                        {
                            BMU[0,0] = i;
                            BMU[0,1] = j;
                            DminR = DR;
                        }
                        if (DminG == -1 || DG < DminG)
                        {
                            BMU[1, 0] = i;
                            BMU[1, 1] = j;
                            DminG = DG;
                        }
                        if (DminB == -1 || DB < DminB)
                        {
                            BMU[2, 0] = i;
                            BMU[2, 1] = j;
                            DminB = DB;
                        }
                    }

                return BMU;

            }

            /// <summary>
            /// Восстанавливает значения пикселей изображения по карте
            /// </summary>
            /// <param name="vector">Возвращает набор координат точек вектора</param>
            /// <param name="MBU">Координаты BMU нейрона</param>
            /// <param name="image">изображение</param>
            /// <param name="neurons">нейроны</param>
            public int Recovery(int[,] vector, int[,] MBU, ImageModel image, NeuronModel2[,] neurons, int DamagePixelsCount)
            {
                for (int d = 0; d < vector.GetLength(0); d++)
                {
                    if (image.Pixels[vector[d, 0], vector[d, 1]].Damage == true)
                    {
                        image.Pixels[vector[d, 0], vector[d, 1]].R = neurons[MBU[0,0], MBU[0,1]].M[d].R;
                        image.Pixels[vector[d, 0], vector[d, 1]].G = neurons[MBU[1,0], MBU[1,1]].M[d].G;
                        image.Pixels[vector[d, 0], vector[d, 1]].B = neurons[MBU[2,0], MBU[2,1]].M[d].B;

                        DamagePixelsCount--;
                        image.Pixels[vector[d, 0], vector[d, 1]].Damage = false;
                        image.Pixels[vector[d, 0], vector[d, 1]].Recovery = true;
                    }
                }
                return DamagePixelsCount;
            }
        }
    }
}
