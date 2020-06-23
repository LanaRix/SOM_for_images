using System;

namespace ImageSOM
{
    public class ConvertorCC
    {
        public string From10To16(int number10)
        {
            string s = Convert.ToString(number10, 16);

            if (s.Length < 2)
                s = s.Insert(0, "0");
            return s;
        }

        public int From16To10(string number16)
        {
            return Convert.ToInt32(number16, 16);
        }

        public int [] from16toRGB(string number16)
        {
            int[] RGB = new int[3];

            int c = number16.Length;
            if (c>1)
            {
                RGB[2] = From16To10(number16.Substring(c - 2));
                number16 = number16.Substring(0, c - 2);

                c = number16.Length;
                if (c>1)
                {
                    RGB[1] = From16To10(number16.Substring(c - 2));
                    RGB[0] = From16To10(number16.Substring(0, c - 2));
                }
                else
                {
                    RGB[1] = From16To10(number16);
                    RGB[0] = 0;
                }
            }
            else
            {
                RGB[2] = From16To10(number16);
                RGB[1] = 0;
                RGB[0] = 0;
            }

            return RGB;
        }
    }
}
