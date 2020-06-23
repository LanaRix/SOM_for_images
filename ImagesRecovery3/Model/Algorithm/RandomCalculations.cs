using System;

namespace ImagesRecovery3.Model.Algorithm
{
    class RandomCalculations
    {
        private Random rnd = new Random();

        public Int32 getRandom(Int32 iMin, Int32 iMax)
        {
            return rnd.Next(iMin, iMax);
        }
    }
}
