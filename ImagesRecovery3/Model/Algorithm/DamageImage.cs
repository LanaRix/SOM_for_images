using ImageSOM;

namespace ImagesRecovery3.Model.Algorithm
{
    class DamageImage
    {
        public DamageImage() { }

        /// <summary>
        /// Повреждает заданную область изображения
        /// </summary>
        /// <param name="model">Изображение</param>
        /// <param name="XLeftTop">Начальная координата X</param>
        /// <param name="YLeftTop">Начальная координата Y</param>
        /// <param name="XRightBottom">Конечная координата X</param>
        /// <param name="YRightBottom">Конечная координата Y</param>
        /// <returns></returns>
        public ImageModel DamageArial(ImageModel model, int XLeftTop, int YLeftTop, int XRightBottom, int YRightBottom)
        {
            for (int x = XLeftTop; x <= XRightBottom; x++)
                for (int y = YLeftTop; y <= YRightBottom; y++)
                {
                    try
                    {
                        model.Pixels[x, y].Damage = true;
                    }
                    catch { }
                }
            return model;
        }

        /// <summary>
        /// Повреждает набор случайных пикселей
        /// </summary>
        /// <param name="model">Изображение</param>
        /// <param name="Count">Количество пикселей, которые надо повредить</param>
        /// <returns></returns>
        public ImageModel DamageRandomPixels(ImageModel model, int Count)
        {
            RandomCalculations random = new RandomCalculations();
            int current = 0;
            int x, y;
            while (current < Count)
            {
                x = random.getRandom(0, model.Width);
                y = random.getRandom(0, model.Height);

                if (!model.Pixels[x, y].Damage)
                {
                    model.Pixels[x, y].Damage = true;
                    current++;
                }
            }
            return model;
        }
    }
}
