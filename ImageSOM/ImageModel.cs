using System.Collections.Generic;

namespace ImageSOM
{
    public class ImageModel
    {
        int width, height, palette;
        Pixel[,] pixels;

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public int Palette { get => palette; set => palette = value; }
        public Pixel[,] Pixels { get => pixels; set => pixels = value; }
    }

    public class Pixel
    {
        bool damage, recovere=false; //наличие возреждения у пикселя
        int r, g, b; //набор цветов пикселя
        int pixelValue;

        public bool Damage { get => damage; set => damage = value; }
        public int R { get => r; set => r = value; }
        public int G { get => g; set => g = value; }
        public int B { get => b; set => b = value; }
        public int PixelValue { get => pixelValue; set => pixelValue = value; }
        public bool Recovery { get => recovere; set => recovere = value; }
    }
}